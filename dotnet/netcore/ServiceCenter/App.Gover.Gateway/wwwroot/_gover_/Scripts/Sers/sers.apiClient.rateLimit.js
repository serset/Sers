/*
* sers.apiClient 扩展
* sers.apiClient
* Date  : 2019-02-19
* author:lith 

    <script type="text/javascript" src="lith.import.js"></script>

    <script type="text/javascript" >
         sers.apiClient.rateLimit.getAll({
                widgetName: 'infoGet.widget'
                , depends: [
                    { type: 'js', src: '/xxx/xx.js' },
                    { type: 'css', src: '/xxx/xx.css' },
                    { type: 'css', content: '.classA{}' }
                ]
                , files: [
                    { type: 'js', src: '/xxx/xx.js' }
                ]
            });
    
    </script>
 */



; (function (scope) {
 
    scope = scope['apiClient'] || (scope['apiClient'] = {});   
 

    var objName = 'rateLimit';
    var obj = scope[objName] || (scope[objName] = {});



    
    obj.getAll = function (onSuc) {
        sers.apiClient.post({ api: '/_gover_/rateLimit/getAll', onSuc: onSuc });
    };


    obj.remove = function (rateLimitKey, onSuc) {
        sers.apiClient.post({ api: '/_gover_/rateLimit/remove', arg: { rateLimitKey: rateLimitKey}, onSuc: onSuc }); 
    };

    obj.add = function (config, onSuc) {
        sers.apiClient.post({ api: '/_gover_/rateLimit/add', arg: config, onSuc: onSuc }); 
    };



})('undefined' != typeof (sers) ? sers : (sers = {}));
