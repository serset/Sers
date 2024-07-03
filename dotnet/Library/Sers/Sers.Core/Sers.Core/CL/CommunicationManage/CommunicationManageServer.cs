using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Sers.Core.CL.MessageOrganize;

using Vit.Core.Module.Log;
using Vit.Core.Util.ConfigurationManager;
using Vit.Core.Util.Reflection;
using Vit.Extensions.Json_Extensions;
using Vit.Extensions.Newtonsoft_Extensions;

namespace Sers.Core.CL.CommunicationManage
{
    public class CommunicationManageServer
    {

        #region static CurConn
        //static AsyncCache<IOrganizeConnection> curConn = new AsyncCache<IOrganizeConnection>();
        static System.Threading.AsyncLocal<IOrganizeConnection> curConn = new System.Threading.AsyncLocal<IOrganizeConnection>();

        /// <summary>
        /// 当前连接
        /// </summary>
        public static IOrganizeConnection CurConn { get => curConn.Value; set => curConn.Value = value; }
        #endregion


        public CommunicationManageServer()
        {
            defaultConfig = Appsettings.json.GetByPath<JObject>("Sers.CL.Config") ?? new JObject();

            requestTimeoutMs = defaultConfig["requestTimeoutMs"]?.ConvertBySerialize<int?>() ?? 300000;
        }


        private JObject defaultConfig;

        /// <summary>
        /// 请求超时时间（单位ms，默认300000）(Config.requestTimeoutMs)
        /// </summary>
        public int requestTimeoutMs { get; }

        public IOrganizeServer[] organizeList { get; private set; }

        #region event
        public Action<IOrganizeConnection> Conn_OnConnected { get; set; }

        public Action<IOrganizeConnection> Conn_OnDisconnected { get; set; }


        /// <summary>
        /// 会在内部线程中被调用 
        /// (conn,sender,requestData, callback)
        /// </summary>
        public Action<IOrganizeConnection, object, ArraySegment<byte>, Action<object, Vit.Core.Util.Pipelines.ByteData>> conn_OnGetRequest { get; set; }

        public Action<IOrganizeConnection, ArraySegment<byte>> conn_OnGetMessage { get; set; }




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

        #region BuildOrganizeAndConnect        
        private bool BuildOrganizeAndConnect()
        {

            #region (x.1)build
            organizeList = BuildOrganize();
            if (organizeList == null || organizeList.Length == 0) return false;
            #endregion


            #region (x.2)set event
            foreach (var organize in organizeList)
            {
                organize.Conn_OnConnected = Conn_OnConnected;
                organize.Conn_OnDisconnected = Conn_OnDisconnected;
                organize.conn_OnGetRequest = conn_OnGetRequest;
                organize.conn_OnGetMessage = conn_OnGetMessage;

            }
            #endregion


            #region (x.3)Start
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
                    Logger.Error(ex);
                    return false;
                }
            }
            #endregion

            return true;
        }

        #region BuildOrganize

        /// <summary>
        /// configs,organizeList
        /// </summary>
        public Action<JObject[], List<IOrganizeServer>> BeforeBuildOrganize { get; set; } = null;

        private IOrganizeServer[] BuildOrganize()
        {
            try
            {
                List<IOrganizeServer> organizeList = new List<IOrganizeServer>();


                #region (x.1) get configs
                var configs = Appsettings.json.GetByPath<JObject[]>("Sers.CL.Server");
                if (configs == null) return null;
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


                //(x.3) BuildOrganize
                foreach (var config in configs)
                {
                    //(x.x.1) get className    
                    var className = config["className"].ConvertToString();
                    if (string.IsNullOrEmpty(className)) continue;
                    var assemblyFile = config["assemblyFile"].ConvertToString();

                    #region (x.x.2) CreateInstance
                    var builder = ObjectLoader.CreateInstance(className, assemblyFile: assemblyFile) as IOrganizeServerBuilder;
                    #endregion

                    //(x.x.3) build
                    builder?.Build(organizeList, config);
                }

                if (organizeList.Count == 0) return null;

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
