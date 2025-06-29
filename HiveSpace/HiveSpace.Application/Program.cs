using HiveSpace.Application.Extensions;
using HiveSpace.Application.Mappers;
using Azure.Identity;


var builder = WebApplication.CreateBuilder(args);
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

if (environment == "Production")
{
    
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        string endpoint = Environment.GetEnvironmentVariable("APP_CONFIGURATION_ENDPOINT")
            ?? throw new InvalidOperationException("The setting `AppConfigureEndpoint` was not found.");
        options.Connect(new Uri(endpoint), new DefaultAzureCredential());
        options.ConfigureKeyVault(kv =>
        {
            kv.SetCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions()));
        });
    });
}
var configuration = builder.Configuration;

// Add services to the container.
builder.Services
    .AddSetupOption(configuration)
    .AddApplicationServices(configuration)
    .AddInfrastructureServices(configuration)
    .AddAutoMapper(typeof(AutoMapperProfiles));
LoggingSetup.ConfigureLogging(builder.Environment, configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("_myAllowSpecificOrigins");
app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

app.Migrate();

app.Run();
