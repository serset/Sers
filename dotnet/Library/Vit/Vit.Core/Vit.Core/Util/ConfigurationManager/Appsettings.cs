using System;

namespace Vit.Core.Util.ConfigurationManager
{
    public class Appsettings : JsonFile
    {
        #region json
        private static JsonFile _json;
        /// <summary>
        /// get config from appsettings.json
        /// </summary>
        public static JsonFile json
        {
            get { return _json ?? (_json = new Appsettings()); }
            set { _json = value; }
        }
        #endregion



        const string fileName = "appsettings.json";

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        protected static string GetDefaultPath()
        {
            return fileName;
        }


        private Appsettings(string configPath = null) : base(configPath ?? GetDefaultPath())
        {
        }

        public override void SaveToFile()
        {
            throw new Exception("[ConfigurationManager] not allowed to Save to " + fileName);
        }


    }
}
