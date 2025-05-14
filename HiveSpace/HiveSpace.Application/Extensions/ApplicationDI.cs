using FluentValidation;
using HiveSpace.Common.Filters;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.AppSettings;
using HiveSpace.Application.Models.Dtos.Request.CartItem;
using HiveSpace.Application.Models.Dtos.Request.Paging;
using HiveSpace.Application.Models.Dtos.Request.Product;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;
using HiveSpace.Application.Models.Dtos.Request.User;
using HiveSpace.Application.Models.Dtos.Request.UserAddress;
using HiveSpace.Application.Queries;
using HiveSpace.Application.Services;
using HiveSpace.Application.Validators.Order;
using HiveSpace.Application.Validators.Paging;
using HiveSpace.Application.Validators.Product;
using HiveSpace.Application.Validators.ShoppingCart;
using HiveSpace.Application.Validators.User;
using HiveSpace.Application.Validators.UserAddress;
using HiveSpace.Common.Interface;
using HiveSpace.Common.Service;
using HiveSpace.Commons.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using StackExchange.Redis;
using System.Text;

namespace HiveSpace.Application.Extensions;

public static class ApplicationDI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<CustomExceptionFilter>();
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.ConfigureSwagger();
        //services.ConfigureLogging();
        services.ConfigureApplicationService();
        services.ConfigureFluentValidation();
        services.ConfigureCustomService();
        services.ConfigureAuthencation(configuration);
        services.ConfigureQuery();
        services.ConfigureCors(configuration);
        services.ConfigureCache(configuration);
        return services;
    }

    private static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
    private static IServiceCollection ConfigureLogging(this IServiceCollection services)
    {
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddNLog();
        });
        return services;
    }

    private static IServiceCollection ConfigureFluentValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateUserRequestDto>, CreateUserValidator>();
        services.AddScoped<IValidator<LoginRequestDto>, LoginValidator>();
        services.AddScoped<IValidator<UserAddressRequestDto>, UserAddressValidator>();
        services.AddScoped<IValidator<ChangePasswordRequestDto>, ChangePasswordValidator>();
        services.AddScoped<IValidator<UpdateUserRequestDto>, UpdateUserValidator>();
        services.AddScoped<IValidator<ProductSearchRequestDto>, ProductSearchValidator>();
        services.AddScoped<IValidator<CreateOrderRequestDto>, CreateOrderValidator>();
        services.AddScoped<IValidator<AddItemToCartRequestDto>, AddItemToCartValidator>();
        services.AddScoped<IValidator<UpdateCartItemRequestDto>, UpdateCartItemValidator>();
        services.AddScoped<IValidator<UpdateMultiCartItemSelectionDto>, UpdateMultiCartItemSelectionValidator>();
        services.AddScoped<IValidator<PagingRequestDto>, PagingValidator>();
        return services;
    }

    private static IServiceCollection ConfigureApplicationService(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserAddressService, UserAddressService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IStorageService, AzureBlobStorageService>();
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<ISkuService, SkuSerive>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<ITransactionService, TransactionService>();
        return services;
    }

    private static IServiceCollection ConfigureCustomService(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IUserContext, UserContext>();
        return services;
    }

    public static IServiceCollection ConfigureAuthencation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var jwtOption = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOption!.Issuer,
                    ValidAudience = jwtOption!.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOption.SecretKey))
                };
            });
        return services;
    }

    public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var appConfig = configuration.GetSection("AppConfig").Get<AppConfig>();

        if (appConfig?.Cors != null && appConfig.Cors.Length > 0)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("_myAllowSpecificOrigins", builder =>
                {
                    builder.WithOrigins(appConfig.Cors)
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });
        }
        return services;
    }

    public static IServiceCollection ConfigureQuery(this IServiceCollection services)
    {
        services.AddScoped<IQueryService, QueryService>();
        return services;
    }

    public static IServiceCollection ConfigureCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisOption = configuration.GetSection("Redis").Get<RedisOption>();
        if (redisOption is null) return services;

        services.AddMemoryCache();
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisOption.ConnectionString));
        services.AddSingleton<IDatabase>(sp =>
            sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
        return services;
    }
}
