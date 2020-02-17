## 1. ApiStation
> api站点。以"_"开始的Api站点为系统站点，不受服务限流等限制。


## 2. ServiceStation
> 对应一个部署的服务站点。一个服务站点可能包含多个Api站点的api节点


## 3. ApiService
> 一个指定路由(如"/_sys_/serviceStation/regist")对应的api服务
 


## 4. ApiNode
> 一个api服务可能在多台服务站点上部署，在某一个服务站点上的api服务就是api节点(ApiNode)


## 5. ApiCenter
> 用来管理 api站点 服务站点 api服务 api节点 等