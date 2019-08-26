using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using Sers.Core.Util.Common;
using System;
using System.IO;

namespace Sers.Core.Util.ConfigurationManager
{
    public class JsonFile
    {
        #region static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">要保存的数据</param>
        /// <param name="filePath">json文件路径，例如：new []{"Data", "App.Robot.json"}</param>
        /// <param name="jsonPath">value在json文件中的json路径，可为null。例如：new []{"taskList"}</param>
        public static void SaveToFile(object value,string[] filePath,string []jsonPath = null)
        {
            new JsonFile(filePath).SetValueByPath(value, jsonPath);             
        }

        /// <summary>
        /// 若失败则返回空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath">json文件路径，例如：new []{"Data", "App.Robot.json"}</param>
        /// <param name="jsonPath">value在json文件中的json路径，可为null。例如：new []{"taskList"}</param>
        /// <returns></returns>
        public static T LoadFromFile<T>(string[] filePath,string[] jsonPath = null)
        {
            try
            {
                return new JsonFile(filePath).Get<T>(jsonPath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return default(T);
            }
        }

        #endregion



        protected JToken root;
        public JToken Root =>root;
        protected string configPath;
        public JsonFile(string configPath)
        {
            this.configPath = configPath;
            RefreshConfiguration();
        }

        /// <summary>
        /// 相对路径
        /// </summary>
        /// <param name="configPath"></param>
        public JsonFile(params string[] configPath ):this(CommonHelp.GetAbsPathByRealativePath(configPath))
        {
            
        }

        /// <summary>
        /// 手动刷新配置，修改配置后，请手动调用此方法，以便更新配置参数
        /// </summary>
        public virtual void RefreshConfiguration()
        {
            root = null;

            #region (x.1)解析Json文件,失败返回null
            string FilePath = configPath;           
            try
            {
                root = JsonConvert.DeserializeObject(File.ReadAllText(FilePath)) as JToken;
                //return JObject.Parse(File.ReadAllText(FilePath));

                //var data = File.ReadAllBytes(FilePath).BytesToString();          
                //return JsonConvert.DeserializeObject(data) as JToken ;
            }
            catch { }
            #endregion

            //(x.2)
            if (root == null)
                root = new JObject();
        }




        public virtual string GetStringByPath(string path)
        {
            var cur = root?.SelectToken(path);
            return cur.ConvertToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys">value在Root中的json路径，可为null。例如：new []{"taskList"}</param>
        /// <returns></returns>
        public virtual  T Get<T>(params string[] keys)
        {
            JToken cur = root;
            if (null != keys && keys.Length > 0)
                foreach (var key in keys)
                {
                    cur = cur?[key];
                }
            return cur.Deserialize<T>();
        }

        public virtual T GetByPath<T>(string path)
        {
            var cur = root.SelectToken(path);
            return cur.Deserialize<T>();
        }
        
       
        public void SetValueByPath(object value, params object[] path)
        {
            if (null == path || path.Length == 0)
            {
                root = value.ToJToken();
            }
            else
            {
                Root.SetValueByPath(value, path);
            }
            SaveToFile();
        }


        public virtual void SaveToFile()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(configPath));

                File.WriteAllText(configPath, root.ToString());
                //File.WriteAllTextAsync(configPath, root.ToString());

                //File.WriteAllBytesAsync(configPath, root.SerializeToBytes());

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


     

    }
}
