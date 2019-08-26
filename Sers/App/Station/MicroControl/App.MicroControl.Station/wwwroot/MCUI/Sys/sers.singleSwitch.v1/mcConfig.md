
{
    mcKey: "201922200",
    mcUiSrc: "/MicroControl/MCUI/Sys/sers.singleSwitch.v1/index.html",

    mcUiInitData: {
                    
	    //"mcKey": "201922200",
        "deviceName": "win10",
        "desc": "关闭电脑，慎用",

		
          "func": [
            {
				"type": "state",
				"title": "hostname",
				"api": "/Mc_Computer/ShellWithReturn",
				"argument": {
					"fileName": "hostname",
					"arguments": ""
				}
            },
            {
				"type": "button",
				"title": "获取系统名称",
				"api": "/Mc_Computer/ShellWithReturn",
				"argument": {
					"fileName": "hostname",
					"arguments": ""
				}
            },
			{
				"type": "button",
				"title": "ping百度",
				"api": "/Mc_Computer/ShellWithReturn",
				"argument": {
					"fileName": "ping",
					"arguments": "www.baidu.com"
				},
				"onSuc":"function(apiRet){ alert(apiRet.data);  }"
            },
			{			
				"type": "button",
				"title": "重启",
				"api": "/Mc_Computer/Shell",
				"argument": {
					"fileName": "shutdown",
					"arguments": "-r -t 10"
				}
            }

          ]	     
	}
}
