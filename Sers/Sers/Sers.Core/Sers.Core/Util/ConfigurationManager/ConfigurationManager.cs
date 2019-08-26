using System;
using System.IO;

namespace Sers.Core.Util.ConfigurationManager
{
    public class ConfigurationManager: JsonFile
    {
        #region Instance        
        private static ConfigurationManager instance;
        public static ConfigurationManager Instance
        {
            get { return instance ?? (instance = new ConfigurationManager()); }
            set { instance = value; }
        }
        #endregion



        const string fileName = "appsettings.json";
        protected static string GetDefaultPath()
        {
            return Path.Combine(AppContext.BaseDirectory , fileName);
        }

 
        public ConfigurationManager(string configPath = null):base(configPath ?? GetDefaultPath())
        {           
        }

        public override void SaveToFile()
        {
            throw new Exception("[ConfigurationManager] not allowed to Save to " + fileName);
        }


    }
}
