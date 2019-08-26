using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace Sers.Core.Util.XmlComment
{
    public class XmlMng:IDisposable
    {
        public void Dispose()
        {
            foreach (var kv in assemblys)
            {
                kv.Value?.Dispose();
            }
            assemblys.Clear();
        }

        private SortedDictionary<String, XmlHelp> assemblys = new SortedDictionary<string, XmlHelp>();


        public void AddXml(string xmlFilePath)
        {
            var xmlHelp=new XmlHelp(xmlFilePath);
            if(!string.IsNullOrEmpty(xmlHelp.assemblyName))
                assemblys[xmlHelp.assemblyName] = xmlHelp;
        }


        public void AddBin(string path=null)
        {
            foreach (FileInfo fi in new DirectoryInfo(path??AppContext.BaseDirectory).GetFiles("*.xml"))
            {
                try
                {
                    AddXml(fi.FullName);
                }
                catch (Exception)
                {                     
                }
               
            }
        }

        public XmlHelp GetXmlHelp(Type type)
        {
            var assemblyName = type.Assembly.FullName+",";
            assemblyName = assemblyName.Split(',')[0];
            assemblys.TryGetValue(assemblyName, out var value);
            return value;
        }

        public XmlHelp GetXmlHelp(MethodInfo method)
        {
            return GetXmlHelp(method.ReflectedType);
        }

      
    }
}
