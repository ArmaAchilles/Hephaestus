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
        /// Prints a given message to the console and then prompts the user to enter a string.
        /// </summary>
        /// <param name="message">String to output to the console and act as a message to the user. The provided string is outputed in the following format: 'messageHere: '.</param>
        /// <returns>Returns the entered string</returns>
        public static string AskToEnterString(string message)
        {
            Console.Write($"{message}: ");
            return Console.ReadLine();
        }

        /// <summary>
        /// Prints a given message to the console and then prompts the user to enter a filesystem path corresponding to either a directory or a file.
        /// </summary>
        /// <param name="message">String to output to the console and act as a message to the user. The provided string is outputed in the following format: 'messageHere: '.</param>
        /// <param name="pathType">The type of path the user needs to enter. Either a directory or an individual file.</param>
        /// <returns>Path to the directory or file.</returns>
        public static string AskToEnterFileSystemPath(string message, PathType pathType)
        {
            string enteredPath;
            Console.Write($"{message}: ");

            while (true)
            {
                enteredPath = Console.ReadLine();
                if ((Directory.Exists(enteredPath) && pathType == PathType.Directory) || (File.Exists(enteredPath) && pathType == PathType.File))
                {
                    break;
                }

                string invalidPathMessage = pathType == PathType.Directory ?
                    $"{enteredPath} does not exist or does not represent a valid directory path. Please try again and enter a valid directory path." :
                    $"{enteredPath} does not exist or does not represent a valid file path. Please try again and enter a valid file path.";
                Console.WriteLine(invalidPathMessage);
            }
            return enteredPath;
        }

        /// <summary>
        /// Prints a given message to the console acting as a error message to the user.
        /// </summary>
        /// <param name="message">String to output to the console and act as a error message to the user. The provided string is outputed in the following format: 'error: messageHere'.</param>
        public static void PrintError(string message)
        {
            Console.Error.WriteLine($"error: {message}");
        }

        /// <summary>
        /// Prints a given message to the console acting as a warning message to the user.
        /// </summary>
        /// <param name="message">String to output to the console and act as a warning message to the user. The provided string is outputed in the following format: 'warning: messageHere'.</param>
        public static void PrintWarning(string message)
        {
            Console.WriteLine($"warning: {message}");
        }

        /// <summary>
        /// Prints a given message to the console as a informative message to the user.
        /// </summary>
        /// <param name="message">String to output to the console and act as a informative message to the user. The provided string is outputed in the following format: 'info: messageHere'.</param>
        public static void PrintInfo(string message)
        {
            Console.WriteLine($"info: {message}");
        }
    }
}