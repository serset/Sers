# ApiMessage
>底层为SersFile

## 第一个文件 为 RpcContextData
>json 字符串存储 见RpcContextData.md
 

## 第二个文件 为 value
>json 字符串存储,亦可为byte[],没有限制
若为Request 则为 ArgValue
若为Reply则 为 ReturnValue






# SersFile
>SersFile是特殊的二进制文件(byte[])
SersFile消息帧存储一组文件，每个文件都为文件帧格式


## 文件帧
>一个文件帧存储一个文件
              第1部分 FileLen          4 byte,  Int32类型 文件长度
              第2部分 FileContent      文件内容，长度为FileLen