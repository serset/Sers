
# RpcContextData
>json 字符串

json key  | 类型 | 说明 | Demo
 ------------- | ------------- | ------------- | ------------- 
route  | string| 接口路由 |  ApiStation1/path1/path2/api1.html
 | | | 
user| json| 用户 |
user.userInfo |json| 用户详细信息 | {"userId":4515222,"姓名":"张三","性别":"男"}
 | | | 
caller|json|| 
caller.source| string |  调用来源 |  Internal 或 Outside
caller.rid| string |requestId|
caller.callStack| string |调用堆栈| [ ...ppprid,pprid,prid]
 | | | 
http|json|| 
http.url| string |       |  http://www.a.com/ApiStation/path1/path2/api1.html?A=b&c=d
http.method| string |请求类型|   如 POST GET PUT等
http.statusCode| int |HTTP状态码|   如 200、400等
http.protocol| string | |   如 "HTTP/2.0"
http.headers|json| |{"Content-Type":"application/json","Authorization":"Bearer #ssdfdasf"}
 | | | 
error|SsError|若不为null，则说明接口调用出现异常，只定义架构层级错误，如 api不存在，调用超时等| 

```javascript
	//Demo:
    {
        "route": "/DemoStation/v1/api/5/rpc/2",
        "caller": {
            "rid": "8320becee0d945e9ab93de6fdac7627a",
            "callStack": ["xxxx","xxxxxx"],    // parentRequestGuid array
            "source": "Outside"
        },
        "http": {
            "url": "https://127.0.0.1:6000/DemoStation/v1/api/5/rpc/2?a=1",
            "method":"GET",
            "statusCode":400,
            "protocol":"HTTP/2.0",
            "headers": {
                "Cache-Control": "no-cache",
                "Connection": "keep-alive",
                "Content-Type": "application/javascript",
                "Accept": "*/*",
                "Accept-Encoding": "gzip, deflate",
                "Authorization": "bearer",
                "Host": "127.0.0.1:6000",
                "User-Agent": "PostmanRuntime/7.6.0",
                "Postman-Token": "78c5a1cb-764f-4e04-b2ae-514924a40d5a"
            }
        },
	"error":{SsError},
        "user": {"userInfo":{} }
    }
```