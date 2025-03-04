using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using HiveSpace.Application.Interfaces;
using HiveSpace.Domain.AggergateModels;
using HiveSpace.Domain.Enums;
using HiveSpace.Infrastructure;
using HiveSpace.TestDataLoader.Features.Models;
using Azure.Storage.Blobs.Models;
using System.Threading;
using HiveSpace.Application.Extensions;

namespace HiveSpace.TestDataLoader.Features;

public record SyncShoppeDataCommand : IRequest { }

public class SyncShoppeDataCommandHandler(IShoppeApi shoppeApi, IConfiguration configuration, HiveSpaceDbContext context, IStorageService storageService, IMediator mediator) : IRequestHandler<SyncShoppeDataCommand>
{
    private readonly IMediator _mediator = mediator;
    private readonly IStorageService _storageService = storageService;
    private readonly IShoppeApi _shoppeApi = shoppeApi;
    private readonly IConfiguration _configuration = configuration;
    private readonly HiveSpaceDbContext _context = context;
    private readonly Dictionary<string, AttributeProduct> _attributeDict = [];
    private readonly Dictionary<int, Category> _categoryDict = [];
    private readonly Dictionary<string, string> _categoryImageDict = [];
    private readonly string _shoppeImageBaseUrl = "https://down-vn.img.susercontent.com/file";
    public async Task Handle(SyncShoppeDataCommand request, CancellationToken cancellationToken)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        if (environment != "Development")
        {
            Console.WriteLine("This option is only for Development environment");
            return;
        }

        var cookie = _configuration["CookieShoppe"];

        if (string.IsNullOrEmpty(cookie)) return;

        await LoadCategoryImageAsync();
        var categoryIds = await SyncCategoryDataAsync(cookie);
        await _mediator.Send(new CopyBlobStorageCommand() { IsCopyFromDevToProd = true, StorageType = StorageType.CategoryImages }, cancellationToken);
        //await SyncAttributeDataAsync(cookie, categoryIds);
    }

    private async Task<List<int>> SyncCategoryDataAsync(string cookie)
    {
        var response = await _shoppeApi.GetCategoryTreeAsync(
            SPC_CDS: "dc602070-5eba-42f7-bfe6-44eb62e6935b",    // SPC_CDS
            SPC_CDS_VER: "2",                                  // SPC_CDS_VER
            include_level: "3",                                // include_level
            version: "1.0.1",                                  // version
            locale: "vi",                                     // locale
            priority: "u=1, i",                                // priority
            session: "9B35983F9D4091BA",                        // sc-fe-session
            versionHeader: "21.72231",                         // sc-fe-ver
            cookie: cookie
        );

        if (!response.IsSuccessStatusCode || response?.Content?.Data?.List is null)
        {
            return [];
        }

        var categories = response.Content.Data.List;
        _context.Category.RemoveRange(_context.Category);
        await _context.SaveChangesAsync();

        // Prepare batch insert
        var categoryIds = new List<int>();

        foreach (var category in categories)
        {
            categoryIds.AddRange(InsertCategory(category));
        }

        await _context.SaveChangesAsync();
        return categoryIds;
    }

    private List<int> InsertCategory(CategoryShoppe categoryShoppe, string? parentName = null)
    {
        var ids = new List<int>
        {
            categoryShoppe.Id
        };

        int? parentId = categoryShoppe.ParentId == 0 ? null : categoryShoppe.ParentId;

        _categoryImageDict.TryGetValue($"{categoryShoppe.Name}-{parentName}", out var fileImageId);
        var category = new Category(categoryShoppe.Id, categoryShoppe.Name, categoryShoppe.DisplayName, parentId, fileImageId);

        _context.Category.Add(category);
        _categoryDict.Add(category.Id, category);

        foreach (var childCategory in categoryShoppe.Children)
        {
            ids.AddRange(InsertCategory(childCategory, categoryShoppe.Name));
        }
        return ids;
    }

    private async Task LoadCategoryImageAsync()
    {
        string jsonFilePath = "Json/categories.json"; // Path to your JSON file
                                                      // Read JSON file
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), jsonFilePath);
        string jsonContent = File.ReadAllText(filePath);
        var categories = JsonConvert.DeserializeObject<List<CategoryJson>>(jsonContent);

        if (categories is null)
        {
            return;
        }

        var containerClient = await _storageService.GetContainerClient(StorageType.CategoryImages.GetDisplayName());
        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            await blobClient.DeleteIfExistsAsync();
        }

        foreach (var category in categories)
        {
            await UpdateCategoryImageDictAsync(category);
        }

        async Task UpdateCategoryImageDictAsync(CategoryJson category, string? parentName = null)
        {
            if (!string.IsNullOrEmpty(category.Image))
            {
                byte[] imageData = await DownloadImageAsync($"{_shoppeImageBaseUrl}/{category.Image}");
                string fileImageId = (await _storageService.UploadFromByteDataAsync(imageData, StorageType.CategoryImages, "image/jpeg")).ToString();
                _categoryImageDict.Add($"{category.Name}-{parentName}", fileImageId);
            }
            foreach (CategoryJson childCategory in category.Children ?? [])
            {
                await UpdateCategoryImageDictAsync(childCategory, category.Name);
            }
        }
    }
    static async Task<byte[]> DownloadImageAsync(string url)
    {
        using HttpClient client = new();
        return await client.GetByteArrayAsync(url);
    }

    private async Task SyncAttributeDataAsync(string cookie, List<int> categoryIds)
    {
        _context.AttributeProduct.RemoveRange(_context.AttributeProduct);
        await _context.SaveChangesAsync();

        foreach (var categoryId in categoryIds)
        {
            var response = await _shoppeApi.GetAttributeTreeAsync(
                contentType: "application/json;charset=UTF-8",
                priority: "u=1, i",
                cookie: cookie,
                body: new AttributeTreeRequest
                {
                    CategoryId = categoryId,
                    Locale = "vi"
                },
                SPC_CDS: "ceccc5ce-6922-452a-865d-d6b2ed41e63a",
                SPC_CDS_VER: "2");

            if (!response.IsSuccessStatusCode || response?.Content?.Data?.List is null || response.Content.Data.List[0] is null ||
                response.Content.Data.List[0].AttributeTree is null)
            {
                continue;
            }
            var attributeResponses = response.Content.Data.List[0].AttributeTree;
            if (attributeResponses is null || attributeResponses.Count == 0 || (attributeResponses[0].AttributeId == 0 && attributeResponses[0].ValueId == 0))
            {
                continue;
            }

            foreach (var attributeResponse in attributeResponses)
            {
                InsertAttribute(attributeResponse, categoryId);
            }

        }
        await _context.SaveChangesAsync();
    }

    private void InsertAttribute(AttributeShoppe attributeShoppe, int? categoryId = null, int? originalAttributeParentId = null, int? parentId = null)
    {
        AttributeProduct attribute;

        if (_attributeDict.TryGetValue($"{attributeShoppe.Name}-{attributeShoppe.DisplayName}-{originalAttributeParentId}", out var dictValue))
        {
            attribute = dictValue;
        }
        else
        {
            attribute = new AttributeProduct(
                attributeShoppe.Name,
                attributeShoppe.DisplayName,
                parentId,
                attributeShoppe.Mandatory,
                attributeShoppe.ValueId);
            _context.AttributeProduct.Add(attribute);

            _attributeDict.Add($"{attributeShoppe.Name}-{attributeShoppe.DisplayName}-{originalAttributeParentId}", attribute);
        }

        if (categoryId.HasValue && _categoryDict.TryGetValue((int)categoryId, out var category))
        {
            attribute.AddCategory(category);
        }

        foreach (var childAttribute in attributeShoppe.Children)
        {
            InsertAttribute(childAttribute, null, originalAttributeParentId ?? attributeShoppe.AttributeId, attribute.Id);
        }
    }
}

