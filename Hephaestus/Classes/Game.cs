using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Hephaestus.Classes
{
    public static class Game
    {
        internal static void Launch(string gameExecutable, string gameExecutableArguments)
        {
            try
            {
                Console.WriteLine($"info: Starting {Path.GetFileName(gameExecutable)}");
                
                Process.Start(new ProcessStartInfo(gameExecutable, gameExecutableArguments));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"error: Failed to start {gameExecutable} because {e.Message}");
            }
        }

        public static void Shutdown(List<string> processesToShutdown)
        {
            Process[] allProcesses = Process.GetProcesses();

            foreach (Process process in allProcesses)
            {
                string processName = process.ProcessName.ToLower();

                if (! processesToShutdown.Contains(processName))
                {
                     continue;
                }
                
                Console.WriteLine($"info: Found {processName}. Terminating process...");

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
                    Console.Error.WriteLine($"error: Failed to shutdown {process} because {e.Message}");
                }
            }
        }
    }
}