using System;
using System.IO;
using Hephaestus.Common.Utilities;
using Hephaestus.Utilities;
using Hephaestus.Common.Classes;
using Hephaestus.Classes;

namespace Hephaestus
{
    public static class Program
    {
        public static bool ForceBuild { private get; set; }
        
        public static void Main(string[] arguments)
        {
            string path = Environment.CurrentDirectory;

            // Handle any passed commands (arguments)
            ArgumentUtility.Handle(arguments);

            // Get the project data (if exists)
            Project project = ProjectUtility.GetProject(path);

            if (project == null)
            {
                Console.WriteLine("Project configuration file does not exist. Run 'hephaestus init' to create one.");
                
                Environment.Exit(1);
            }
            
            int exitCode = Builder.Build(project, ForceBuild);
    
            Environment.Exit(exitCode);
        }
    }
}
