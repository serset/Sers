using System.Xml;

namespace Sers.Core.Util.XmlComment
{

    public class MethodComment
    {
        public class Param
        {
            public string name;
            public string comment;

            public Param(XmlNode node)
            {
                name = node.Attributes["name"].Value;
                comment = node.InnerText?.Trim();
            }

        }

        public string summary;
        public string returns;
        public Param[] param;

    }

}