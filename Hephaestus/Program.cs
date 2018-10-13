using System;
using HephaestusCommon.Utilities;
using Hephaestus.Utilities;
using HephaestusCommon.Classes;
using Hephaestus.Classes;
using HephaestusCommon.Classes.Exceptions;

namespace Hephaestus
{
    public static class Program
    {
        public static void Main(string[] arguments)
        {
            string path = Environment.CurrentDirectory;

            // Handle any passed commands (arguments)
            ArgumentUtility.Handle(arguments);

            // Get the project data (if exists)
            Project project;
            try
            {
                project = ProjectUtility.GetProject(path);
            }
            catch (ProjectDoesNotExistException)
            {
                throw new ProjectDoesNotExistException("Project configuration file does not exist. Run 'hephaestus init' to create one.");
            }

            int exitCode = Builder.Build(project);

            Environment.Exit(exitCode);
        }
    }
}
