# SsValidation
{"path":"user.userType","ssError":{}, "ssValid":{"type":"Equal","value":"Logined"} }


ssValid目前支持以下验证

x.1
{ "type":"Equal","value":"Logined"}

x.2
{ "type":"Regex","value":"^\\d{11}$"}

x.3
{ "type":"Required" }

x.4
{ "type":"Null"}   always true

x.5
{ "type":"NotEqual","value":"Logined"}

x.6
{ "type":"Scope","min":10.8,"max":12.5}  //包含最大值 最小值，可只指定最大值或最小值