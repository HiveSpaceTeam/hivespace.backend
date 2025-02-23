using Refit;

namespace HiveSpace.TestDataLoader.Features.Models;
public interface IShoppeApi
{
    [Get("/category/get_category_tree")]
    Task<ApiResponse<ShoppeResponse<CategoryShoppe>>> GetCategoryTreeAsync(
        [Query] string SPC_CDS,
        [Query] string SPC_CDS_VER,
        [Query] string include_level,
        [Query] string version,
        [Header("locale")] string locale,
        [Header("priority")] string priority,
        [Header("sc-fe-session")] string session,
        [Header("sc-fe-ver")] string versionHeader,
        [Header("Cookie")] string cookie
    );

    [Post("/listing-upload/component/get_attribute_tree")]
    Task<ApiResponse<ShoppeResponse<AttributeTreeResponse>>> GetAttributeTreeAsync(
        [Header("content-type")] string contentType,
        [Header("priority")] string priority,
        [Header("Cookie")] string cookie,
        [Body] AttributeTreeRequest body,
        [Query] string SPC_CDS,
        [Query] string SPC_CDS_VER);

}
