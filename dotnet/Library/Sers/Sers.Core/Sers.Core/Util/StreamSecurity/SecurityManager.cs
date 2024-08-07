﻿using System;
using System.Linq;

using Newtonsoft.Json.Linq;

using Sers.Core.Util.StreamSecurity.Security;

using Vit.Core.Module.Log;
using Vit.Core.Util.Reflection;
using Vit.Extensions;
using Vit.Extensions.Newtonsoft_Extensions;

namespace Sers.Core.Util.StreamSecurity
{
    public class SecurityManager
    {

        public static SecurityManager BuildSecurityManager(JArray configs = null)
        {
            if (configs == null || configs.Count == 0) return null;


            Logger.Info("[CL.SecurityManager] initing");

            //(x.1) Build security
            var securitys = configs.Select(config =>
            {
                //(x.x.1) get assemblyFile className    
                var className = config["className"].ConvertToString();
                if (string.IsNullOrEmpty(className)) className = typeof(SampleSecurity).FullName;
                var assemblyFile = config["assemblyFile"].ConvertToString();

                #region (x.x.2) CreateInstance
                var security = ObjectLoader.CreateInstance(className, assemblyFile: assemblyFile) as ISecurity;
                if (security == null)
                {
                    var msg = "[CL.SecurityManager] className not exists(" + className + ").";
                    msg += Environment.NewLine + "  config:  " + config;
                    Logger.Error(msg);
                    throw new ArgumentException(msg);
                }
                #endregion


                //(x.x.3) Init
                security.Init(config as JObject);
                return security;
            }).ToArray();


            Logger.Info("[CL.SecurityManager] inited");
            return new SecurityManager { securitys = securitys };
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Encryption(ArraySegment<byte> data)
        {
            foreach (var security in securitys)
            {
                security.Encryption(data);
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Decryption(ArraySegment<byte> data)
        {
            for (int t = securitys.Length - 1; t >= 0; t--)
            {
                securitys[t].Decryption(data);
            }
        }


        ISecurity[] securitys;
    }
}
