# Sers���
SersΪһ�׿�ƽ̨�����ԵĿ�Դ΢����ܹ�Э��
>Դ���ַ: [https://github.com/serset/Sers](https://github.com/serset/Sers "https://github.com/serset/Sers")  
>��ǰ�汾Ϊ2.1��RequestQueueģʽ��  

![](https://img.shields.io/github/license/Serset/Sers.svg)  
![](https://img.shields.io/github/repo-size/Serset/Sers.svg)  ![](https://img.shields.io/github/last-commit/Serset/Sers.svg)  
 

| Build | NuGet | Docker |
| -------- | -------- | -------- |
|![](https://github.com/serset/Sers/workflows/ki_devops3_build/badge.svg) | [![](https://img.shields.io/nuget/v/Sers.ServiceStation.svg)](https://www.nuget.org/packages/Sers.ServiceStation) ![](https://img.shields.io/nuget/dt/Sers.ServiceStation.svg)   | ![](https://img.shields.io/docker/v/serset/sers/latest.svg) ![](https://img.shields.io/docker/pulls/serset/sers.svg) ![](https://img.shields.io/docker/image-size/serset/sers/latest.svg) |

Sersӵ���������ԣ�  
- �����ԡ���ƽ̨��Ŀǰ��֧��c#��java��c++��javascript  
- ��Ч�߲��������򲢷���������QPS:2000000  
- ������࣬��javascript���룬���벻��1000�У�ѹ����ֻ��8KB  
- ����չ������������չ����  
- ֧��IOCP��ZMQ��WebSocket��NamedPipe��SharedMemory�ȶ���ͨѶ��ʽ  
- �޴������룬.net core�����������ֻ��Ҫ1�д���  



# վ�㻮��
 SersΪ���Ļ���΢����ܹ�Э�飬������ݷ�Ϊ�������ĺͷ���վ�㡣

## (x.1)��������
������������(ServiceCenter)�ṩ����ע�ᡢ�����֡�����ַ������ؾ��⣩��Api��������վ�������Ϣ���ĵȵȷ���  
�������з���վ�㶼��Ҫ��˷������Ľ���ע�ᡣ���е����󶼻ᾭ���������Ľ���ת����  
����������������Gover���������ܡ��ṩ��������أ�վ������أ���������������ͳ�Ƶȹ��ܡ������������ڷ������ġ�  
���������������ڵ�ַΪ��http://ip:6022/_gover_/index.html  
�����˿ں���appsettings.json�����ļ������á�  


## (x.2)����վ��
��������վ��(ServiceStation)�ṩӦ�ò����  
��������վ������ж����ͨ�����������໥���ӡ��ڷ���վ������ʱ��������������ķ������ע������ע�����  
��������վ��ע��ɹ��󼴿�������վ�㣨�����������أ��ṩ���񡣿ɵ�������վ���ṩ�ķ���  
�����ṩ�ķ�����url(route)��Ϊ�����ʶ��  
�������԰ѷ���վ��ֱ�Ӹ��ӵ��������ģ����ͨ�Ų㣬����ģʽ���ṩ����200����qps���������ݾ����ڴ�ģʽ�¼�����á�  

## (x.3)��������
������������(Gateway)ͨ��http��ʽ���Ⱪ¶�ڲ�����  
��������������һ������ķ���վ�㡣������http�������󣬰�����ת�����������ġ���������Ϊ����Ķ�����ڡ�  
���������������������أ�������appsettings.json�����ļ������ý������á�  
���������������汾��c++�棨CGateway����dotnet��(Gateway)�� c++�棨CGateway����Ը���Ч��  
