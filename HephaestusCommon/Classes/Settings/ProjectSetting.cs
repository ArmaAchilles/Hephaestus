using HephaestusCommon.Utilities;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace HephaestusCommon.Classes.Settings
{
    public class ProjectSetting
    {
        public string ProjectDirectory { get; set; }

        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }

        public string AddonBuilderFile { get; set; }

        public string ProjectPrefix { get; set; }

        public string PrivateKeyFile { get; set; }

        public string GameExecutable { get; set; }
        public string GameExecutableArguments { get; set; }

        public bool ShutdownGameBeforeBuilding { get; set; }
        public bool StartGameAfterBuilding { get; set; }

        // Folder name, checksum
        Dictionary<string, string> Checksums { get; set; }

        public ProjectSetting GetSetting()
        {
            ProjectSetting projectSetting = null;

            MasterSetting masterSetting = MasterSetting.GetSetting();

            string projectDirectory = "";
            foreach (string directory in masterSetting.ProjectDirectories)
            {
                if (directory == ProjectDirectory)
                {
                    projectDirectory = directory;
                }
            }

            string jsonPath = JsonUtility.GetJsonPath(JsonUtility.EJsonType.Project, projectDirectory);

            if (jsonPath != null)
            {
                projectSetting = JsonConvert.DeserializeObject<ProjectSetting>(File.ReadAllText(jsonPath));
            }

            return projectSetting;
        }

        public void SetSetting(ProjectSetting projectSetting)
        {
            string json = JsonConvert.SerializeObject(projectSetting, Formatting.Indented);

            JsonUtility.WriteJsonFile(JsonUtility.EJsonType.Project, json, projectSetting.ProjectDirectory);
        }
    }
}
