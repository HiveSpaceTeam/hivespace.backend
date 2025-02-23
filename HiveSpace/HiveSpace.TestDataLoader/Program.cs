using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using HiveSpace.Infrastructure;
using Refit;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization.ContractResolverExtentions;
using HiveSpace.TestDataLoader.Features.Models;
using HiveSpace.Application.Services;
using HiveSpace.Application.Interfaces;
using HiveSpace.Common.Interface;

namespace HiveSpace.TestDataLoader;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                config.SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{environment}.json", optional: true);
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;
                services.AddDbContext<NichoShopDbContext>(options =>
                    options.UseNpgsql(configuration.GetSection("Postgres:ConnectionString").Value));

                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

                services.AddScoped<IStorageService, AzureBlobStorageService>();
                services.AddScoped<IUserContext, UserContextFake>();

                services.AddRefitClient<IShoppeApi>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://banhang.shopee.vn/api/v3"))
                    .AddTypedClient(client =>
                    {
                        var refitSettings = new RefitSettings
                        {
                            ContentSerializer = new NewtonsoftJsonContentSerializer(
                                new JsonSerializerSettings
                                {
                                    ContractResolver = new SnakeCasePropertyNamesContractResolver()
                                })
                        };

                        return RestService.For<IShoppeApi>(client, refitSettings);
                    });
            })
            .Build();

        var mediator = host.Services.GetRequiredService<IMediator>();

        while (true)
        {
            List<string> menuOptions = MenuOption.GetAll<MenuOption>().Select(option => $"{option.Id}. {option.Name}").ToList();
            MenuHelper.DisplayMenu(menuOptions);

            int choice = MenuHelper.GetMenuChoice();

            if (choice == 0)
            {
                Console.WriteLine("Exiting the application. Goodbye!");
                break;
            }
            else
            {
                var selectedOption = MenuOption.GetAll<MenuOption>().FirstOrDefault(option => option.Id == choice);

                if (selectedOption == null)
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                    continue;
                }
                Console.WriteLine($"Executing: {selectedOption.Name}");
                try
                {
                    await mediator.Send(selectedOption.Command);
                    Console.WriteLine("Action completed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        
    }
}


