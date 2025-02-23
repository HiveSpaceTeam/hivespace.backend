using AutoMapper;
using HiveSpace.Application.Models.Dtos.Request.User;
using HiveSpace.Application.Models.Dtos.Request.UserAddress;
using HiveSpace.Application.Models.ViewModels;
using HiveSpace.Domain.AggergateModels.ProductAggregate;
using HiveSpace.Domain.AggergateModels.UserAggregate;

namespace HiveSpace.Application.Mappers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<UserAddress, UserAddressDto>()
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Value));

        CreateMap<User, UserInfoDto>()
           .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Value));
        CreateMap<ProductCategory, ProductCategoryViewModel>();
        CreateMap<Product, ProductDetailViewModel>();
    }
}
