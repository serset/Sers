using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sers.Core.CL.MessageOrganize;
using Vit.Core.Module.Log;
using Vit.Core.Util.ConfigurationManager;
using Vit.Core.Util.Reflection;
using Vit.Extensions;

namespace Sers.Core.CL.CommunicationManage
{
    public class CommunicationManageClient
    {

        public CommunicationManageClient()
        {
            defaultConfig= ConfigurationManager.Instance.GetByPath<JObject>("Sers.CL.Config")??new JObject();

            requestTimeoutMs = defaultConfig["requestTimeoutMs"]?.ConvertBySerialize<int?>() ?? 6000;
        }

        private JObject defaultConfig;

        public IOrganizeClient[] organizeList { get; private set; }

        IOrganizeConnection[] connList;

        /// <summary>
        /// 请求超时时间（单位ms，默认60000）(Config.requestTimeoutMs)
        /// </summary>
        public int requestTimeoutMs { get; }

        #region  event

        public Action<IOrganizeConnection> Conn_OnDisconnected { get; set; }

        /// <summary>
        /// 会在内部线程中被调用 
        /// </summary>
        public Action<IOrganizeConnection,object, ArraySegment<byte>, Action<object, List<ArraySegment<byte>>>> conn_OnGetRequest{   get;set;    }

        public Action<IOrganizeConnection,ArraySegment<byte>> conn_OnGetMessage{   get; set;   }
        #endregion

        #region SendMessageAsync      
        public void SendMessageAsync(List<ArraySegment<byte>> message)
        {
            foreach (var conn in connList)
            {
                conn.SendMessageAsync(message);
            }
        }     
        #endregion


        #region Start
        public bool Start()
        {
            if (!BuildOrganizeAndConnect())
            {
                Stop();
                return false;
            }      
            return true;
        }

       
        private bool BuildOrganizeAndConnect()
        {
            #region (x.1)build
            organizeList = BuildOrganize();
            if (organizeList == null || organizeList.Length == 0) return false;
            #endregion


            #region (x.2)set event
            foreach (var organize in organizeList)
            {
                organize.conn_OnGetMessage = conn_OnGetMessage;
                organize.conn_OnGetRequest = conn_OnGetRequest;
                organize.Conn_OnDisconnected = Conn_OnDisconnected;
            }
            #endregion


            #region (x.3)Connect
            foreach (var organize in organizeList)
            {
                try
                {
                    if (!organize.Start())
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message,ex);
                    return false;
                }
            }
            #endregion

            //(x.4) get  connList
            connList = organizeList.Select(organize=>organize.conn).ToArray();

            return true;
        }


        #region BuildOrganize
        /// <summary>
        ///
        /// </summary>
        public Action<JObject[], List<IOrganizeClient>> BeforeBuildOrganize = null;
        private IOrganizeClient[] BuildOrganize()
        {
            try
            {
                List<IOrganizeClient> organizeList = new List<IOrganizeClient>();


                #region (x.1) get configs
                var configs = ConfigurationManager.Instance.GetByPath<JObject[]>("Sers.CL.Client");
                foreach (var config in configs)
                {
                    foreach (var defaultConfigItem in defaultConfig)
                    {
                        if (config[defaultConfigItem.Key] == null)
                        {
                            config[defaultConfigItem.Key] = defaultConfigItem.Value;
                        }
                    }
                }
                #endregion


                //(x.2) BeforeBuildOrganize
                try
                {
                    BeforeBuildOrganize?.Invoke(configs, organizeList);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }


                //(x.3) Build Organize
                foreach (var config in configs)
                {
                    //(x.x.1) get assemblyFile className    
                    var className = config["className"].ConvertToString();
                    if (string.IsNullOrEmpty(className)) continue;
                    var assemblyFile = config["assemblyFile"].ConvertToString();

                    #region (x.x.2) CreateInstance
                    var builder = ObjectLoader.CreateInstance(assemblyFile, className) as IOrganizeClientBuilder;
                    #endregion

                    //(x.x.3) build
                    builder?.Build(organizeList, config);
                }
                return organizeList.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }
        #endregion

        #endregion


        #region Stop
        public void Stop()
        {
            if (organizeList == null) return;
            foreach (var organize in organizeList)
            {
                try
                {
                    organize.Stop();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            organizeList = null;
        }
        #endregion


        

    }
}
