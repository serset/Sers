namespace Sers.Core.CL.CommunicationManage
{
    public partial class CommunicationManageClient
    {
        /// <summary>
        /// 站点通讯层默认连接配置
        /// </summary>
        const string DefaultClientConfig = @"
[
    {
        // Socket.Iocp
        /* (x.1) type - Iocp */
        /* the class of builder in assemblyFile  */
        ""className"": ""Sers.CL.Socket.Iocp.OrganizeClientBuilder"",

        /* 通信模式（默认值：Simple）。可为 Simple、Timer、ThreadWait  */
        //""mode"": ""ThreadWait"",

        /* (x.2) conn config */
        /* 服务端 host地址。例如： ""127.0.0.1""、""sers.cloud"" */
        ""host"": ""127.0.0.1"",
        /* 服务端 监听端口号。例如： 4501 */
        ""port"": 4501,
        /* 连接秘钥，用以验证连接安全性。服务端和客户端必须一致 */
        ""secretKey"": ""SersCL""
    }
]
";

    }
}
