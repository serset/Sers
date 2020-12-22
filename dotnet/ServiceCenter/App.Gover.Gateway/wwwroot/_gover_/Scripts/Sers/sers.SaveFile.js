/*
* sers.SaveFile 扩展
* sers.SaveFile
* Date  : 2019-03-07
* author:lith 

    <script type="text/javascript" src="sers.SaveFile.js"></script>

    <script type="text/javascript" >
       sers.SaveFile('{}','a.js');
    
    </script>
 */



; (function (scope) {

    var objName = 'SaveFile'; 


    function SaveFile(content, filename) {    // 创建隐藏的可***链接
        var eleLink = document.createElement('a');
        eleLink.download = filename;
        eleLink.style.display = 'none';    // 字符内容转变成blob地址
        var blob = new Blob([content]);
        eleLink.href = URL.createObjectURL(blob);    // 触发点击
        document.body.appendChild(eleLink);
        eleLink.click();    // 然后移除
        document.body.removeChild(eleLink);
    };


    scope[objName] = SaveFile;



})('undefined' != typeof (sers) ? sers : (sers = {}));


