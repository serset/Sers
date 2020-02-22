
参数项 描述
`
Name | 类型 | 说明  | Demo
 ------------- | ------------- | ------------- | ------------- 
name  | string | 名称 |  mobile
type  | string | 数据类型 | 可为 object、string、int32、int64、float、double、bool、datetime、binary 或 SsModelEntity的name
mode  | string | 数据模式 |  只可为 value、object、array
description | string | 描述 |  用户手机号
valid | array | 限制 | 
defaultValue | string| 默认值 | 15000000000
example | string| 示例值 | 15012345678

SsModelUnionId 生成规则:
	按指定顺序序列化后的字符串  进行md5






# (x.1)SsModel

格式：
```javascript
{ 		 
	"type":"type1",
	"mode":"object",
	//"valid":[ {SsValid1},{SsValid2} ]
	"description":"用户手机号"
	"defaultValue":"",
	"example":"15000000000"
  
	"models":[ {SsModelEntity1} , {SsModelEntity2}  ]
}
```




# (x.2)SsModelEntity
格式：
```javascript
{	 
	/*  数据类型。可以唯一定位到一个模型 */
	"type":"type1",
	"mode":"object",	 
	"propertys":[ {SsModelProperty} , {SsModelProperty}  ]
}
```


demo:
```javascript
{	 
	"type":"LoginArg",	 
	"mode":"object",	 
	"propertys":[
		{
		"type":"string",
		"name":"mobile",
		"description":"用户手机号"
		"example":"15000000000",
		"defaultValue":""
		}
	]
}
```




# (x.3)SsModelProperty.md

格式：
```javascript
{
	"name":"mobile",
	"type":"type1",
	"mode":"object",
	//"valid":[ {SsValid1},{SsValid2} ]
	"description":"用户手机号"
	"defaultValue":"",
	"example":"15000000000"
}
```


Demo:
```javascript
{
	"name":"mobile",
	"type":"string",
	"description":"用户手机号",
	"defaultValue":"15000000000",
	"example":"15012345678",
	"valid":[
		{"type":"MaxValue","value":"99999999999", "errorMessage": "手机号格式不正确,不是11位"},
		{"type":"MinValue","value":"00000000000", "errorMessage": "手机号格式不正确,不是11位"},
		{"type":"Regex","value":"^\\d{11}$", "errorMessage": "手机号格式不正确,不是11位"},
		{"type":"Required","errorMessage": "手机号不能为空"}, 
		{"type":"Equal","value":"15012345678", "errorMessage": "手机号必须为15012345678"},
		//校验长度，可以用于Array,Collection,Map,String 等
		{"type":"Size","min":5,"max":11,"errorMessage": "手机号长度错误"} 
	]
}
```


