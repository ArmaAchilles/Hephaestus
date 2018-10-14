using System;

namespace Hephaestus.Utilities
{
    public static class ConsoleUtility
    {
        public static bool AskYesNoQuestion(string question)
        {
            bool result;
            
            Console.WriteLine($"{question} (Y/N)");
            ConsoleKey key = Console.ReadKey().Key;
            
            // Add some spacing because Console.ReadKey() does not terminate the line
            Console.WriteLine();

            switch (key)
            {
                case ConsoleKey.Y:
                    result = true;
                    break;
                
                case ConsoleKey.N:
                    result = false;
                    break;
                
                default:
                    Console.Error.WriteLine("Incorrect selection. Try again.");
                    result = AskAgain();
                    break;
            }

            return result;
        }

        public static string AskToEnterString(string message)
        {
            Console.WriteLine($"{message}:");

            string enteredString = Console.ReadLine();

            return enteredString;
        }

        private static bool AskAgain()
        {
            bool result;
            
            ConsoleKey key = Console.ReadKey().Key;
            
            // Add some spacing because Console.ReadKey() does not terminate the line
            Console.WriteLine();
            
            switch (key)
            {
                case ConsoleKey.Y:
                    result = true;
                    break;
                
                case ConsoleKey.N:
                    result = false;
                    break;
                
                default:
                    Console.Error.WriteLine("Incorrect selection. Try again");
                    result = AskAgain();
                    break;
            }

            return result;
        }
    }
}