using System;
using System.IO;
using Hephaestus.Common.Classes;

namespace Hephaestus.Common.Utilities
{
    public static class ConsoleUtility
    {
        /// <summary>
        /// Loops till the user gives a Y or N answer which then is converted into a boolean.
        /// </summary>
        /// <param name="question">Question to ask the user. Question is transformed into 'questionHere (Y/N): '.</param>
        /// <returns>Returns true if user selected yes and returns false if user selected no.</returns>
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

        /// <summary>
        /// Ask the user to enter a string.
        /// </summary>
        /// <param name="message">Message displayed to the user. Message is transformed into 'messageHere: '.</param>
        /// <returns></returns>
        public static string AskToEnterString(string message)
        {
            Console.Write($"{message}: ");

            string enteredString = Console.ReadLine();

            return enteredString;
        }

        /// <summary>
        /// Ask the user to enter either a directory or a file path.
        /// </summary>
        /// <param name="message">Message displayed to the user. Message is transformed into 'messageHere: '.</param>
        /// <param name="pathType">Select what type of path should the user enter.</param>
        /// <returns>Path to the directory or file.</returns>
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

        /// <summary>
        /// Logs an error message to the console.
        /// </summary>
        /// <param name="message">Message to the user. Message is transformed into 'error: messageHere'.</param>
        public static void Error(string message)
        {
            Console.Error.WriteLine($"error: {message}");
        }

        /// <summary>
        /// Logs a warning to the console.
        /// </summary>
        /// <param name="message">Message to the user. Message is transformed into 'warning: messageHere'.</param>
        public static void Warning(string message)
        {
            Console.WriteLine($"warning: {message}");
        }

        /// <summary>
        /// Logs an info message to the console.
        /// </summary>
        /// <param name="message">Message to the user. Message is transformed into 'info: messageHere'.</param>
        public static void Info(string message)
        {
            Console.WriteLine($"info: {message}");
        }
    }
}