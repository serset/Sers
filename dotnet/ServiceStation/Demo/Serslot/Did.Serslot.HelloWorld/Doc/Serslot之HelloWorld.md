# Serslot之HelloWorld
>2020-01-27 18:17

------------

Sers提供了3种c#接入的方式（用户亦可自定义接入），Serslot是对net core2.1 web api的原生支持。无需修改原有web api代码结构，修改3处地方（仅一处代码）即可无缝接入。[点我查看源码](https://github.com/serset/serset.github.io/tree/master/code/SerslotDemo2.1.1.250)。[点我下载源码](https://serset.github.io/file/demo/SerslotDemo2.1.1.250.zip)。

## 1.添加nuget包引用
>编辑csproj文件，添加如下代码，通过nuget安装Serslot

```xml
<ItemGroup>
	<PackageReference Include="Sers.Serslot" Version="2.1.1.250" />
</ItemGroup>
```

## 2.修改项目启动代码
>编辑Program.cs文件，按照如下添加两行代码

```csharp
// Program.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vit.Extensions;   //----添加代码1

namespace SerslotDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerslot()  //----添加代码2
                .UseStartup<Startup>();
    }
}

```

## 3.修改配置文件
>编辑 appsettings.json 文件，添加 Sers 配置（暂时不要关心配置内容是什么、有什么作用，直接把 Sers节点（3到30行）复制进去就好）。

```json
//appsettings.json
{
  "Sers": {
    /* 通讯层配置 */
    "CL": {      
      /* one conn is one channel.can be multiable */
      "Client": [
        {
          // Socket.Iocp
          /* (x.1) type - Iocp */
          /* the class of builder in assemblyFile  */
          "className": "Sers.CL.Socket.Iocp.OrganizeClientBuilder",

          /* 通信模式（默认值：Simple）。可为 Simple、Timer、ThreadWait  */
          //"mode": "ThreadWait",

          /* (x.2) conn config */
          /* 服务端 host地址。例如： "127.0.0.1"、"sers.cloud" */
          "host": "127.0.0.1",
          /* 服务端 监听端口号。例如： 4501 */
          "port": 4501,
          /* 连接秘钥，用以验证连接安全性。服务端和客户端必须一致 */
          "secretKey": "SersCL"
        }
      ]
    },

    /* LocalApiService 配置,可不指定 */
    "LocalApiService": {
      /* 后台服务的线程个数（单位个，默认0,代表不开启服务） */
      "workThreadCount": 16
    }    
  },

  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*"
}

```


## 4.运行服务中心
如果是在windows环境中，可以直接下载服务中心程序然后运行。
下载[服务中心程序文件](https://serset.github.io/file/Sers/Sers2.1.1.250/SersPublish2.1.1.250.zip)，解压，双击文件夹中的批处理文件“01 ServiceCenter.bat”即可。
>服务中心是用net core编写的，请先安装[netcore2.1运行环境](https://serset.github.io/?md/解析Sers微服务/0.1windows安装netcore2.1运行环境.md)。

控制台有如下类似输出则代表服务中心启动成功。
```
[INFO][14:57:33.9310][WebHost]will listening on: http://*:4580
[INFO][14:57:33.9323][WebHost]wwwroot : ......\wwwroot
Hosting environment: Production
Content root path: ......\Sers2.1.1.250\ServiceCenter
Now listening on: http://[::]:4580
Application started. Press Ctrl+C to shut down.
```

 ## 5.运行程序
 运行程序，在服务中心的控制台看到如下输出则代表服务接入成功
 
 ```
[INFO][15:32:51.3471][CL] OnConnected,connTag:
[INFO][15:32:51.5732][ApiCenterService]Regist serviceStation,stationName:
[INFO][15:32:51.5744][ApiCenterService]Add ApiNode,serviceKey:/api/Values/*_DELETE
[INFO][15:32:51.5820][ApiCenterService]Add ApiNode,serviceKey:/api/Values_GET
[INFO][15:32:51.5832][ApiCenterService]Add ApiNode,serviceKey:/api/Values/*_GET
[INFO][15:32:51.5855][ApiCenterService]Add ApiNode,serviceKey:/api/Values_POST
[INFO][15:32:51.5870][ApiCenterService]Add ApiNode,serviceKey:/api/Values/*_PUT
```
 
 打开地址 <http://localhost:4580/api/Values>,得到如下返回：
 ```json
["value1","value2"]
```
 说明我们的api注册到服务中心，并被成功调用了。
 









 