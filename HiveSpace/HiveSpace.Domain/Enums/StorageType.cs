using System.ComponentModel.DataAnnotations;

namespace HiveSpace.Domain.Enums;
public enum StorageType
{
    [Display(Name = "productimage")]
    ProductImages = 1,

    [Display(Name = "avatar")]
    Avatar = 2,

    [Display(Name = "categoryimage")]
    CategoryImages = 3,
}
