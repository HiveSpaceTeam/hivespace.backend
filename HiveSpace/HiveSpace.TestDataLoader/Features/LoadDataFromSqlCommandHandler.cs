using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using HiveSpace.Domain.SeedWork;
using HiveSpace.Infrastructure;

namespace HiveSpace.TestDataLoader.Features;

public record LoadDataFromSqlCommand : IRequest
{
}

public class LoadDataFromSqlCommandHandler(NichoShopDbContext context) : IRequestHandler<LoadDataFromSqlCommand>
{
    private readonly NichoShopDbContext _context = context;
    public async Task Handle(LoadDataFromSqlCommand command, CancellationToken cancellationToken)
    {

        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        
        string sqlFolderPath = Path.Combine(basePath, "Sql");

        List<string> files = Directory.GetFiles(sqlFolderPath)
            .Select(file => Path.GetFileName(file) ?? string.Empty)
            .Where(file => !string.IsNullOrEmpty(file))
            .ToList(); // Get all files
        files.Add("All");

        var fileMenu = files.Select((file, index) => $"{index+1}. {file}").ToList();

        while (true)
        {
            MenuHelper.DisplayMenu(fileMenu, "Choose file to load data"); // Display all files

            int choice = MenuHelper.GetMenuChoice();
            if (choice == 0)
            {
                break;
            }
            else
            {
                var fileName = files.ElementAtOrDefault(choice - 1);

                if (fileName == null)
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                    continue;
                }
                if (fileName == "All")
                {
                    foreach (var file in files.Take(files.Count - 1))
                    {
                        await ExecuteSql(basePath, file, cancellationToken);
                    }
                }
                else
                {
                    await ExecuteSql(basePath, fileName, cancellationToken);
                }

                
            }
        }
    }

    private async Task ExecuteSql(string basePath, string fileName, CancellationToken cancellationToken)
    {
        string sqlFilePath = Path.Combine(basePath, "Sql", fileName);

        if (!File.Exists(sqlFilePath))
        {
            Console.WriteLine("SQL file not found.");
        }

        try
        {
            // Read the SQL file
            string sql = File.ReadAllText(sqlFilePath);

            // Execute the SQL
            await _context.Database.ExecuteSqlRawAsync(sql, cancellationToken: cancellationToken);

            Console.WriteLine($"SQL script in {fileName} executed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while executing the SQL script: {ex.Message}");
        }
    }
}
