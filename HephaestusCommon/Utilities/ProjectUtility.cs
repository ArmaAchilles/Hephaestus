using System;
using System.IO;
using HephaestusCommon.Classes;
using HephaestusCommon.Classes.Exceptions;
using Newtonsoft.Json;

namespace HephaestusCommon.Utilities
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

            Project project = File.Exists(jsonPath)
                ? JsonConvert.DeserializeObject<Project>(File.ReadAllText(jsonPath))
                : throw new ProjectDoesNotExistException();

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
                throw new ProjectFailedToSaveException(e.Message);
            }
        }
    }
}
