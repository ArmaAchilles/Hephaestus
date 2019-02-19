using System;
using System.IO;
using Hephaestus.Common.Classes;
using Newtonsoft.Json;

namespace Hephaestus.Common.Utilities
{
    public static class ProjectUtility
    {
        /// <summary>
        /// Gets the .hephaestus.json path. Does not mean that the file exists.
        /// </summary>
        /// <param name="path">Path where to assign the .hephaestus.json to.</param>
        /// <returns>.hephaestus.json path.</returns>
        private static string GetJsonPath(string path)
        {
            return $@"{path}\.hephaestus.json";
        }

        /// <summary>
        /// Checks if .hephaestus.json file exists.
        /// </summary>
        /// <param name="path">Path were to look for .hephaestus.json</param>
        /// <returns>Does .hephaestus.json exist?</returns>
        public static bool ProjectExists(string path)
        {
            string jsonPath = GetJsonPath(path);

            return File.Exists(jsonPath);
        }

        /// <summary>
        /// Loop recursively till hits root of the drive and try to find .hephaestus.json and once found returns an instance of the Project class.
        /// If not found then returns null.
        /// </summary>
        /// <param name="path">Directory path where to look for .hephaestus.json.</param>
        /// <returns>Returns an instance of Project class if .hephaestus.json was found and if not found then returns null.</returns>
        public static Project GetProject(string path)
        {
            string jsonPath = GetJsonPath(path);

            while (! File.Exists(jsonPath))
            {
                // Climb back till .hephaestus.json is found.
                if (Directory.GetParent(path) != null)
                {
                    path = Directory.GetParent(path).ToString();
                    jsonPath = Path.Combine(path, ".hephaestus.json");
                }
                else
                {
                    break;
                }
            }

            // If nothing was found then return null.
            if (! File.Exists(jsonPath)) return null;
            
            // Read the JSON.
            Project project = JsonConvert.DeserializeObject<Project>(File.ReadAllText(jsonPath));

            // If we found a .hephaestus.json that references a different project in the parent directory.
            if (project.ProjectDirectory != path)
            {
                project = null;
            }

            return project;
        }

        /// <summary>
        /// Converts an instance of Project to JSON and saves to disk.
        /// </summary>
        /// <param name="path">Directory where to save the JSON.</param>
        /// <param name="project">Project data.</param>
        public static void SetProject(string path, Project project)
        {
            string jsonPath = GetJsonPath(path);
            
            string json = JsonConvert.SerializeObject(project, Formatting.Indented);

            try
            {
                File.WriteAllText(jsonPath, json);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Failed to save project because {e.Message}");
            }
        }
    }
}
