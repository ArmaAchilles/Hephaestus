using System;
using System.IO;
using Hephaestus.Common.Classes;

namespace Hephaestus.Common.Utilities
{
    public static class ConsoleUtility
    {
        public static bool AskYesNoQuestion(string question)
        {
            bool result;
            
            while (true)
            {
                Console.Write($"{question} (Y/N): ");
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
            Console.Write($"{message}: ");

            string enteredString = Console.ReadLine();

            return enteredString;
        }

        public static string AskToEnterPath(string message, PathType pathType)
        {
            string enteredString;
            while (true)
            {
                Console.Write($"{message}: ");
                
                enteredString = Console.ReadLine();

                if (pathType == PathType.Directory)
                {
                    if (Directory.Exists(enteredString))
                    {
                        break;
                    }
    
                    Console.WriteLine("That is not a valid directory. Try again.");
                }
                else
                {
                    if (File.Exists(enteredString))
                    {
                        break;
                    }

                    Console.WriteLine("That is not a valid file. Try again.");
                }
            }

            return enteredString;
        }

        public static void Error(string message)
        {
            Console.Error.WriteLine($"error: {message}");
        }

        public static void Warning(string message)
        {
            Console.WriteLine($"warning: {message}");
        }

        public static void Info(string message)
        {
            Console.WriteLine($"info: {message}");
        }
    }
}