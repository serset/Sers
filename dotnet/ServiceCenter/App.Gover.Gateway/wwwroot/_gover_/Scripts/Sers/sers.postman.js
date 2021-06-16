/*
* sers.postman 扩展
* sers.postman
* Date  : 2019-03-07
* author:lith 
 
 */


; (function (scope) {

    var objName = 'postman';

    if (scope[objName]) return;


    var obj = {};

    scope[objName] = obj;




    obj.collectionConfig = {
        //也可为 GET POST 等
        method: 'GET',
        fileName: 'Sers.postman_collection(' + new Date().toJSON().slice(0,10) + ').json',
        value: {
            "info": {
                "_postman_id": "a791d304-0ff1-4726-b196-e0943980ab70",
                "name": "Sers",
                "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
            }
        }
    };

    obj.environmentConfig = {
        fileName: 'Sers.postman_environment_localhost.json',
        value: {
            "id": "5597c8d1-509f-4c24-beba-16d1b7a85442",
            "name": "Sers-localhost",
            "values": [
                {
                    "key": "ApiHost",
                    //"value": "localhost:4580",
                    "value": location.host,
                    "description": "",
                    "enabled": true
                },
                {
                    "key": "At",
                    "value": "",
                    "description": "",
                    "enabled": true
                }
            ],
            "_postman_variable_scope": "environment",
            "_postman_exported_at": "2019-03-08T02:24:57.105Z",
            //"_postman_exported_at": new Date().toJSON(),
            "_postman_exported_using": "Postman/6.7.4"
        }
    };






    //apiDescs 为apiDesc数组
    function apiDescToPostmanConfig(apiDescs) {

        /*
         apiStations
        {   StationName1:[  {apiDesc1} ,{apiDesc2}  ]
            ,StationName2:[  {apiDesc3} ,{apiDesc4}  ]
        }



         */
        //(x.1) 构建apiStations
        var apiStations = {};
        for (var t in apiDescs) {
            var apiDesc = apiDescs[t];
            if (!apiDesc) continue;
            var route = apiDesc.route;

            try {
                var stationName = route.split('/')[1];
            } catch (e) {
            }
            if (!stationName) continue;


            var station = apiStations[stationName];
            if (!station) {
                station = apiStations[stationName] = [];
            }
            station.push(apiDesc);
        }

        //(x.2) 构建postman config
        var config =
        {
            "info": obj.collectionConfig.value.info,
            "item": []
        };
        var root = config.item;

        //(x.3) 构建postmanItem
        for (var stationName in apiStations) {
            var rootItem = apiStationToPostmanItem(apiStations[stationName], stationName);
            root.push(rootItem);
        }
        
        //(x.4) 排序
        root.sort(function (a, b) { return a.name < b.name ? -1 : 1; });

        return config;
    }


    function apiStationToPostmanItem(apiDescs, stationName) {
        var rootItem = {
            "name": stationName,
            "item": []
        };
        var item = rootItem.item;

        apiDescs.sort(function (a, b) { return a.route < b.route ? -1 : 1; });
        for (var i in apiDescs) {

            try {
                var apiDesc = apiDescs[i];
                item.push(apiDescToPostmanItem(apiDesc, i));
            } catch (e) {
                console.log(e);
            }
        }
        return rootItem;
    };

    function objToString(obj) {
        if (obj == null || typeof (obj) == 'undefined') return null;
        if ( typeof (obj) == 'string') return obj;
        return JSON.stringify(obj, null, 2);
    }




    function apiDescToPostmanItem(apiDesc, index) {
        var route = apiDesc.route.slice(1);

        var method = obj.collectionConfig.method;
        try {
            method = apiDesc.extendConfig.httpMethod;
        } catch (e) {
        }

        var item = {
            "name": "x." + (1 + parseInt(index)) + " " + route + " " + apiDesc.name,
            "protocolProfileBehavior": {
                "disableBodyPruning": true
            },
            "request": {
                "method": method,
                "header": [
                    {
                        "key": "Authorization",
                        "value": "Bearer {{At}}",
                        "type": "text"
                    },
                    {
                        "key": "Content-Type",
                        "value": "application/json",
                        "type": "text"
                    }
                ],
                "body": {
                    "mode": "raw",
                    //"raw": "{\n\t\"arg1\":\"valu1\",\n\t\"arg2\":{\"arg2_1\":\"value2_1\"}\n}"
                    "raw": objToString(sers.ssModel.getExampleBySsModel(apiDesc['argType']))
                },
                "url": {
                    "raw": "http://{{ApiHost}}/" + route,
                    "protocol": "http",
                    "host": [
                        "{{ApiHost}}"
                    ]
                    //,
                    //"path": [
                    //    "station1",
                    //    "api1"
                    //]
                    , "path": route.split('/')
                }
            },
            "response": []
        };

        return item;
    }


    obj.apiDescToPostmanConfig = apiDescToPostmanConfig;


})('undefined' != typeof (sers) ? sers : (sers = {}));


;/* 
 
    SsApiDesc:
     {
       "catagory":"ApiStation1/ss/ccc"
       ,"description":"获取用户信息"

       //路由
       ,"route":"ApiStation1/path1/path2/api1.html"

       //请求参数类型   SsModel类型
       ,"argType":  { SsModel }

       //返回数据类型   SsModel类型
       ,"returnType":  { SsModel }
    }



postman config
 //PostManDemo.postman_collection.json
 {
	"info": {
		"_postman_id": "a791d304-0ff1-4726-b196-e0943980ab70",
		"name": "PostManDemo",
		"schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
	},
	"item": [
		{
			"name": "fold1aaa",
			"item": [
				{
					"name": "fold1/api",
					"request": {
						"method": "POST",
						"header": [
							{
								"key": "Authorization",
								"value": "Bearer *iDB49A7S_APP_LINUX",
								"type": "text"
							},
							{
								"key": "Content-Type",
								"value": "application/json",
								"type": "text"
							}
						],
						"body": {
							"mode": "raw",
							"raw": "{\n\t\"arg1\":\"valu1\",\n\t\"arg2\":{\"arg2_1\":\"value2_1\"}\n}"
						},
						"url": {
							"raw": "http://{{apiHost}}/station1/api1",
							"protocol": "http",
							"host": [
								"{{apiHost}}"
							],
							"path": [
								"station1",
								"api1"
							]
						}
					},
					"response": []
				}
			]
		}
	]
} 
 
 
 
 */;


