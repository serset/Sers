#region << 版本注释-v3 >>
/*
 * ========================================================================
 * 版本：v3
 * 时间：2021-01-23
 * 作者：lith
 * 邮箱：serset@yeah.net
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

namespace Vit.Core.Util.Reflection
{
    public class ObjectLoader
    {


        #region (x.1)Assembly


        #region LoadAssembly       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyFile">如： "Vit.Core.dll"</param>
        /// <param name="assemblyName">如： "Vit.Core"</param>
        /// <returns></returns>
        public static Assembly LoadAssembly(string assemblyFile = null, string assemblyName = null)
        {
            if (!string.IsNullOrEmpty(assemblyName))
                return Assembly.Load(assemblyName);

            if (!string.IsNullOrEmpty(assemblyFile))
                return LoadAssemblyFromFile(assemblyFile);

            return null;
        }
        #endregion


        #region LoadAssemblyFromFile       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyFile">如： "Vit.Core.dll"</param>
        /// <returns></returns>
        public static Assembly LoadAssemblyFromFile(string assemblyFile)
        {
            if (string.IsNullOrEmpty(assemblyFile))
            {
                return null;
            }

            Assembly assembly = null;

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

        #endregion


        #region (x.2)Class

        /// <summary>
        /// 若未指定assembly，则从当前加载的所有Assembly中查找
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="className">如： "Vit.Core.Util.ConfigurationManager.JsonFile"、"Vit.Core.Util.ConfigurationManager.JsonFile,Vit.Core"</param>
        /// <returns></returns>
        public static Type GetType(string className, Assembly assembly)
        {
            if (assembly == null)
            {
                //(x.x.1)
                {
                    var obj = Type.GetType(className, false);
                    if (obj != null) return obj;
                }


                //(x.x.2)load from CurrentDomain
                foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    var obj = asm?.GetType(className);
                    if (obj != null) return obj;
                }
            }
            else
            {
                try
                {
                    var obj = assembly.GetType(className);
                    if (obj != null) return obj;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            return null;
        }






        /// <summary>
        /// 若未指定assembly，则从当前加载的所有Assembly中查找
        /// </summary>
        /// <param name="className">如： "Vit.Core.Util.ConfigurationManager.JsonFile"、"Vit.Core.Util.ConfigurationManager.JsonFile,Vit.Core"</param>
        /// <param name="assemblyFile">如： "Vit.Core.dll"</param>
        /// <param name="assemblyName">如： "Vit.Core"</param>
        /// <returns></returns>
        public static Type GetType(string className, string assemblyFile = null, string assemblyName = null)
        {
            Assembly assembly = LoadAssembly(assemblyFile, assemblyName);

            return GetType(className, assembly);
        }
        #endregion



        #region (x.3)CreateInstance

        /// <summary>
        /// 若未指定assembly，则从当前加载的所有Assembly中查找
        /// </summary>
        /// <param name="className">如： "Vit.Core.Util.ConfigurationManager.JsonFile"、"Vit.Core.Util.ConfigurationManager.JsonFile,Vit.Core"</param>
        /// <param name="assemblyFile">如： "Vit.Core.dll"</param>
        /// <param name="assemblyName">如： "Vit.Core"</param>
        public static object CreateInstance(string className, string assemblyFile = null, string assemblyName = null)
        {
            var type = GetType(className, assemblyFile: assemblyFile, assemblyName: assemblyName);
            if (type != null) return Activator.CreateInstance(type);
            return null;
        }

        #endregion

    }
}
