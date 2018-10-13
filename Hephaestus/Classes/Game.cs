using System;
using System.Collections.Generic;
using System.Diagnostics;
using Hephaestus.Classes.Exceptions;

namespace Hephaestus.Classes
{
    public static class Game
    {
        internal static void Launch(string gameExecutable, string gameExecutableArguments)
        {
            try
            {
                Process.Start(new ProcessStartInfo(gameExecutable, gameExecutableArguments));
            }
            catch (Exception e)
            {
                throw new GameFailedToStartException(e.Message);
            }
        }

        public static void Shutdown(List<string> processesToShutdown)
        {
            Process[] allProcesses = Process.GetProcesses();

            foreach (Process process in allProcesses)
            {
                string processName = process.ProcessName.ToLower();

                if (! processesToShutdown.Contains(processName)) continue;
                
                Console.WriteLine($"Found {processName}. Terminating process...");

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
                    throw new GameFailedToShutdownException(e.Message);
                }
            }
        }
    }
}