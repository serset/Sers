using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vit.Extensions;
using Vit.Core.Module.Log;
using Vit.Core.Util.Common;
using System;
using System.IO;

namespace Vit.Core.Util.ConfigurationManager
{
    public class JsonFile
    {
        #region static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">要保存的数据</param>
        /// <param name="filePath">json文件路径，例如：new []{"Data", "App.Robot.json"}</param>
        /// <param name="valueKeys">value在json文件中的json路径，可为null。例如：new []{"taskList"}</param>
        public static void SetToFile(object value,string[] filePath,string []valueKeys = null)
        {
            new JsonFile(filePath).Set(value, valueKeys);             
        }

        /// <summary>
        /// 若失败则返回空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath">json文件路径，例如：new []{"Data", "App.Robot.json"}</param>
        /// <param name="valueKeys">value在json文件中的json路径，可为null。例如：new []{"taskList"}</param>
        /// <returns></returns>
        public static T GetFromFile<T>(string[] filePath,string[] valueKeys = null)
        {
            try
            {
                return new JsonFile(filePath).Get<T>(valueKeys);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return default(T);
            }
        }

        #endregion

        
    
        public JToken root { get; protected set; }
        public string configPath { get; protected set; }

        /// <summary>
        /// 通过绝对路径(或相对路径)加载json文件
        /// </summary>
        /// <param name="configPath"></param>
        public JsonFile(string configPath)
        {
            if (string.IsNullOrEmpty(configPath)) return;


            configPath = CommonHelp.GetAbsPath(configPath);

            this.configPath = configPath;
            RefreshConfiguration();
        }

        /// <summary>
        /// 通过相对路径加载json文件
        /// </summary>
        /// <param name="path">如： new []{"Data","sqler.json"}</param>
        public JsonFile(params string[] path ):this(path.Length==0?null:CommonHelp.GetAbsPath(path))
        {
        }


        #region RefreshConfiguration

        /// <summary>
        /// 手动刷新配置，修改配置后，请手动调用此方法，以便更新配置参数
        /// </summary>
        public virtual void RefreshConfiguration()
        {
            root = null;

            #region (x.1)解析Json文件
            try
            {
                string fileContent;
                if (!File.Exists(configPath) || string.IsNullOrEmpty(fileContent = File.ReadAllText(configPath))  )
                {
                    root = new JObject();
                    return;
                }
                root =  JsonConvert.DeserializeObject(fileContent) as JToken;
            }
            catch { }
            #endregion

            //(x.2)
            if (root == null)
                root = new JObject();
        }
        #endregion

        /// <summary>
        /// 保存到原始json文件
        /// </summary>
        public virtual void SaveToFile()
        {
            if (string.IsNullOrEmpty(configPath)) return;

            try
            {
                string dir = Path.GetDirectoryName(configPath);
                if(!string.IsNullOrWhiteSpace(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllText(configPath, root.ToString());
                //File.WriteAllTextAsync(configPath, root.ToString());

                //File.WriteAllBytesAsync(configPath, root.SerializeToBytes());

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
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
        /// <param name="keys">value在Root中的json路径，可为null。例如：new []{"taskList",0,"name"}</param>
        /// <returns></returns>
        public virtual  T Get<T>(params object[] keys)
        {
            JToken cur = root;
            if (null != keys && keys.Length > 0)
            {
                foreach (var key in keys)
                {
                    cur = cur?[key];
                }
            }
            return cur.Deserialize<T>();
        }

        /// <summary>
        /// 会自动保存到原始json文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="keys">value在Root中的json路径，可为null。例如：new []{"taskList",0,"name"}</param>
        public void Set(object value, params object[] keys)
        {
            if (null == keys || keys.Length == 0)
            {
                root = value.ToJToken();
            }
            else
            {
                root.ValueSetByPath(value, keys);
            }
            SaveToFile();
        }




        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">value在Root中的json路径，可为null。例如："a.b.c"</param>
        /// <returns></returns>
        public virtual T GetByPath<T>(string path)
        {
            if(string.IsNullOrEmpty(path))
                return root.Deserialize<T>();
      
            return root.SelectToken(path).Deserialize<T>();
        }


        /// <summary>
        /// 会自动保存到原始json文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="path">value在Root中的json路径，可为null。例如："a.b.c"</param>
        public void SetByPath(object value, string path)
        {
            Set(value,path?.Split('.'));
        }


       

     

    }
}
