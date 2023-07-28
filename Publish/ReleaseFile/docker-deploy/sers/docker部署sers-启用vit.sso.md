# docker部署sers-启用vit.sso



---------------------------------
# 修改配置文件
``` json
      "BeforeCallApi": [
        {
          /* 在此Assembly中加载类 */
          "assemblyFile": "Sers.Core.Module.Api.ApiEvent.BeforeCallApi.JsonWebToken.dll",
          /* 动态加载的类名，必须继承接口 Sers.Core.Module.Api.ApiEvent.BeforeCallApi.IBeforeCallApi */
          "className": "Sers.Core.Module.Api.ApiEvent.BeforeCallApi.JsonWebToken.JsonWebToken",
          //在调用接口前，会获取 rpcData.http.headers.Authorization(格式为 "Bearer xxxxxx")，或cookie中的Authorization， 并把jwt中的Claims信息放到 rpcData.user.userInfo

          // if token is valid, will set rpcData.caller.source to CallerSource
          "CallerSource": "Internal",
          //"issuer": "https://sso.lith.cloud:4",
          //"audiences": [ "http://localhost:4580" ],
          "publicKeysDiscovery_Url": "https://sso.lith.cloud:4/oauth2/v1/discovery/keys"
        }
      ]

```


# 创建容器并运行
``` bash
cd /root/docker

cd sers
docker run --name=sers --restart=always -d \
-p 4580:4580 -p 4501:4501 \
-v /etc/localtime:/etc/localtime \
-v $PWD/appsettings.json:/root/app/appsettings.json \
-v $PWD/AuthService.Env.js:/root/app/wwwroot/_gover_/Scripts/Vit.SSO/AuthService.Env.js \
-v $PWD/Logs:/root/app/Logs \
-v $PWD/Data:/root/app/Data \
serset/sers

```



