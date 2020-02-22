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

{"type":"If", "path":"a.b"  ,"condition":SsExp,  "valueWhenTrue":SsExp,  "valueWhenFalse":SsExp }

{"type":"Switch", "path":"a.b"  ,   "body":[  {"condition":SsExp,"value":SsExp } ,...   ] , "default":SsExp   }

{"type":"And",    "path":"a.b",    "value":[  SsExp1,SsExp2 ...        ]   }

{"type":"Or",    "path":"a.b",    "value":[  SsExp1,SsExp2 ...        ]   }

{"type":"Not",    "path":"a.b",    "value":SsExp    }

{"type":"NotNull",    "path":"a.b",    "value":SsExp    }

{ "type":"Regex",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }  //正则匹配

{"type":"==",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }

{"type":"!=",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }

{"type":">",    "path":"a.b",   "value":SsExp,  "resultWhenNull":false    }

{"type":">=",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }

{"type":"<",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false   }

{"type":"<=",    "path":"a.b",    "value":SsExp,  "resultWhenNull":false    }

{"type":"in",    "path":"a.b",    "value":SsExp  }   //value 值必须为数组

{"type":"not in",    "path":"a.b",    "value":SsExp }   //value 值必须为数组