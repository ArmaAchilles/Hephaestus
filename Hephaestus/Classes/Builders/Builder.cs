using System;
using System.Diagnostics;
using System.IO;
using Hephaestus.Common.Classes;

namespace Hephaestus.Classes.Builders
{
    public class Builder : IBuilder
    {
        public Process Process { get; private set; }

        public void Build(string sourceCodeDirectory, Project project, ProcessStartInfo startInfo)
        {
            try
            {
                Process process = new Process
                {
                    StartInfo = startInfo,

                    EnableRaisingEvents = true
                };

                process.Start();
                
                process.BeginErrorReadLine();
                
                process.ErrorDataReceived += (sender, args) => OnErrorDataReceived(args, sourceCodeDirectory);

                Process = process;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Builder failed to launch for {Path.GetFileName(sourceCodeDirectory)} because {e.Message}");
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