# 简介

Gover是一个实现IApiCenterManage接口的ApiCenterManage 服务治理框架




## (1) ServiceStationMng
	├── ServiceStation A
	├── ServiceStation B
	└── ServiceStation C	
		├────────────── 
		|  ServerInfo:   Linux Ubuntu
		|                cpu 内存
		├────────────── 
		├── ApiNode3
		└── ApiNode4 

## (2) ApiStationMng
	├── ApiStation _sys_
	├── ApiStation AuthCenter
	└── ApiStation Demo		 
		├── ApiService1
		├── ApiService2
		└── ApiService3
			├── ApiNode1   ServiceStation A
			├── ApiNode2   ServiceStation B
			├── ApiNode3   ServiceStation C
			└── ApiNode4   ServiceStation C

## (3) ApiLoadBalancingMng
	├── LoadBalancingForApiNode1
	├── LoadBalancingForApiNode2
	└── LoadBalancingForApiNode3
		├── ApiNode1   ServiceStation A
		├── ApiNode2   ServiceStation B
		├── ApiNode3   ServiceStation C
		└── ApiNode4   ServiceStation C

