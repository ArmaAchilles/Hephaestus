using System;
using System.Diagnostics;
using System.IO;
using Hephaestus.Common.Utilities;

namespace Hephaestus.Common.Classes
{
    public class Game
    {
        public string GameExecutable { get; }
        public string GameExecutableArguments { get; }

        public Game(string gameExecutable, string gameExecutableArguments)
        {
            GameExecutable = gameExecutable;
            GameExecutableArguments = gameExecutableArguments;
        }

        /// <summary>
        /// Launch the game.
        /// </summary>
        /// <param name="game">Game data.</param>
        public static void Launch(Game game)
        {
            try
            {
                ConsoleUtility.Info($"Starting {Path.GetFileName(game.GameExecutable)}");
                
                Process.Start(new ProcessStartInfo(game.GameExecutable, game.GameExecutableArguments));
            }
            catch (Exception e)
            {
                ConsoleUtility.Error($"Failed to start {game.GameExecutable} because {e.Message}");
            }
        }

        /// <summary>
        /// Close the process.
        /// </summary>
        /// <param name="processToShutdown">Process to terminate.</param>
        public static void Shutdown(string processToShutdown)
        {
            // Get all processes running.
            Process[] allProcesses = Process.GetProcesses();

            // Loop through each process.
            foreach (Process process in allProcesses)
            {
                // Turn the process name into lowercase for any inconsistencies between naming.
                string processName = process.ProcessName.ToLower();

                // If the process name doesn't match process to shutdown then we move on onto the next item in the loop.
                if (processToShutdown != processName)
                {
                    continue;
                }
                
                ConsoleUtility.Info($"Found {processName}. Terminating process...");

                try
                {
                    // Shutdown the process.
                    if (! process.HasExited)
                    {
                        process.CloseMainWindow();

                        process.WaitForExit();
                    }
                    
                    process.Close();
                }
                catch (Exception e)
                {
                    ConsoleUtility.Error($"error: Failed to shutdown {process} because {e.Message}");
                }
            }
        }
    }
}