﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace Vit.Core.Util.XmlComment
{
    public class XmlCommentMng : IDisposable
    {
        public void Dispose()
        {
            foreach (var kv in assemblys)
            {
                kv.Value?.Dispose();
            }
            assemblys.Clear();
        }

        private SortedDictionary<String, XmlCommentHelp> assemblys = new SortedDictionary<string, XmlCommentHelp>();

        /// <summary>
        /// 添加xml注释
        /// </summary>
        /// <param name="xmlFilePath"></param>
        public void AddXml(string xmlFilePath)
        {
            var xmlHelp = new XmlCommentHelp(xmlFilePath);
            if (!string.IsNullOrEmpty(xmlHelp.assemblyName))
                assemblys[xmlHelp.assemblyName] = xmlHelp;
        }


        /// <summary>
        /// 添加bin目录下的所有xml注释文件
        /// </summary>
        /// <param name="path"></param>
        public void AddBin(string path = null)
        {
            foreach (FileInfo fi in new DirectoryInfo(path ?? AppContext.BaseDirectory).GetFiles("*.xml"))
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

        public XmlCommentHelp GetXmlHelp(Type type)
        {
            var assemblyName = type.Assembly.FullName + ",";
            assemblyName = assemblyName.Split(',')[0];
            assemblys.TryGetValue(assemblyName, out var value);
            return value;
        }

        public XmlCommentHelp GetXmlHelp(MethodInfo method)
        {
            return GetXmlHelp(method.ReflectedType);
        }


    }
}
