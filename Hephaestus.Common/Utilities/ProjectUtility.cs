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

            return File.Exists(jsonPath)
                ? JsonConvert.DeserializeObject<Project>(File.ReadAllText(jsonPath))
                : null;
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
