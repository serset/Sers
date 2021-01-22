# Ioc Populate

## appsettings.json demo

``` json
//appsettings.json demo
{"Ioc": {
    "Services": [
      {
        /* 生命周期。可为 Scoped、Singleton、Transient。默认Scoped */
        "Lifetime": "Scoped",
        "Service": "Cy.NetCore.Common.Interfaces.IService,Cy.NetCore.Common.Interfaces"
      },
      {
        /* 生命周期。可为 Scoped、Singleton、Transient。默认Scoped */
        "Lifetime": "Scoped",
        "Service": "Cy.NetCore.Common.Interfaces.IService,Cy.NetCore.Common.Interfaces",
        "Implementation": "Cy.NetCore.Common.DataBase.ServiceImpl.ServiceA,Cy.NetCore.Common.DataBase"
      },
      {
        /* 生命周期。可为 Scoped、Singleton、Transient。默认Scoped */
        "Lifetime": "Scoped",
        "Service": {

          /* 在此Assembly文件中查找类(如 Vit.Core.dll)(assemblyFile、assemblyName 指定任一即可) */
          "assemblyFile": "Did.SersLoader.Demo.dll",

          /* 在此Assembly中查找类(如 Vit.Core)(assemblyFile、assemblyName 指定任一即可) */
          //"assemblyName": "Did.SersLoader.Demo",

          /* 动态加载的类名 */
          "className": "Bearer"

        },
        "Implementation": {

          /* 在此Assembly文件中查找类(如 Vit.Core.dll)(assemblyFile、assemblyName 指定任一即可) */
          "assemblyFile": "Did.SersLoader.Demo.dll",

          /* 在此Assembly中查找类(如 Vit.Core)(assemblyFile、assemblyName 指定任一即可) */
          //"assemblyName": "Did.SersLoader.Demo",

          /* 动态加载的类名 */
          "className": "Bearer",

          "Invoke": [
            {
              "Name": "fieldName",
              "Value": "lith"
            },
            {
              "Name": "prpertyName",
              "Value": 12
            },
            {
              "Name": "methodName",
              "Params": [ 1, "12" ]
            }
          ]
        }
      }
    ]
  }
}
```