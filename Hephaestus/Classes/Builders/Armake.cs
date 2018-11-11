using System;
using System.Diagnostics;
using System.IO;
using Hephaestus.Common.Classes;

namespace Hephaestus.Classes.Builders
{
    public class Armake
    {
        public Process Process { get; }

        public Armake(string sourceCodeDirectory, Project project)
        {
            string libraryPath = $@"{AppDomain.CurrentDomain.BaseDirectory}Libraries";
            
            string armakePath = Environment.Is64BitOperatingSystem
                ? $@"{libraryPath}\armake_w64.exe"
                : $@"{libraryPath}\armake_w32.exe";
            
            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        RedirectStandardError = true,
                        RedirectStandardOutput = false,

                        UseShellExecute = false,

                        CreateNoWindow = true,

                        FileName = armakePath,
                        
                        WorkingDirectory = Path.GetTempPath(),

                        Arguments = $@"build -f -k ""{project.PrivateKeyFile}"" -w unquoted-string ""{sourceCodeDirectory}"" " + 
                                    $@"""{project.TargetDirectory}\{Path.GetFileName(sourceCodeDirectory)}.pbo"""
                    },

                    EnableRaisingEvents = true
                };

                process.Start();
                
                process.BeginErrorReadLine();
                
                process.ErrorDataReceived += (sender, args) => OnErrorDataReceived(args, sourceCodeDirectory);

                Process = process;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Armake failed to launch for {Path.GetFileName(sourceCodeDirectory)} folder because {e.Message}");
            }
        }

        private static void OnErrorDataReceived(DataReceivedEventArgs e, string sourceCodeDirectory)
        {
            if (! string.IsNullOrEmpty(e.Data))
            {
                Console.Error.WriteLine($"{Path.GetFileName(sourceCodeDirectory)} | {e.Data}");
            }
        }
    }
}