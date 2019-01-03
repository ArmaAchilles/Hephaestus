using System;
using System.IO;
using Hephaestus.Common.Classes;
using Newtonsoft.Json;

namespace Hephaestus.Common.Utilities
{
    public static class ProjectUtility
    {
        private static string GetJsonPath(string path)
        {
            return $@"{path}\.hephaestus.json";
        }

        public static bool ProjectExists(string path)
        {
            string jsonPath = GetJsonPath(path);

            return File.Exists(jsonPath);
        }

        public static Project GetProject(string path)
        {
            string jsonPath = GetJsonPath(path);

            while (! File.Exists(jsonPath))
            {
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

            if (! File.Exists(jsonPath)) return null;
            
            Project project = JsonConvert.DeserializeObject<Project>(File.ReadAllText(jsonPath));

            if (project.ProjectDirectory != path)
            {
                project = null;
            }

            return project;
        }

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
