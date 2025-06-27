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
using Azure.Storage.Blobs;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.Extensions.DependencyInjection;

namespace HiveSpace.Application.Extensions;


public static class ApplicationDI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddControllers(options =>
        {
            options.Filters.Add<CustomExceptionFilter>();
        });
        services.ConfigureSwagger();
        //services.ConfigureLogging();
        services.ConfigureApplicationService();
        services.ConfigureFluentValidation();
        services.ConfigureCustomService();
        services.ConfigureAuthentication(configuration);
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
        // In Program.cs or Startup.cs
        services.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetSection("AzureBlobStorage:Connectionstring").Value;
            return new BlobServiceClient(connectionString);
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

    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        const string MonolithJwtScheme = "MonolithJwt";
        const string IdentityServerJwtScheme = "IdentityServerJwt";
        const string HybridJwtScheme = "HybridJwt";
        var serviceProvider = services.BuildServiceProvider();
        var jwtSetting = serviceProvider.GetRequiredService<IOptions<JwtSetting>>().Value; 
        var identityServerJwtSetting = serviceProvider.GetRequiredService<IOptions<IdentityServerJwtSetting>>().Value; ;
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = HybridJwtScheme;
            options.DefaultChallengeScheme = HybridJwtScheme;
        })
        .AddJwtBearer(MonolithJwtScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSetting!.Issuer,
                ValidAudience = jwtSetting.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSetting.SecretKey))
            };
        })
        .AddJwtBearer(IdentityServerJwtScheme, options =>
        {
            
            options.Authority = identityServerJwtSetting!.Authority;
            options.Audience = identityServerJwtSetting.Audience;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = identityServerJwtSetting.Authority,
                ValidAudience = identityServerJwtSetting.Audience,
                ValidateIssuerSigningKey = false,
                ClockSkew = TimeSpan.FromMinutes(5),
                SignatureValidator = (token, _) => new JsonWebToken(token)
            };
            //options.Events = new JwtBearerEvents
            //{
            //    OnAuthenticationFailed = context =>
            //    {
            //        Console.WriteLine($"Authentication failed for {IdentityServerJwtScheme}: {context.Exception}");
            //        return Task.CompletedTask;
            //    },
            //    OnTokenValidated = context =>
            //    {
            //        Console.WriteLine($"Token successfully validated for {IdentityServerJwtScheme}. User: {context.Principal?.Identity?.Name}");
            //        return Task.CompletedTask;
            //    }
            //};
        })
        .AddPolicyScheme(HybridJwtScheme, "JWT from Monolith or Identity Server", options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                string? authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                {
                    return MonolithJwtScheme;
                }
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                var jwtHandler = new JwtSecurityTokenHandler();
                if (!jwtHandler.CanReadToken(token))
                {
                    return MonolithJwtScheme;
                }
                var jwtToken = jwtHandler.ReadJwtToken(token);
                var issuer = jwtToken.Claims.FirstOrDefault(c => c.Type == "iss")?.Value;
                var identityAuthority = identityServerJwtSetting.Authority;
                if (issuer == identityAuthority ||  issuer == (identityAuthority?.TrimEnd('/') + "/"))
                {
                    return IdentityServerJwtScheme;
                }
                return MonolithJwtScheme;
            };
        });

        // Add authorization services
        services.AddAuthorization();

        return services;
    }
}
