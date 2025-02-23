using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using HiveSpace.Application.Extensions;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Services;
using HiveSpace.Domain.Enums;

namespace HiveSpace.TestDataLoader.Features;

public record CopyBlobStorageCommand : IRequest
{
    public bool IsCopyFromDevToProd { get; set; }
    public StorageType StorageType { get; set; }
}

public class CopyBlobStorageCommandHandler : IRequestHandler<CopyBlobStorageCommand>
{
    private readonly IStorageService _devStorageService;
    private readonly IStorageService _prodStorageService;

    public CopyBlobStorageCommandHandler()
    {
        var devConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json")
            .Build();

        // Load production configuration
        var prodConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Production.json")
            .Build();

        // Create instances of AzureBlobStorageService
        _devStorageService = new AzureBlobStorageService(devConfiguration);
        _prodStorageService = new AzureBlobStorageService(prodConfiguration);
    }

    public async Task Handle(CopyBlobStorageCommand request, CancellationToken cancellationToken)
    {
        var prodContainerClient = await _prodStorageService.GetContainerClient(request.StorageType.GetDisplayName());
        var devContainerClient = await _devStorageService.GetContainerClient(request.StorageType.GetDisplayName());

        if (request.IsCopyFromDevToProd)
        {
            await CopyBlobStorage(prodContainerClient, devContainerClient, cancellationToken);
        }
        else
        {
            await CopyBlobStorage(devContainerClient, prodContainerClient, cancellationToken);
        }
    }

    private static async Task CopyBlobStorage(BlobContainerClient destContainerClient, BlobContainerClient srcContainerClient, CancellationToken cancellationToken)
    {
        await foreach (BlobItem blobItem in destContainerClient.GetBlobsAsync(cancellationToken: cancellationToken))
        {
            var blobClient = destContainerClient.GetBlobClient(blobItem.Name);
            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        await foreach (BlobItem blobItem in srcContainerClient.GetBlobsAsync(cancellationToken: cancellationToken))
        {
            var destBlobClient = destContainerClient.GetBlobClient(blobItem.Name);
            var srcBlobClient = srcContainerClient.GetBlobClient(blobItem.Name);

            using var stream = new MemoryStream();
            await srcBlobClient.DownloadToAsync(stream, cancellationToken);
            stream.Position = 0;

            await destBlobClient.UploadAsync(stream, overwrite: true, cancellationToken: cancellationToken);
        }
    }
}
