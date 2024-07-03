#region << Version-v4 >>
/*
 * ========================================================================
 * Version： v4
 * Time   ： 2021-04-26
 * Author ： lith
 * Email  ： serset@yeah.net
 * Remarks： 
 * ========================================================================
*/
#endregion


using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.DependencyModel;

using Vit.Core.Module.Log;
using Vit.Core.Util.Common;

namespace Vit.Core.Util.Reflection
{
    public class ObjectLoader
    {


        #region #1 Assembly


        #region LoadAssembly
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyFile">for example : "Vit.Core.dll"</param>
        /// <param name="assemblyName">for example : "Vit.Core"</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// <param name="assemblyFile">for example : "Vit.Core.dll"</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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


        #region #2 Class

        /// <summary>
        /// If no assembly is specified, it will search through all currently loaded assemblies
        /// </summary>
        /// <param name="className">for example : "Vit.Core.Util.ConfigurationManager.JsonFile" , "Vit.Core.Util.ConfigurationManager.JsonFile,Vit.Core"</param>
        /// <param name="assembly"></param>
        /// <param name="assemblyFile">for example : "Vit.Core.dll"</param>
        /// <param name="assemblyName">for example : "Vit.Core"</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetType(string className, Assembly assembly = null, string assemblyFile = null, string assemblyName = null)
        {
            #region #1 get assembly
            if (assembly == null)
            {
                assembly = LoadAssembly(assemblyFile, assemblyName);
            }
            #endregion


            #region #2 get type from assembly
            if (assembly == null)
            {
                // ##1
                {
                    var obj = Type.GetType(className, false);
                    if (obj != null) return obj;
                }


                // ##2 load from CurrentDomain
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
            #endregion
        }

        #endregion



        #region #3 CreateInstance

        /// <summary>
        /// If no assembly is specified, it will search through all currently loaded assemblies
        /// </summary>
        /// <param name="className">for example : "Vit.Core.Util.ConfigurationManager.JsonFile" , "Vit.Core.Util.ConfigurationManager.JsonFile,Vit.Core"</param>
        /// <param name="assemblyFile">for example : "Vit.Core.dll"</param>
        /// <param name="assemblyName">for example : "Vit.Core"</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object CreateInstance(string className, string assemblyFile = null, string assemblyName = null)
        {
            var type = GetType(className, assemblyFile: assemblyFile, assemblyName: assemblyName);
            if (type != null) return Activator.CreateInstance(type);
            return null;
        }
        #endregion

    }
}
