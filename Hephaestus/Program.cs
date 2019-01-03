using System;
using Hephaestus.Classes;
using Hephaestus.Common.Classes;
using Hephaestus.Common.Utilities;
using Hephaestus.Utilities;

namespace Hephaestus
{
    public static class Program
    {
        public static bool ForceBuild { private get; set; }
        
        public static void Main(string[] arguments)
        {
            // Handle any passed commands (arguments).
            ArgumentUtility.Handle(arguments);

            // Get the project data (if exists).
            Project project = ProjectUtility.GetProject(Environment.CurrentDirectory);

            // If .hephaestus.json doesn't exist or can't be retrieved.
            if (project == null)
            {
                Console.WriteLine("Project configuration file does not exist. Run 'hephaestus init' to create one.");
                
                Environment.Exit(1);
            }
            
            // Build our data.
            int exitCode = Compiler.Build(project, ForceBuild);
    
            Environment.Exit(exitCode);
        }
    }
}
