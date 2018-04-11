using System;

namespace AddonBuilder.Utility
{
    public class ConsoleUtil
    {
        /// <summary>
        /// Helper method that displays a given message in the console in red indicating an error.
        /// </summary>
        /// <param name="message">The message to display in the console</param>
        public static void ShowConsoleErrorMsg(string message)
        {
            Console.Clear();
            ConsoleColor previousConsoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{message}");
            Console.ForegroundColor = previousConsoleColor;
            Console.WriteLine("\nPress any key to continue.");
            Console.ReadKey();
        }
    }
}
