﻿using System;
using System.Linq;

using Newtonsoft.Json.Linq;

using Vit.Core.Module.Log.LogCollector;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Core.Util.ConfigurationManager;
using Vit.Core.Util.Reflection;
using Vit.Extensions;
using Vit.Extensions.Newtonsoft_Extensions;

namespace Vit.Core.Module.Log
{
    /// <summary>
    /// FATAL > ERROR > WARN > INFO > DEBUG 
    /// </summary>
    public static class Logger
    {

        public static readonly LogMng log = new LogMng();

        #region static Init
        static Logger()
        {
            if (false != Appsettings.json.GetByPath<bool?>("Vit.Logger.PrintToTxt"))
            {
                PrintToTxt = true;
            }

            if (false != Appsettings.json.GetByPath<bool?>("Vit.Logger.PrintToConsole"))
            {
                PrintToConsole = true;
            }
            LoadCollector();
        }


        private static void LoadCollector()
        {
            #region GetInstance
            ILogCollector GetInstance(JObject config)
            {
                //(x.x.1) get className
                var className = config["className"].ConvertToString();
                if (string.IsNullOrEmpty(className)) return null;

                #region (x.x.2)是否内置对象
                if (className == "SplunkCollector" || className == "Vit.Core.Module.Log.LogCollector.Splunk.SplunkCollector")
                {
                    return new LogCollector.Splunk.SplunkCollector();
                }

                if (className == "ElasticSearchCollector" || className == "Vit.Core.Module.Log.LogCollector.ElasticSearch.ElasticSearchCollector")
                {
                    return new LogCollector.ElasticSearch.ElasticSearchCollector();
                }
                #endregion

                var assemblyFile = config["assemblyFile"].ConvertToString();
                if (string.IsNullOrEmpty(assemblyFile))
                {
                    return null;
                }
                return ObjectLoader.CreateInstance(className, assemblyFile: assemblyFile) as ILogCollector;
            }
            #endregion


            var configs = Vit.Core.Util.ConfigurationManager.Appsettings.json.GetByPath<JObject[]>("Vit.Logger.Collector");
            if (configs == null || configs.Length == 0) return;
            configs.IEnumerable_ForEach(config =>
            {
                try
                {
                    //(x.x.1) GetInstance
                    var collector = GetInstance(config);
                    if (collector == null) return;


                    //(x.x.2) init
                    collector.Init(config);


                    log.AddCollector(collector);
                }
                catch { }
            });


        }

        #endregion

        #region PrintToTxt  
        static bool _PrintToTxt = false;
        public static bool PrintToTxt
        {
            get { return _PrintToTxt; }
            set
            {
                _PrintToTxt = value;
                if (value)
                {
                    if (!log.collectors.Any(c => c is TxtCollector))
                    {
                        log.collectors.Add(new LogCollector.TxtCollector());
                    }
                }
                else
                {
                    log.collectors.RemoveAll(c => c is TxtCollector);
                }
            }
        }
        #endregion

        #region PrintToConsole  
        static bool _PrintToConsole = false;
        public static bool PrintToConsole
        {
            get { return _PrintToConsole; }
            set
            {
                _PrintToConsole = value;
                if (value)
                {
                    if (!log.collectors.Any(c => c is ConsoleCollector))
                    {
                        log.collectors.Add(new ConsoleCollector());
                    }
                }
                else
                {
                    log.collectors.RemoveAll(c => c is ConsoleCollector);
                }
            }
        }
        #endregion

        #region Log
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Log(LogMessage msg)
        {
            log.Log(msg);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Log(Level level, string message, params object[] metadata)
        {
            log.Log(level, message, metadata);
        }
        #endregion


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Debug(string message)
        {
            log.Debug(message);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Debug(string message, params object[] metadata)
        {
            log.Debug(message, metadata);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Warn(string message)
        {
            log.Warn(message);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Warn(string message, params object[] metadata)
        {
            log.Warn(message, metadata);
        }


        #region Info


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Info(string message)
        {
            log.Info(message);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Info(string message, params object[] metadata)
        {
            log.Info(message, metadata);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Info(object message)
        {
            log.Info(message);
        }
        #endregion



        #region Error

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Error(string message)
        {
            log.Error(message);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Error(string message, params object[] metadata)
        {
            log.Error(message, metadata);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Error(Exception ex)
        {
            log.Error(ex);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Error(string message, Exception ex)
        {
            log.Error(message, ex);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Error(SsError ssError)
        {
            log.Error(ssError);
        }

        #endregion



    }
}
