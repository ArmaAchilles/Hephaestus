using Hephaestus.Common.Classes;
using System;
using System.Diagnostics;
using System.IO;

namespace Hephaestus.Classes.Builders
{
    public class AddonBuilder
    {
        public Process Process { get; }

        public AddonBuilder(string sourceCodeDirectory, Project project)
        {
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

                        FileName = project.AddonBuilderFile,

                        Arguments =
                            $@"""{sourceCodeDirectory}"" ""{project.TargetDirectory}"" " +
                                $@"-prefix=""{project.ProjectPrefix}\{Path.GetFileName(sourceCodeDirectory)}"" -sign=""{project.PrivateKeyFile}"" " +
                                $@"-temp=""{Path.GetTempPath()}\{project.ProjectPrefix}"" " +
                                $@"-include=""{AppDomain.CurrentDomain.BaseDirectory}\Hephaestus.AddonBuilderIncludes.txt"" -binarizeFullLogs"
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
                Console.Error.WriteLine($"Addon Builder failed to launch for {Path.GetFileName(sourceCodeDirectory)} folder because {e.Message}");
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
