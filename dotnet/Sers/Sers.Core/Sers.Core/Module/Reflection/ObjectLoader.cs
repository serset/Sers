#region << 版本注释-v1 >>
/*
 * ========================================================================
 * 版本：v1
 * 时间：2020-04-14
 * 作者：lith
 * 邮箱：sersms@163.com
 * 说明： 
 * ========================================================================
*/
#endregion


using Microsoft.Extensions.DependencyModel;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Vit.Core.Module.Log;
using Vit.Core.Util.Common;

namespace Sers.Core.Module.Reflection
{
    public class ObjectLoader
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

            #region (x.1)LoadAssemblyByFile
            if (!string.IsNullOrEmpty(assemblyFile))
            {
                assembly = LoadAssemblyByFile(assemblyFile);
                try
                {
                    obj = assembly?.CreateInstance(className);
                    if (obj != null) return obj;
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
            #endregion

            return null;
             
        }


        #region LoadAssemblyByFile       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <returns></returns>
        public static Assembly LoadAssemblyByFile(string assemblyFile)
        {
            if (string.IsNullOrEmpty(assemblyFile))
            {
                return null;
            }

            Assembly assembly=null;

            #region (x.1) get assembly from dll file
            try
            {
                var filePath = CommonHelp.GetAbsPath(assemblyFile);
                if (File.Exists(filePath))
                {
                    assembly = Assembly.LoadFrom(filePath); 
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            #endregion


            var assemblyFileName = Path.GetFileNameWithoutExtension(assemblyFile);

            #region (x.2)Get from DependencyContext               
            if (assembly == null)
            {
                assembly = DependencyContext.Default.RuntimeLibraries
                 .Where(m => m.Name == assemblyFileName)
                 .Select(o => Assembly.Load(new AssemblyName(o.Name))).FirstOrDefault();
            }
            #endregion

            #region (x.3)Get from ReferencedAssemblies               
            if (assembly == null)
            {
                assembly = Assembly.GetEntryAssembly().GetReferencedAssemblies()
                    .Where(m => m.Name == assemblyFileName)
                    .Select(Assembly.Load).FirstOrDefault();
            }
            #endregion


            #region (x.4)Get from CurrentDomain
            if (assembly == null)
            {
                assembly = System.AppDomain.CurrentDomain.GetAssemblies().Where(asm => asm.ManifestModule.Name == assemblyFile).FirstOrDefault();
            }           
            #endregion

            return assembly;
        }
        #endregion
    }
}
