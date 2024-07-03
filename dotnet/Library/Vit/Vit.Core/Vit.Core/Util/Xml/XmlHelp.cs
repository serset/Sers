
using System.IO;
using System.Text;
using System.Xml;

using Newtonsoft.Json.Linq;

namespace Vit.Core.Util.Xml
{
    /// <summary>
    ///    
    /// </summary>
    public class XmlHelp
    {
        public const string jsonStringDemo = @"
{                                                                                    
    Declaration:{ ""version"":""1.0"",""encoding"":""utf-8"",""standalone"":null }             
    ,Children:                                                                        
    [
        {                                                                            
            Tag:""soap12:Envelope""                                                    
            , Attribute:{""xmlns:xsi"":""http://www.w3.org/2001/XMLSchema-instance"",""a"":""bbb""}
            ,Children:                                                               
            [
                ""dddddddd"",
                {Tag:""DoTransResult"", Children[""string""]                              
            ]                                                                        
        }                                                                            
    ]                                                                                
}
";


        public const string xmlStringDemo = @"
//xml:
<?xml version=""1.0"" encoding=""utf-8"" ?>
<soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" a=""bbb"" > 
    dddddddd
    <DoTransResult> string</DoTransResult>    
</soap12:Envelope> 
";


        #region XmlString To Json


        /* 
         * 




  




















//*/


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static JObject XmlStringToJson(string xmlString)
        {

            //(x.1)创建文档对象
            JObject joXml = new JObject();
            XmlDocument doc = new XmlDocument();
            //加载XML
            doc.LoadXml(xmlString);



            #region (x.2) 头部信息          
            if (doc.ChildNodes[0] is XmlDeclaration dec)
            {
                var joDeclaration = new JObject();
                joXml["Declaration"] = joDeclaration;

                if (!string.IsNullOrEmpty(dec.Version))
                    joDeclaration["version"] = dec.Version;
                if (!string.IsNullOrEmpty(dec.Encoding))
                    joDeclaration["encoding"] = dec.Encoding;
                if (!string.IsNullOrEmpty(dec.Standalone))
                    joDeclaration["standalone"] = dec.Standalone;
            }
            #endregion

            #region (x.3) Children        
            JArray joChildren = new JArray();
            joXml["Children"] = joChildren;
            joChildren.Add(ParseXmlNode(doc.DocumentElement));
            #endregion


            return joXml;


            #region function ParseXmlNode


            JToken ParseXmlNode(XmlNode xmlNode)
            {
                if (null == xmlNode) return null;

                if (xmlNode is XmlText xmlText) return xmlText.Data;

                if (!(xmlNode is XmlElement xmlElem)) return null;

                #region 处理XmlElement


                /*

                 { Tag:"soap12:Envelope"
                        ,Attribute:{"xmlns:xsi":"http://www.w3.org/2001/XMLSchema-instance"}
                        ,Children:[ 
                                    "dddddddd",
                                    {Tag:"DoTransResult",Children["string"] }
                                ]
                     }                 
                 */
                //(x.x.1) CreateElement
                JObject joElem = new JObject();
                joElem["Tag"] = xmlElem.Name;

                //(x.x.2) Attribute
                var Attribute = xmlElem.Attributes;
                if (null != Attribute)
                {
                    var joAttribute = new JObject();
                    joElem["Attribute"] = joAttribute;
                    foreach (XmlAttribute attr in Attribute)
                    {
                        joAttribute[attr.Name] = attr.Value;
                    }
                }


                //(x.x.3) Children
                var _Children = xmlElem.ChildNodes;
                if (xmlElem.HasChildNodes)
                {
                    var _joChildren = new JArray();
                    joElem["Children"] = _joChildren;

                    foreach (XmlNode child in xmlElem.ChildNodes)
                    {
                        var joChild = ParseXmlNode(child);
                        if (null != joChild)
                            _joChildren.Add(joChild);
                    }
                }
                return joElem;
                #endregion
            }
            #endregion
        }
        #endregion



        #region Json To XmlDocument
        /// <summary>
        /// 
        /// </summary>
        /// <param name="joXml"></param>
        /// <returns></returns>
        public static XmlDocument JsonToXmlDocument(JObject joXml)
        {

            JObject joDeclaration = joXml["Declaration"]?.Value<JObject>();


            //(x.1)创建文档对象
            XmlDocument doc = new XmlDocument();


            #region (x.2) 头部信息           
            //声明XML头部信息
            XmlDeclaration dec = doc.CreateXmlDeclaration(joDeclaration?["version"]?.Value<string>() ?? "1.0", joDeclaration?["encoding"]?.Value<string>() ?? "UTF-8", joDeclaration?["standalone"]?.Value<string>());

            //添加进doc对象子节点
            doc.AppendChild(dec);
            #endregion


            #region (x.3) 递归创建子节点

            var Children = joXml["Children"]?.Value<JArray>();
            if (null != Children)
            {
                foreach (JToken child in Children)
                {
                    var xmlChild = ToXmlNode(doc, child);
                    if (null != xmlChild)
                        doc.AppendChild(xmlChild);
                }
            }
            #endregion

            //(x.4) return
            return doc;



            #region function ToXmlNode


            XmlNode ToXmlNode(XmlDocument xmlDoc, JToken elem)
            {
                //(x.1)为string
                if (elem.Type == JTokenType.String)
                {
                    return xmlDoc.CreateTextNode(elem.Value<string>());
                }

                //(x.2)不为JOject 返回null
                if (elem.Type != JTokenType.Object)
                {
                    return null;
                }




                #region (x.3)为JOject


                /*

                 { Tag:"soap12:Envelope"
                        ,Attribute:{"xmlns:xsi":"http://www.w3.org/2001/XMLSchema-instance"}
                        ,Children:[ 
                                    "dddddddd",
                                    {Tag:"DoTransResult",Children["string"] }
                                ]
                     }                 
                 */
                //(x.x.1) CreateElement
                XmlElement xmlElem = xmlDoc.CreateElement(elem["Tag"]?.Value<string>());

                //(x.x.2) Attribute
                var Attribute = elem["Attribute"]?.Value<JObject>();
                if (null != Attribute)
                {
                    foreach (var kv in Attribute)
                    {
                        xmlElem.SetAttribute(kv.Key, kv.Value.Value<string>());
                    }
                }


                //(x.x.3) Children
                var _Children = elem["Children"]?.Value<JArray>();
                if (null != _Children)
                {
                    foreach (JToken child in _Children)
                    {
                        var xmlChild = ToXmlNode(xmlDoc, child);
                        if (null != xmlChild)
                            xmlElem.AppendChild(xmlChild);
                    }
                }
                return xmlElem;
                #endregion
            }
            #endregion
        }




        #endregion

        #region XmlDocument To String
        public string XmlDocumentToString(XmlDocument doc)
        {
            using (StringWriter sw = new StringWriter())
            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(sw))
            {
                doc.WriteTo(xmlTextWriter);
                return sw.ToString();
            }
        }
        #endregion


        #region Json To XmlString


        /// <summary>
        /// 直接字符串拼接构建XmlString
        /// </summary>
        /// <param name="joXml"></param>
        /// <returns></returns>
        public static string JsonToXmlString(JObject joXml)
        {
            /*
<?xml version="1.0" encoding="utf-8"?>
<soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
  <soap12:Body>
    <DoTrans xmlns="http://tempuri.org/">
      <transCode>string</transCode>
      <inJsonString>string</inJsonString>
    </DoTrans>
  </soap12:Body>
</soap12:Envelope>            
             */

            JObject joDeclaration = joXml["Declaration"]?.Value<JObject>();


            //(x.1)创建文档对象
            StringBuilder xmlBuilder = new StringBuilder(1000);

            JToken token;

            #region (x.2) 头部信息     
            //xmlBuilder.Append("<?xml version="1.0" encoding="utf - 8"?>");
            xmlBuilder.Append("<?xml");

            //version
            if (null != (token = joDeclaration?["version"]))
            {
                xmlBuilder.Append(" version=\"");
                AppendXmlStr(xmlBuilder, token.Value<string>());
                xmlBuilder.Append("\"");
            }

            //encoding
            if (null != (token = joDeclaration?["encoding"]))
            {
                xmlBuilder.Append(" encoding=\"");
                AppendXmlStr(xmlBuilder, token.Value<string>());
                xmlBuilder.Append("\"");
            }

            //standalone
            if (null != (token = joDeclaration?["standalone"]))
            {
                xmlBuilder.Append(" standalone=\"");
                AppendXmlStr(xmlBuilder, token.Value<string>());
                xmlBuilder.Append("\"");
            }
            xmlBuilder.Append("?>");
            #endregion


            #region (x.3) 递归创建子节点

            var Children = joXml["Children"]?.Value<JArray>();
            if (null != Children)
            {
                foreach (JToken child in Children)
                {
                    AppendXmlNode(xmlBuilder, child);
                }
            }
            #endregion

            #region (x.4) ToString
            return xmlBuilder.ToString();
            #endregion


            #region function AppendXmlNode


            void AppendXmlNode(StringBuilder builder, JToken elem)
            {
                //(x.1)为string
                if (elem.Type == JTokenType.String)
                {
                    AppendXmlStr(xmlBuilder, elem.Value<string>());
                }

                //(x.2)不为JOject 返回null
                if (elem.Type != JTokenType.Object)
                {
                    return;
                }


                #region (x.3)为JOject
                /*
    <DoTrans xmlns="http://tempuri.org/">
      <transCode>string</transCode>
      <inJsonString>string</inJsonString>
    </DoTrans>

                 { Tag:"soap12:Envelope"
                        ,Attribute:{"xmlns:xsi":"http://www.w3.org/2001/XMLSchema-instance"}
                        ,Children:[ 
                                    "dddddddd",
                                    {Tag:"DoTransResult",Children["string"] }
                                ]
                     }                 
                 */
                var tag = elem["Tag"]?.Value<string>();
                //(x.x.1) 开始标签
                builder.Append("<");
                AppendXmlStr(xmlBuilder, tag);


                //(x.x.2) Attribute
                var Attribute = elem["Attribute"]?.Value<JObject>();
                if (null != Attribute)
                {
                    foreach (var kv in Attribute)
                    {
                        builder.Append(" ");
                        AppendXmlStr(xmlBuilder, kv.Key);
                        builder.Append("=\"");
                        AppendXmlStr(xmlBuilder, kv.Value.Value<string>());
                        builder.Append("\"");
                    }
                }
                builder.Append(">");

                //(x.x.3) Children
                var _Children = elem["Children"]?.Value<JArray>();
                if (null != _Children)
                {
                    foreach (JToken child in _Children)
                    {
                        AppendXmlNode(builder, child);
                    }
                }
                //(x.x.4) 结束标签
                builder.Append("</");
                AppendXmlStr(xmlBuilder, tag);
                builder.Append(">");
                #endregion
            }
            #endregion
        }

        #endregion


        #region Util


        /// <summary>
        /// <para>向xml 的 内容 转换。                                                         </para>
        /// <para>例如 转换为  &lt;a title=""&gt;ok&lt;/a&gt;  中a标签的内容体（innerHTML）    </para>
        /// <para>或 转换为  &lt;a title=""&gt;ok&lt;/a&gt;  中title的值。                     </para>
        /// <para>                                                                             </para>
        /// <para>转换 " &amp;  &lt;   &gt;  为  &amp;quot;  &amp;amp;   &amp;lt;   &amp;gt; 。</para>
        /// <para>注： ' 对应 &amp;apos; ，但有些浏览器不支持，故此函数不转换。                </para>
        /// </summary>
        /// <param name="str">若为 空 或 空字符串，则原样返回</param>
        /// <returns></returns>
        static string StrToXmlStr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            StringBuilder builder = new StringBuilder();
            foreach (char c in str)
            {
                switch (c)
                {
                    case '"':
                        builder.Append("&quot;");
                        break;
                    case '&':
                        builder.Append("&amp;");
                        break;
                    //case '\'':
                    //    builder.Append("&apos;");
                    //    break;
                    case '<':
                        builder.Append("&lt;");
                        break;
                    case '>':
                        builder.Append("&gt;");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// <para>向xml 的 内容 转换 并追加到 builder。                                         </para>
        /// <para>例如 转换为  &lt;a title=""&gt;ok&lt;/a&gt;  中a标签的内容体（innerHTML）     </para>
        /// <para>或 转换为  &lt;a title=""&gt;ok&lt;/a&gt;  中title的值。                      </para>
        /// <para>                                                                              </para>
        /// <para>转换 " &amp;  &lt;   &gt;  为  &amp;quot;  &amp;amp;   &amp;lt;   &amp;gt; 。 </para>
        /// <para>注： ' 对应 &amp;apos; ，但有些浏览器不支持，故此函数不转换。                 </para>
        /// </summary>
        /// <param name="builder">不可为空</param>
        /// <param name="str">若为 空 或 空字符串，则原样返回</param>
        /// <returns>原 builder</returns>
        static StringBuilder AppendXmlStr(StringBuilder builder, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return builder;
            }
            foreach (char c in str)
            {
                switch (c)
                {
                    case '"':
                        builder.Append("&quot;");
                        break;
                    case '&':
                        builder.Append("&amp;");
                        break;
                    //case '\'':
                    //    builder.Append("&apos;");
                    //    break;
                    case '<':
                        builder.Append("&lt;");
                        break;
                    case '>':
                        builder.Append("&gt;");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
            return builder;
        }

        #endregion

    }
}
