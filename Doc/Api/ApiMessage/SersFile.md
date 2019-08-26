# SersFile
>SersFile是特殊的二进制文件(byte[])
SersFile消息帧存储一组文件，每个文件都为文件帧格式


## 文件帧
>一个文件帧存储一个文件
              第1部分 FileLen          4 byte,  Int32类型 文件长度
              第2部分 FileContent      文件内容，长度为FileLen