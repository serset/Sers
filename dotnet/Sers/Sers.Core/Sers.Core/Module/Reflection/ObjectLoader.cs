using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Vit.Core.Module.Log;
using Vit.Core.Util.Common;

namespace Sers.Core.Module.Reflection
{
    internal class ObjectLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyFile">如： "Sers.CL.Zmq.FullDuplex.dll"</param>
        /// <param name="className">如： "Sers.CL.Zmq.FullDuplex.OrganizeClientBuilder"</param>
        /// <returns></returns>
        public static object CreateInstance(string assemblyFile,string className)
        {
            Assembly assembly = null;
            Object obj = null;

            #region (x.1)load from relative path
            if (!string.IsNullOrEmpty(assemblyFile))
            {
                try
                {
                    var filePath = CommonHelp.GetAbsPathByRealativePath(assemblyFile);
                    if (File.Exists(filePath))
                    {
                        assembly = Assembly.LoadFrom(filePath);

                        obj = assembly?.CreateInstance(className);
                        if (obj != null) return obj;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            #endregion


            #region (x.2)load from CurrentDomain
            if (string.IsNullOrEmpty(assemblyFile))
            {
                foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    obj = asm?.CreateInstance(className);
                    if (obj != null) return obj;
                }
            }
            else
            {
                assembly = System.AppDomain.CurrentDomain.GetAssemblies().Where(asm => asm.ManifestModule.Name == assemblyFile).FirstOrDefault();

                obj = assembly?.CreateInstance(className);
                if (obj != null) return obj;
            }
            #endregion

            return null;
             
        }
    }
}
