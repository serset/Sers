/*
* sers.ssApiDescToMd 扩展
* Date  : 2019-02-19
* author:lith 

 
 
 */



; (function (scope) {

    var objName = 'ssApiDescsToMd';

    if (scope[objName]) return;


    



 
    function mdEncode(str) { return (''+(str || '')).replace('_', '\\_'); }


    //apiDescs 为apiDesc数组
     function ssApiDescsToMd(apiDescs) {

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

	   //(x.2) 排序
	   var stationArr=[];
	   for (var stationName in apiStations) {
            stationArr.push({stationName:stationName,station:apiStations[stationName]});
        }
	   stationArr.sort(function (a, b) { return a.stationName < b.stationName ? -1 : 1; });

        

        //(x.3) 构建md
        var md = '';

        for (var t in stationArr) {
        	  var item=stationArr[t];
            md += '\n\n----\n\n## ' + mdEncode(item.stationName);
            md += apiStationToMd(item.station);
        }
        return md;
    }


    /*
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


    md:

    ----
    ### (x.1) ApiStation1/path1/path2/api1 获取用户信息
    route:  ** ApiStation1/path1/path2/api1 **
    >获取用户信息

    请求参数: SsModel
    返回数据: SsModel
     */

    var apiStationToMd = function (apiDescs) {
        var mdModel = '\n\n### (x.{{no}}) {{counter}} {{route}} {{name}}\n\n\
> {{description}}\n\n\
name: {{name}}\n\n\
method: {{httpMethod}}\n\n\
route:  ** {{route}} **\n\n\
系统描述: {{sysDesc}}\n\n\
请求参数: {{Arg_SsModel}}\n\n\
请求参数demo: \n\n\
```javascript\n\
{{Arg_Example}}\n\
```\n\
返回数据: {{Return_SsModel}}\n\n\
返回数据demo: \n\n\
```javascript\n\
{{Return_Example}}\n\
```\n----\n\n\
';

        var md = '';

        apiDescs.sort(function (a, b) { return a.route < b.route ? -1 : 1; });
        for (var i in apiDescs) {
            var apiDesc = apiDescs[i];
            var value;
            //构建apiDesc

            var item = mdModel;
            for (var key in apiDesc) {
                value = apiDesc[key];
                if ('string' == typeof (value)) {
                    value = mdEncode(value);
                    item = item.replace(new RegExp('{{' + key + '}}', 'g'), value);
                }
            }

            //item = item.replace(/{{route}}/g, apiDesc.route);
            //item = item.replace(/{{description}}/g, apiDesc.description);
            item = item.replace(/{{no}}/g, 1 + parseInt(i));

            var example;
            //Arg
            item = item.replace(/{{Arg_SsModel}}/g, ssModelToMd(apiDesc['argType']));
            example = sers.ssModel.getExampleBySsModel(apiDesc['argType']);
            if (example) {
                example = JSON.stringify(example, null, 2);
            } else {
                example = '';
            }
            item = item.replace(/{{Arg_Example}}/g, example);

            //Return
            item = item.replace(/{{Return_SsModel}}/g, ssModelToMd(apiDesc['returnType']));
            example = sers.ssModel.getExampleBySsModel(apiDesc['returnType']);
            example = JSON.stringify(example,null,2);
            item = item.replace(/{{Return_Example}}/g, example);

            //counter
            // "ext": { "counter": { "sumCount": 0, "errorCount": 0 } }
            var counter = '';
            if (apiDesc.ext && apiDesc.ext.counter) {
                counter = '\[' + apiDesc.ext.counter.sumCount + '\/' + apiDesc.ext.counter.errorCount +'\]';
            }
            item = item.replace(/{{counter}}/g, counter);

            //httpMethod
            try {
                value = 'all';
                value = apiDesc.extendConfig.httpMethod;
            } catch (e) {
            }           
            value = mdEncode(value);
            item = item.replace(/{{httpMethod}}/g, value);


            //系统描述 sysDesc
            try {
                value = '';
                value = apiDesc.extendConfig.sysDesc;
            } catch (e) {
            }
            value = mdEncode(value);
            item = item.replace(/{{sysDesc}}/g, value);


            //清空其他参数
            item = item.replace(/{{\S*}}/g, '');
            md += item;
        }

        return md;
    };




    function ssModelToMd(ssModel) {
        /*
//SsModel
{
	"type":"type1",
	"mode":"object",
	"description":"用户手机号"
	"defaultValue":"",
	"example":"15000000000",
	"models":[ {SsModelEntity1} , {SsModelEntity2}  ]
}

//SsModelEntity
{
	"type":"type1",
	"mode":"object",
	"propertys":[ {SsModelProperty} , {SsModelProperty}  ]
}

//SsModelProperty
{
	"name":"mobile",
	"type":"type1",
	"mode":"object",
	"description":"用户手机号",
	"defaultValue":"",
	"example":"15000000000"
}



md:

名称|类型|说明|demo|默认值
--|--|--|--|--
├─ success|bool|成功或失败标记||
├─ status|ApiStatsResult|API执行状态||
├─ data|LoginResponse|登录返回数据||
│ ├─ accessToken|string|访问令牌||
│ │ └─ accessToken|string|访问令牌||
│ └─ accessToken|string|访问令牌||
└─ accessToken|string|访问令牌||

         */
        
        if (!ssModel || !ssModel.mode || !ssModel.type ) return '无';
        //if (!ssModel.models || ssModel.models.length == 0) return '无';

        var typeMap = {};

        for (var t in ssModel.models) {
            var m = ssModel.models[t];          
            typeMap[m.type] = m;
        }


        var arg = { md: '' };       

        arg.md += '\n\n名称|类型|说明|example|默认值\n--| --| --| --| --\n';         


        var prefixItem = '&#124;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
        var prefixItem_Null = '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';

        

        function buildModelEntity(modelEntity, prefix) {            

            var propertys = modelEntity.propertys;
            for (var t in propertys) {
                var property = propertys[t];

                //子模型
                var type = property.type;
                var childEntity = typeMap[type];

                var line = '';
                var childPrefix = prefix;

                //(x.1) prefix
                line += prefix;

                //(x.2) ├─
                if (t == propertys.length - 1) {
                    line += '└── ';
                    childPrefix += prefixItem_Null;
                } else {
                    line += '├── ';
                    childPrefix += prefixItem;
                }


                //(x.3) 名称
                line += property.name + '|';


                //(x.4) mode|type  | 说明 | example | 默认值

                //(x.4.1)类型                
                if (property.mode == 'value') {
                    type = '<span title="' + property.type + '">' + property.type + '</span>';                    
                } else {
                    // 为 object 或者 array 
                    type = '<span title="' + property.type+'">' + property.mode+'</span>';
                }


                //(x.4.2)
                line += type + '|' + (mdEncode(property.description) || '') + '|' + (mdEncode(property.example) || '') + '|' + (mdEncode(property.defaultValue) || '') + '\n';

                arg.md += line;


                if (childEntity ) {                
                    if (typepath.indexOf(property.type) < 0) {
                        typepath.push(property.type);
                        buildModelEntity(childEntity, childPrefix);
                        typepath.pop();
                    }
                }

            }

        }

        //添加模型信息 实体
        {  

            var line = '';
            
            var prefix = '';
            //(x.1) prefix
            line += prefix;

            //(x.2) ├─
            line += '└── ';


            //(x.3) 名称
            line += '实体' + '|';


            //(x.4) mode|type  | 说明 | example | 默认值

            //(x.4.1)类型                
            if (ssModel.mode == 'value') {
                type = '<span title="' + ssModel.type + '">' + ssModel.type + '</span>';
            } else {
                // 为 object 或者 array 
                type = '<span title="' + ssModel.type + '">' + ssModel.mode + '</span>';
            }


            //(x.4.2)
            line += type + '|' + (mdEncode(ssModel.description) || '') + '|' + (mdEncode(ssModel.example) || '') + '|' + (mdEncode(ssModel.defaultValue) || '') + '\n';

            arg.md += line;
        }

        var typepath = ['', ssModel.type];
        var modelEntity = typeMap[ssModel.type];
        if (modelEntity) {
            buildModelEntity(modelEntity, prefixItem_Null);
            return arg.md;
        } else {          
            return arg.md;
        }
        return '' + ssModel.name;
    }




    scope[objName] = ssApiDescsToMd;


})('undefined' != typeof (sers) ? sers : (sers = {}));
