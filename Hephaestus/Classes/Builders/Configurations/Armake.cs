using System;
using System.Diagnostics;
using System.IO;
using Hephaestus.Common.Classes;

namespace Hephaestus.Classes.Builders.Configurations
{
    public class Armake : Builder
    {
        public Armake(string sourceCodeDirectory, Project project)
        {
            string libraryPath = $@"{AppDomain.CurrentDomain.BaseDirectory}Libraries";
            
            string armakePath = Environment.Is64BitOperatingSystem
                ? $@"{libraryPath}\armake_w64.exe"
                : $@"{libraryPath}\armake_w32.exe";
            
            Build(sourceCodeDirectory, project, new ProcessStartInfo
            {
                RedirectStandardError = true,
                RedirectStandardOutput = false,

                UseShellExecute = false,

                CreateNoWindow = true,

                FileName = armakePath,
                        
                WorkingDirectory = Path.GetTempPath(),

                Arguments = $@"build -f -k ""{project.PrivateKeyFile}"" -w unquoted-string ""{sourceCodeDirectory}"" " + 
                            $@"""{project.TargetDirectory}\{Path.GetFileName(sourceCodeDirectory)}.pbo"""
            });
        }
    }
}