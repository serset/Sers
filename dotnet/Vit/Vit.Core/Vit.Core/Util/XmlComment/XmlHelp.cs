using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using  System.Linq;
using System.Reflection;
using System.Xml;

namespace Vit.Core.Util.XmlComment
{
    public class XmlHelp:IDisposable
    {
        /*  
<?xml version="1.0"?>
<doc>
    <assembly>
        <name>test</name>
    </assembly>
    <members>
        <member name="T:Namespace1.Namespace2.Class1">
            <summary>
            summary-Class1
            </summary>
        </member>
        <member name="M:Namespace1.Namespace2.Class1.Method1(System.String,System.Int32)">
            <summary>
            summary-Method1
            </summary>
            <param name="arg1">param-arg1</param>
            <param name="arg2">param-arg2</param>
            <returns>returns-return</returns>
        </member>
    </members>
</doc>
*/

        #region MyRegion

        public  string assemblyName;
        private SortedDictionary<String, XmlNodeList> members=new SortedDictionary<string, XmlNodeList>();

        public XmlHelp(string xmlFilePath)
        {
            //(x.1)创建文档对象
            JObject joXml = new JObject();
            XmlDocument doc = new XmlDocument();
            //加载XML
            doc.LoadXml(File.ReadAllText(xmlFilePath));


            var docElem = doc.DocumentElement;

            #region assemblyName

            assemblyName=NodeList_GetInnerText(docElem.ChildNodes, "assembly");
 
            #endregion

            #region members
            var xmlMembers= (from XmlNode item in docElem.ChildNodes
                where item is XmlElement && item.Name == "members"
                            select ((XmlElement)item)).FirstOrDefault();

            if(null== xmlMembers) return;
            ;
            foreach (XmlNode node in xmlMembers.ChildNodes)
            {
                string name = node.Attributes?["name"]?.Value;
                if(string.IsNullOrWhiteSpace(name))continue;
                
                var ChildNodes = node.ChildNodes;
                if (null== ChildNodes) continue;
                members[name] = ChildNodes;
            }
            #endregion
        }
        #endregion


        static string NodeList_GetInnerText(XmlNodeList list,string tagName= "summary")
        {
            if (null == list) return null;
            return (from XmlNode item in list
                            where  item.Name == tagName
                            select item.InnerText).FirstOrDefault();
        }

        public string Type_GetSummary(Type type)
        {
            var memberName = "T:" + type.FullName;

            if (!members.TryGetValue(memberName, out var list)) return null;

            return NodeList_GetInnerText(list);

        }

        
        public string Property_GetSummary(PropertyInfo info)
        {
            /*     <member name = "P:Sers.Core.Module.Api.MsTest.LocalApi.Controllers.DemoFullController.ArgModel.arg1" 
                        <summary>
                        arg1Desc - xml
                        </summary>
                    </member>          
             */
            var memberName = "P:" + info.ReflectedType.FullName.Replace('+', '.') + "." + info.Name;

            if (!members.TryGetValue(memberName, out var nodeList)) return null;
            return NodeList_GetInnerText(nodeList, "summary")?.Trim();
        }
        public string Field_GetSummary(FieldInfo info)
        {
            /*   <member name="F:Sers.Core.Module.Api.MsTest.LocalApi.Controllers.DemoFullController.ArgModel.arg1">
                    <summary>
                    arg1Desc-xml
                    </summary>
                 </member>             
             */
            var memberName = "F:" + info.ReflectedType.FullName.Replace('+', '.');

            //memberName可能为  "F:xxxxxxxxxxxxxx[[xx]]"
            // Sers.Core.Module.Api.ApiReturn`1[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
            var t = memberName.IndexOf("[[");
            if (t > 0)
            {
                memberName = memberName.Remove(t);
            }

            memberName += "." + info.Name;

            if (!members.TryGetValue(memberName, out var nodeList)) return null;
            return NodeList_GetInnerText(nodeList, "summary")?.Trim();
        }

        public MethodComment Method_GetComment(MethodInfo info)
        {
            var memberName = "M:" + info.ReflectedType.FullName.Replace('+', '.');

            var sign = info.Name;

            var argArray = info.GetParameters();
            if (argArray.Length > 0)
            {
                sign += "(";
                sign += string.Join(",", (from item in argArray select item?.ParameterType?.FullName?.Replace('+','.')).ToArray());
                sign += ")";
            }

            memberName += "." + sign;
            if (!members.TryGetValue(memberName, out var nodeList)) return null;

            var comment = new MethodComment();

            comment.summary= NodeList_GetInnerText(nodeList,"summary")?.Trim();

            comment.returns = NodeList_GetInnerText(nodeList, "returns");

            #region param
            comment.param=new MethodComment.Param[argArray.Length];
            
            var arr = (from XmlNode item in nodeList
                where item.Name == "param"
                    select new MethodComment.Param(item));
            foreach (var param in arr)
            {
                for (int i = 0; i < argArray.Length; i++)
                {
                    if (argArray[i].Name == param.name)
                    {
                        comment.param[i] = param;
                    } 
                }
            }
            #endregion

            return comment;

        }

        public void Dispose()
        {
            members.Clear();
        }
    }
}
