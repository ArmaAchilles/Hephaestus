using System;
using System.IO;
using Hephaestus.Common.Classes;

namespace Hephaestus.Common.Utilities
{
    /// <summary>
    /// Provides a set of faculties to handle various console related tasks.
    /// </summary>
    public static class ConsoleUtility
    {
        /// <summary>
        /// Prints a given message to the console which acts as a question to the user from which they respond with either Y for yes or N for no.
        /// </summary>
        /// <param name="question">String to output to the console and act as a question to the user. The provided string is outputed in the following format: 'questionHere (Y/N): '.</param>
        /// <returns>Returns true if the user entered Y for yes or false if the user entered N for no.</returns>
        public static bool AskYesNoQuestion(string question)
        {
            string response;
            Console.Write($"{question} (Y/N): ");

            while (true)
            {
                response = Console.ReadLine()?.ToLower();

                if (response != "y" || response != "yes" || response != "n" || response != "no")
                {
                    Console.WriteLine($"{response} is not a valid anwser to the given question: {question}. Please enter either Y for yes or N for no.");
                    continue;
                }
                break;
            }
            return response == "y" || response == "yes" ? true : false;
        }

        /// <summary>
        /// Prints a given message to the console and prompts the user to enter a string.
        /// </summary>
        /// <param name="message">String to output to the console and act as a message to the user. The provided string is outputed in the following format: 'messageHere: '.</param>
        /// <returns>Returns the entered string</returns>
        public static string AskToEnterString(string message)
        {
            //TODO: prevent the user from entering an empty string
            Console.Write($"{message}: ");
            return Console.ReadLine();
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