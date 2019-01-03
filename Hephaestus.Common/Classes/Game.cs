using System;
using System.Diagnostics;
using System.IO;
using Hephaestus.Common.Utilities;

namespace Hephaestus.Common.Classes
{
    public class Game
    {
        public string GameExecutable { get; set; }
        public string GameExecutableArguments { get; set; }

        public Game(string gameExecutable, string gameExecutableArguments)
        {
            GameExecutable = gameExecutable;
            GameExecutableArguments = gameExecutableArguments;
        }

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

        public static void Shutdown(string processToShutdown)
        {
            Process[] allProcesses = Process.GetProcesses();

            foreach (Process process in allProcesses)
            {
                string processName = process.ProcessName.ToLower();

                if (processToShutdown == processName)
                {
                    continue;
                }
                
                ConsoleUtility.Info($"Found {processName}. Terminating process...");

                try
                {
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