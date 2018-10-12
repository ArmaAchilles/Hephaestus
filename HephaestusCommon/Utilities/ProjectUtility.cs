using System;
using System.IO;
using HephaestusCommon.Classes;
using Newtonsoft.Json;

namespace HephaestusCommon.Utilities
{
    public class ProjectUtility
    {
        private static string GetJsonPath(string path)
        {
            return String.Format(@"{0}\.hephaestus.json", path);
        }

        public static bool ProjectExists(string path)
        {
            string jsonPath = GetJsonPath(path);

            if (File.Exists(jsonPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Project GetProject(string path)
        {
            Project project;

            string jsonPath = GetJsonPath(path);

            if (File.Exists(jsonPath))
            {
                project = JsonConvert.DeserializeObject<Project>(File.ReadAllText(jsonPath));
            }
            else
            {
                project = null;
            }

            return project;
        }

        public static void SetProject(string path, Project project)
        {
            string jsonPath = GetJsonPath(path);

            string json = JsonConvert.SerializeObject(project, Formatting.Indented);

            File.WriteAllText(jsonPath, json);
        }
    }
}
