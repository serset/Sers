/*
* sers.ssApiDescToMd 扩展
* Date  : 2019-02-19
* author:lith 

 
 
 */



; (function (scope) {

    var objName = 'ssModel';

    if (scope[objName]) return;


    var obj = scope[objName] = {};
     

    function getExampleBySsModel(ssModel) {
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

*/
        if(!ssModel.mode || !ssModel.type) return null;
        //if (!ssModel.models || ssModel.models.length == 0) return {};

        //(x.1) mode 为 value
        if (ssModel.mode == 'value')
        {
            return parseValue(ssModel.type, ssModel.example);
        }

        //(x.2)指定了example，直接返回
        try {
            if ('string' == typeof (ssModel.example) && ssModel.example != '') {
                return eval('(' + ssModel.example + ')');                 
            }
        } catch (e) {

        }


        var typeMap = {};

        for (var t in ssModel.models) {
            var m = ssModel.models[t];           
            typeMap[m.type] = m;
        }


        var typepath = ['', ssModel.type];
        var modelEntity = typeMap[ssModel.type];
        if (!modelEntity) {
            return null;
        }

        return buildModelEntity(modelEntity);


        function buildModelEntity(modelEntity) {           

            var model = {};

            var propertys = modelEntity.propertys;
            for (var t in propertys) {
                var property = propertys[t];

                var key = property.name;
                var value = getExampleValueByModelProperty(property);                

                model[key] = value;
            }
            return model;
        }

        // mode 必须为 "value"
        function parseValue(type, example) {           
            var value = example;
            try {
                if (type == 'int32' || type == 'int64') {
                    value = parseInt(example);
                } else if (type == 'float' || type == 'double') {
                    value = parseFloat(example);
                } else if (type == 'bool') {                    
                    value = Boolean(example);
                }  
            } catch (e) {
            }
            return value;
        }

        function getExampleValueByModelProperty(property) {

            var value = null;
            //var example = property.example || property.defaultValue;
            var example = property.example;

            //子模型
            if (property.mode == 'value') {        
                value = parseValue(property.type, example);                 
                return value;
            }

            //指定了example，直接返回
            try {
                if ('string' == typeof (example) && example != '') {
                    value = eval('(' + example + ')');
                    return value;
                }

            } catch (e) {

            }

            

            if (property.mode == 'object') {

                var type = property.type;
                var childEntity = typeMap[type];

                if (childEntity) {
                    if (typepath.indexOf(property.type) < 0) {     
                        typepath.push(property.type);
                        value = buildModelEntity(childEntity);       
                        typepath.pop();
                    }                    
                }

            } else if (property.mode == 'array') {

                var type = property.type;
                var childEntity = typeMap[type];
               
                if (childEntity) {
                    typepath.push(property.type);
                    try {
                        var property = childEntity.propertys[0];                       
                        value = getExampleValueByModelProperty(property);                        
                    } catch (e) {
                    }
                    typepath.pop();
                }
                value = [value];
            }

            
            return value;
        }

    }


    obj.getExampleBySsModel = getExampleBySsModel;


})('undefined' != typeof (sers) ? sers : (sers = {}));
