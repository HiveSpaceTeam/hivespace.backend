namespace HiveSpace.TestDataLoader;
public static class MenuHelper
{
    public static void DisplayMenu(List<string> options, string menuTitle = "Menu Options")
    {
        Console.WriteLine("\n======================================");
        Console.WriteLine($"{menuTitle}:");

        Console.WriteLine("0. Exit");
        foreach (var option in options )
        {
            Console.WriteLine(option);
        }
        Console.Write("Your choice: ");
    }

    public static int GetMenuChoice()
    {
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice)) { }
        return choice;
    }
}
