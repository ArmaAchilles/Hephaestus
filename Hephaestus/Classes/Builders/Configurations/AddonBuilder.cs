using System;
using System.Diagnostics;
using System.IO;
using Hephaestus.Common.Classes;

namespace Hephaestus.Classes.Builders.Configurations
{
    public class AddonBuilder : Builder
    {
        public AddonBuilder(string sourceCodeDirectory, Project project)
        {
            Build(sourceCodeDirectory, project, new ProcessStartInfo
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
            });
        }
    }
}
