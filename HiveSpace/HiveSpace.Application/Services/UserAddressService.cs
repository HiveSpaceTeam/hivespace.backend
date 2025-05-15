using AutoMapper;
using HiveSpace.Application.Helpers;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.UserAddress;
using HiveSpace.Common.Interface;
using HiveSpace.Domain.AggergateModels.UserAggregate;
using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Exceptions;
using HiveSpace.Domain.Repositories;

namespace HiveSpace.Application.Services;

public class UserAddressService : IUserAddressService
{
    private readonly IUserContext _userContext;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _redisService;
    public UserAddressService(IUserRepository userRepository, IUserContext userContext, IMapper mapper, ICacheService redisService)
    {
        _userRepository = userRepository;
        _userContext = userContext;
        _mapper = mapper;
        _redisService = redisService;
    }

    private string CacheKey(Guid userId) => $"userAddress_{userId}";

    public async Task<List<UserAddressDto>> GetUserAddressAsync()
    {
        var result = await _redisService.GetOrCreateAsync(CacheKey(_userContext.UserId), async () =>
        {
            var user = await _userRepository.GetByIdAsync(_userContext.UserId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);
            var userAddresses = user.Addresses.ToList();
            var res = _mapper.Map<List<UserAddressDto>>(userAddresses);
            return res;
        });
        return result;
    }

    public async Task<Guid> CreateUserAddressAsync(UserAddressRequestDto param)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);
        var userAddressProp = new UserAddressProps()
        {
            FullName = param.FullName,
            Street = param.Street,
            Ward = param.Ward,
            District = param.District,
            Province = param.Province,
            Country = param.Country,
            ZipCode = param.ZipCode,
            PhoneNumber = param.PhoneNumber,
        };
        var address = user.AddAddress(userAddressProp);
        await _userRepository.SaveChangesAsync();
        await _redisService.RemoveAsync(CacheKey(_userContext.UserId));
        return address.Id;
    }

    public async Task<bool> UpdateUserAddressAsync(UserAddressRequestDto param, Guid userAddressId)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);
        
        var userAddress = user.Addresses.FirstOrDefault(x => x.Id == userAddressId)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserAddressNotFound);

        var userAddressProp = new UserAddressProps()
        {
            FullName = param.FullName,
            Street = param.Street,
            Ward = param.Ward,
            District = param.District,
            Province = param.Province,
            Country = param.Country,
            ZipCode = param.ZipCode,
            PhoneNumber = param.PhoneNumber,
        };
        user.UpdateAddress(userAddressId, userAddressProp);
        await _userRepository.SaveChangesAsync();
        await _redisService.RemoveAsync(CacheKey(_userContext.UserId));
        return true;
    }

    public async Task<bool> SetDefaultUserAddressAsync(Guid userAddressId)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);
        
        var userAddress = user.Addresses.FirstOrDefault(x => x.Id == userAddressId)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserAddressNotFound);

        user.SetDefaultAddress(userAddressId);
        await _userRepository.SaveChangesAsync();
        await _redisService.RemoveAsync(CacheKey(_userContext.UserId));
        return true;
    }

    public async Task<bool> DeleteUserAddressAsync(Guid userAddressId)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);
        
        var userAddress = user.Addresses.FirstOrDefault(x => x.Id == userAddressId)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserAddressNotFound);

        user.RemoveAddress(userAddressId);
        await _userRepository.SaveChangesAsync();
        await _redisService.RemoveAsync(CacheKey(_userContext.UserId));
        return true;
    }

    public async Task<UserAddress> GetByIdAsync(Guid userAddressId)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);
        return user.Addresses.FirstOrDefault(x => x.Id == userAddressId) ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserAddressNotFound);
    }
}
