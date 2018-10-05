using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using HephaestusCommon.Utilities;

namespace HephaestusCommon.Classes.Settings
{
    public class MasterSetting
    {
        public MasterSetting(List<string> projectDirectories)
        {
            ProjectDirectories = projectDirectories;
        }

        public List<string> ProjectDirectories;

        public static MasterSetting GetSetting()
        {
            MasterSetting masterSetting = null;

            string jsonPath = JsonUtility.GetJsonPath(JsonUtility.EJsonType.Master);

            if (jsonPath != null)
            {
                masterSetting = JsonConvert.DeserializeObject<MasterSetting>(File.ReadAllText(jsonPath));
            }

            return masterSetting;
        }

        public void SetSetting(MasterSetting masterSetting)
        {
            string json = JsonConvert.SerializeObject(masterSetting, Formatting.Indented);

            JsonUtility.WriteJsonFile(JsonUtility.EJsonType.Master, json);
        }
    }
}
