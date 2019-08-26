/*
* sers.apiClient 扩展
* sers.apiClient
* Date  : 2019-02-19
* author:lith 

    <script type="text/javascript" src="lith.import.js"></script>

    <script type="text/javascript" >
         sers.apiClient.get({
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
 

    var objName = 'robot';
    var obj = scope[objName] || (scope[objName] = {});

    var ssConfig = {
        apiHost: location.hostname + ':' + location.port //"127.0.0.1:6000"
    };
    
    obj.task_getAll = function (onSuc) {
        $.get('http://' + ssConfig.apiHost + '/_robot_/task/getAll', onSuc);
    };

    obj.task_add = function (config,onSuc) {
        $.post('http://' + ssConfig.apiHost + '/_robot_/task/add', config, onSuc);
    };

    obj.task_start = function (id, onSuc) {
        $.get('http://' + ssConfig.apiHost + '/_robot_/task/start?id='+id, onSuc);
    };

    obj.task_stop = function (id, onSuc) {
        $.get('http://' + ssConfig.apiHost + '/_robot_/task/stop?id=' + id, onSuc);
    };

    obj.task_remove = function (id, onSuc) {
        $.get('http://' + ssConfig.apiHost + '/_robot_/task/remove?id=' + id, onSuc);
    };



})('undefined' != typeof (sers) ? sers : (sers = {}));
