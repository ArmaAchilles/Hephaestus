using HephaestusCommon.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hephaestus.Classes
{
    public class AddonBuilder
    {
        public Process Process { get; }

        public AddonBuilder(string sourceCodeDirectory, Project project)
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

                    Arguments = String.Format(@"""{0}"" ""{1}"" -prefix=""{2}"" -sign=""{3}"" -temp=""{4}\{5}"" -binarizeFullLogs",
                    sourceCodeDirectory,
                    project.TargetDirectory,
                    project.ProjectPrefix,
                    project.PrivateKeyFile,
                    Path.GetTempPath(),
                    project.ProjectPrefix)
                },

                EnableRaisingEvents = true
            };

            process.Start();

            Process = process;
        }
    }
}
