# Sers Introduction
Sers was a set of cross-platform and cross-language open source micro-service architecture protocol  
>source address: [https://github.com/serset/Sers](https://github.com/serset/Sers "https://github.com/serset/Sers")  
>the current version is 2.1, requestQueue mode.  
  
Sers has the following features:  
- Cross-language, cross-platform, currently supports C #, Java, C ++, JavaScript  
- High efficiency and concurrency (millions of concurrency), QPS:2 million  
- Lightweight and simple, JavaScript access, less than 1000 lines of code, only 8KB after compression  
- Easy to expand, can expand access  
- Support IOCP, ZMQ, WebSocket, NamedPipe, SharedMemory and other communication methods  
- No code intrusion, NET Core only takes 1 line of code to join the service center  



# Station classification
 Sers was a centralized micro-service architecture protocol, which was divided into service center and service station according to identity.

## (x.1)Service Center
ServiceCenter provides services such as service registration, service discovery, request distribution (load balancing), API management, service station management, message subscription, etc.

All service sites need to register with this service center. All requests are forwarded through the service center.

The service center has built-in GOVER service governance function. Provide service management monitoring, station management monitoring, service limit flow, service statistics and other functions. Service governance is deployed in the service center.

The service governance entry address is http://ip:6022/_gover_/index.html

The port number is configured in the appsettings.json configuration file.


## (x.2)Service Station
Service Station provides application layer services.

Service station can have multiple, connected to each other through a service center. When the service station is started, it initiates a service registration request to the service center to register the service.

Once the service station is registered successfully, it can provide services to other stations (including the service gateway). Services provided by other stations can be invoked.

The service provided is identified by a URL (Route).

The service station can be attached directly to the service center (dispensing with the communication layer, monomer mode) to provide services. The performance data of 2 million QPS was measured in this mode.

## (x.3)Service Gateway
The Service Gateway exposes internal services to the outside world over HTTP.

A service gateway is a special service station. The gateway listens for the request using HTTP and forwards the request to the service center. The service gateway is the external gateway to the service.

The service center has a built-in gateway that can be configured to enable in the AppSettings. Json configuration file.

There are two versions of the Gateway, the C ++ version and the dotnet version. The C ++ version (CGateway) is relatively efficient.

