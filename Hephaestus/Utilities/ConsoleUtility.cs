using System;

namespace Hephaestus.Utilities
{
    public static class ConsoleUtility
    {
        public static bool AskYesNoQuestion(string question)
        {
            bool result;
            
            while (true)
            {
                Console.WriteLine($"{question} (Y/N)");
                ConsoleKey key = Console.ReadKey().Key;

                // Add some spacing because Console.ReadKey() does not terminate the line
                Console.WriteLine();

                if (key == ConsoleKey.Y)
                {
                    result = true;
                    break;
                }

                if (key == ConsoleKey.N)
                {
                    result = false;
                    break;
                }

                if (key != ConsoleKey.Y && key != ConsoleKey.N)
                {
                    Console.WriteLine("Incorrect selection. Try again.");
                }
            }

            return result;
        }

        public static string AskToEnterString(string message)
        {
            Console.WriteLine($"{message}: ");

            string enteredString = Console.ReadLine();

            return enteredString;
        }
    }
}