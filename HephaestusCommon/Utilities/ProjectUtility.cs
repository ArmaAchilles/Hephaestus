using System;
using System.IO;
using HephaestusCommon.Classes;
using Newtonsoft.Json;

namespace HephaestusCommon.Utilities
{
    public class ProjectUtility
    {
        private string GetJsonPath(string path)
        {
            return String.Format("{0}\\.hephaestus.json", path);
        }

        public Project GetProject(string path)
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

        public void SetProject(string path, Project project)
        {
            string jsonPath = GetJsonPath(path);

            string json = JsonConvert.SerializeObject(project, Formatting.Indented);

            File.WriteAllText(jsonPath, json);
        }
    }
}
