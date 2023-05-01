using System.Reflection;
using System.Text;

namespace Persistify.Helpers;

public static class PersistifyHelpers
{
    public static void WriteWelcomeMessage(string environmentName)
    {
        Console.WriteLine();
        WriteLine($"Welcome to Persistify v{Assembly.GetExecutingAssembly().GetName().Version}! 🚀\n", ConsoleColor.Magenta);

        // Select an emoji based on the environment
        var environmentEmoji = environmentName.ToLower() switch
        {
            "development" => "🔧",
            "staging" => "🔬",
            "production" => "🚀",
            _ => "❓",
        };

        var asciiArt = @"
        ____                           _            __     _     ____             
       / __ \  ___    _____   _____   (_)   _____  / /_   (_)   / __/   __  __    
      / /_/ / / _ \  / ___/  / ___/  / /   / ___/ / __/  / /   / /_    / / / /    
     / ____/ /  __/ / /     (__  )  / /   (__  ) / /_   / /   / __/   / /_/ /     
    /_/      \___/ /_/     /____/  /_/   /____/  \__/  /_/   /_/      \__, /      
                                                                     /____/       
";

        var lines = asciiArt.Split("\n");
        var maxLength = lines.Max(l => l.Length);

        var consoleColors = new[]
        {
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Green,
            ConsoleColor.Blue,
            ConsoleColor.Magenta,
            ConsoleColor.Cyan,
            ConsoleColor.White,
        };
        Write(new string('\n', lines.Length), ConsoleColor.Black);
        var initialCursorTop = Console.CursorTop - lines.Length;
        for (int i = 0; i < maxLength; i++)
        {
            Console.ForegroundColor = consoleColors[(i * 10) % consoleColors.Length];
            for (var j = 0; j < lines.Length; j++)
            {
                Console.SetCursorPosition(i + 4, initialCursorTop + j);
                if (i < lines[j].Length)
                {
                    Write(lines[j][i].ToString(), consoleColors[(i * 10) % consoleColors.Length], ConsoleColor.Black);
                }
                else
                {
                    Write(" ", ConsoleColor.Black, ConsoleColor.Black);
                }
            }

            Thread.Sleep(10);
        }
        Console.WriteLine();
        Console.WriteLine();
        WriteLine($"🖥️  Server is starting in {environmentEmoji} {environmentName} environment.", ConsoleColor.Magenta);
        WriteLine("🛑 Press Ctrl+C to exit.\n", ConsoleColor.Magenta);
    }

    private static void WriteLine(string message, ConsoleColor foregroundColor, ConsoleColor? backgroundColor = null)
    {
        Console.ForegroundColor = foregroundColor;
        if (backgroundColor.HasValue)
        {
            Console.BackgroundColor = backgroundColor.Value;
        }
        Console.WriteLine($"    {message}");
        Console.ResetColor();
    }

    private static void Write(string text, ConsoleColor color, ConsoleColor? backgroundColor = null)
    {
        Console.ForegroundColor = color;
        if (backgroundColor.HasValue)
        {
            Console.BackgroundColor = backgroundColor.Value;
        }
        Console.Write(text);
        Console.ResetColor();
    }
}
