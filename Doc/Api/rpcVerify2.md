# rpcVerify2
 
{"type":"Switch", "body":[  

{"condition":{"type":"not in","path":"caller.source"  ,  "value":["Internal","Outside"]  },    "value": {"type":"_", ssError}  } ,
{"condition":{"type":"!=","path":"caller.source"  ,  "value":"Internal"  },    "value": {"type":"_", ssError}  } 

]   }   


SsExp 目前支持以下几种表达式


    //path 可不指定

    {"type":"SsExp",  "path":"a.b"  ,"value":SsExp  }

    {"type":"Cur",  "path":"a.b"  }  //返回当前path索引的值
    
    {"type":"Value", "value":Value  } //返回Value

    {"type":"_", ...  }  //直接返回当前表达式 

    {"type":"Switch", "path":"a.b"  ,   "body":[  {"condition":SsExp,"value":SsExp } ,...   ] , "default":SsExp   }


    以下为bool表达式,默认返回值为bool类型，也可手动指定满足条件时的返回值
    均存在如下属性：
    resultWhenNull   存在null值时的返回值，默认false
    resultWhenTrue   结果为true时的返回值，默认为true
    resultWhenFalse 结果为false时的返回值，默认为false

   
    {"type":"If", "path":"a.b"  ,"condition":SsExp }                 

    {"type":"And",    "path":"a.b",    "value":[  SsExp1,SsExp2 ...        ]   }

    {"type":"Or",    "path":"a.b",    "value":[  SsExp1,SsExp2 ...        ]   }

    {"type":"Not",    "path":"a.b",    "value":SsExp    }

    {"type":"NotNull",    "path":"a.b",    "value":SsExp    }

    { "type":"Regex",    "path":"a.b",    "value":SsExp  }  //正则匹配

    {"type":"==",    "path":"a.b",    "value":SsExp  }

    {"type":"!=",    "path":"a.b",    "value":SsExp  }

    {"type":">",    "path":"a.b",   "value":SsExp   }
						     
    {"type":">=",    "path":"a.b",    "value":SsExp    }
						     
    {"type":"<",    "path":"a.b",    "value":SsExp   }
						     
    {"type":"<=",    "path":"a.b",    "value":SsExp    }

    {"type":"in",    "path":"a.b",    "value":SsExp  }   //value 值必须为数组,否则返回 false

    {"type":"not in",    "path":"a.b",    "value":SsExp }   //value 值必须为数组,否则返回 true