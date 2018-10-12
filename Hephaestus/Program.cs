using System;
using System.IO;
using System.Reflection;
using HephaestusCommon.Utilities;
using Hephaestus.Utilities;
using HephaestusCommon.Classes;

namespace Hephaestus
{
    public class Program
    {
        public static void Main(string[] arguments)
        {
            string path = Environment.CurrentDirectory;

            Project project;

            // Handle any passed commands (arguments)
            ArgumentUtility.Handle(arguments);

            // Get the project data (if exists)
            if (ProjectUtility.ProjectExists(path))
            {
                project = ProjectUtility.GetProject(path);
            }
            else
            {
                project = null;

                Console.Error.WriteLine("Project configuration file does not exist. Run 'hephaestus init' to create one.");

                Environment.Exit(2);
            }

            int exitCode = BuilderUtility.Build(project);

            Environment.Exit(exitCode);
        }
    }
}
