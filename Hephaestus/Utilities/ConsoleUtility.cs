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
                string key = Console.ReadLine()?.ToLower();
                
                if (key == "y")
                {
                    result = true;
                    break;
                }

                if (key == "n")
                {
                    result = false;
                    break;
                }

                Console.WriteLine("Incorrect selection. Try again.");
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