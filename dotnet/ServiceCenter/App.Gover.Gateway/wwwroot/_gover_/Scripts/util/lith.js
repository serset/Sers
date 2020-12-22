/*
 * lith 扩展  
 * Date   : 2019-08-22
 * Version: 1.6
 * author : Lith
 * email  : sersms@163.com
 <script type="text/javascript" src="lith.js"></script>

 */

/**  扩展 String
 * 说明  : 对String类的prototype和类扩充函数： toJsonStr、toJsStr、toXmlStr、decodeXmlStr、html2Text、isNotStr、trim、lTrim、rTrim
 * Date  : 2017-09-22
 * author: Lith
 */
; (function (String) {

    //String的去除空格函数
    String.prototype.trim = function () { return this.replace(/(^\s*)|(\s*$)/g, ""); };
    String.prototype.lTrim = function () { return this.replace(/(^\s*)/g, ""); };
    String.prototype.rTrim = function () { return this.replace(/(\s*$)/g, ""); };




    String.prototype.toJsonStr = function () {
        /// <summary> 向json键值对数据的字符串型值 转换。 
        /// <para>例如 转换为javascript 代码  var oJson={"name":"ff"};  中json对象的name属性所对应的值（以双引号包围）。</para> 
        /// <para>转换   \b \t \n \f \r \" \\ 为 \\b \\t \\n \\f \\r \\" \\\\</para>
        /// </summary>          
        /// <returns type="string">转换后的字符串</returns>
        return this.replace(/\\/g, "\\\\").replace(/\x08/g, "\\b").replace(/\t/g, "\\t").replace(/\n/g, "\\n").replace(/\f/g, "\\f").replace(/\r/g, "\\r").replace(/\"/g, "\\\"");
    };




    String.prototype.toJsStr = function () {
        /// <summary> 向javascript的字符串转换。
        /// <para>例如转换为javascript 代码  var str="";  中str对象所赋的值（以引号包围）。 </para>   
        /// <para>转换   \b \t \n \f \r \" \' \\ 为 \\b \\t \\n \\f \\r \\" \\' \\\\        </para>
        /// </summary>          
        /// <returns type="string">转换后的字符串</returns>
        return this.replace(/\\/g, "\\\\").replace(/\x08/g, "\\b").replace(/\t/g, "\\t").replace(/\n/g, "\\n").replace(/\f/g, "\\f").replace(/\r/g, "\\r").replace(/\"/g, "\\\"").replace(/\'/g, "\\\'");
    };



    String.prototype.toXmlStr = function () {
        /// <summary> 向xml转换。
        /// <para>例如 转换  &lt;a title=&quot;&quot;&gt;ok&lt;/a&gt;  中a标签的内容体（innerHTML）或 转换  &lt;a title=&quot;&quot;&gt;ok&lt;/a&gt;  中title的值。</para>  
        /// <para>转换     &amp; 双引号 &lt; &gt;     为      &amp;amp; &amp;quot; &amp;lt; &amp;gt;(注： 单引号 对应 &amp;apos; (&amp;#39;) ，但有些浏览器不支持，故此函数不转换。)</para>  
        /// </summary>          
        /// <returns type="string">转换后的字符串</returns>
        return this.replace(/\&/g, "&amp;").replace(/\"/g, "&quot;").replace(/\</g, "&lt;").replace(/\>/g, "&gt;");
    };

    String.prototype.decodeXmlStr = function () {
        /// <summary> xml属性字符串反向转换（与toXmlStr对应）。
        /// <para>例如 反向转换  &lt;a title=&quot;&quot;&gt;ok&lt;/a&gt;  中a标签的内容体（innerHTML）或 转换  &lt;a title=&quot;&quot;&gt;ok&lt;/a&gt;  中title的值。</para>    
        /// <para>转换  &amp;amp;  &amp;quot;  &amp;lt;  &amp;gt; 为 &quot; &amp; &lt; &gt; (注： 单引号 对应 &amp;apos; (&amp;#39;) ，但有些浏览器不支持，故此函数不转换。)</para>
        /// </summary>          
        /// <returns type="string">转换后的字符串</returns>
        return this.replace(/\&amp\;/g, "&").replace(/\&quot\;/g, "\"").replace(/\&lt\;/g, "<").replace(/\&gt\;/g, ">");
    };


    String.prototype.html2Text = function () {
        /// <summary> 清除Html格式。例如 ： 转换   "&lt;br/&gt;aa&lt;p&gt;ssfa&lt;/p&gt;" 为 "aassfa" <summary>          
        /// <returns type="string">转换后的字符串</returns>
        return this.replace(/<[^>].*?>/g, "");
    };



    function isNotStr(str) {
        return null == str || undefined == str;
    }

    String.isNotStr = isNotStr;

    String.trim = function (str) { return isNotStr(str) ? '' : ('' + str).trim(); };
    String.lTrim = function (str) { return isNotStr(str) ? '' : ('' + str).lTrim(); };
    String.rTrim = function (str) { return isNotStr(str) ? '' : ('' + str).rTrim(); };
    String.toJsonStr = function (str) {
        /// <summary> 向json键值对数据的字符串型值 转换。 
        /// <para>例如 转换为javascript 代码  var oJson={"name":"ff"};  中json对象的name属性所对应的值（以双引号包围）。</para> 
        /// <para>转换   \b \t \n \f \r \" \\ 为 \\b \\t \\n \\f \\r \\" \\\\</para>
        /// </summary>          
        /// <returns type="string">转换后的字符串</returns>
        return isNotStr(str) ? '' : ('' + str).toJsonStr();
    };
    String.toJsStr = function (str) {
        /// <summary> 向javascript的字符串转换。
        /// <para>例如转换为javascript 代码  var str="";  中str对象所赋的值（以引号包围）。 </para>   
        /// <para>转换   \b \t \n \f \r \" \' \\ 为 \\b \\t \\n \\f \\r \\" \\' \\\\        </para>
        /// </summary>          
        /// <returns type="string">转换后的字符串</returns>
        return isNotStr(str) ? '' : ('' + str).toJsStr();
    };
    String.toXmlStr = function (str) {
        /// <summary> 向xml转换。
        /// <para>例如 转换  &lt;a title=&quot;&quot;&gt;ok&lt;/a&gt;  中a标签的内容体（innerHTML）或 转换  &lt;a title=&quot;&quot;&gt;ok&lt;/a&gt;  中title的值。</para>  
        /// <para>转换     &amp; 双引号 &lt; &gt;     为      &amp;amp; &amp;quot; &amp;lt; &amp;gt;(注： 单引号 对应 &amp;apos; (&amp;#39;) ，但有些浏览器不支持，故此函数不转换。)</para>  
        /// </summary>          
        /// <returns type="string">转换后的字符串</returns>
        return isNotStr(str) ? '' : ('' + str).toXmlStr();
    };

    String.decodeXmlStr = function (str) {
        /// <summary> xml属性字符串反向转换（与toXmlStr对应）。
        /// <para>例如 反向转换  &lt;a title=&quot;&quot;&gt;ok&lt;/a&gt;  中a标签的内容体（innerHTML）或 转换  &lt;a title=&quot;&quot;&gt;ok&lt;/a&gt;  中title的值。</para>    
        /// <para>转换  &amp;amp;  &amp;quot;  &amp;lt;  &amp;gt; 为 &quot; &amp; &lt; &gt; (注： 单引号 对应 &amp;apos; (&amp;#39;) ，但有些浏览器不支持，故此函数不转换。)</para>
        /// </summary>          
        /// <returns type="string">转换后的字符串</returns>
        return isNotStr(str) ? '' : ('' + str).decodeXmlStr();
    };
    String.html2Text = function (str) {
        /// <summary> 清除Html格式。例如 ： 转换   "&lt;br/&gt;aa&lt;p&gt;ssfa&lt;/p&gt;" 为 "aassfa" <summary>          
        /// <returns type="string">转换后的字符串</returns>
        return isNotStr(str) ? '' : ('' + str).html2Text();
    };



})(String);




/**  对Date的扩展。扩展 Date.pattern
  * 说明  : 将 Date 转化为指定格式的String  月(M)、日(d)、12小时(h)、24小时(H)、分(m)、秒(s)、周(E)、季度(q)
  * demo  : new Date().pattern("yyyy-MM-dd hh:mm:ss.S")  //2006-07-02 08:09:04.423   
  * Date  : 2017-09-22
  * author: Lith
  */
; (function (Date) {
    /*** 对Date的扩展，将 Date 转化为指定格式的String * 月(M)、日(d)、12小时(h)、24小时(H)、分(m)、秒(s)、周(E)、季度(q)
    *     可以用 1-2 个占位符 * 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)
    * eg: 
    * (newDate()).pattern("yyyy-MM-dd hh:mm:ss.S")==> 2006-07-02 08:09:04.423      
    * (new Date()).pattern("yyyy-MM-dd E HH:mm:ss") ==> 2009-03-10 二 20:09:04      
    * (new Date()).pattern("yyyy-MM-dd EE hh:mm:ss") ==> 2009-03-10 周二 08:09:04      
    * (new Date()).pattern("yyyy-MM-dd EEE hh:mm:ss") ==> 2009-03-10 星期二 08:09:04      
    * (new Date()).pattern("yyyy-M-d h:m:s.S") ==> 2006-7-2 8:9:4.18      
    */
    Date.prototype.pattern = function (fmt) {
        var o = {
            "M+": this.getMonth() + 1, //月份         
            "d+": this.getDate(), //日         
            "h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12, //小时         
            "H+": this.getHours(), //小时         
            "m+": this.getMinutes(), //分         
            "s+": this.getSeconds(), //秒         
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度         
            "S": this.getMilliseconds() //毫秒         
        };
        var week = {
            "0": "/u65e5",
            "1": "/u4e00",
            "2": "/u4e8c",
            "3": "/u4e09",
            "4": "/u56db",
            "5": "/u4e94",
            "6": "/u516d"
        };
        if (/(y+)/.test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        }
        if (/(E+)/.test(fmt)) {
            fmt = fmt.replace(RegExp.$1, ((RegExp.$1.length > 1) ? (RegExp.$1.length > 2 ? "/u661f/u671f" : "/u5468") : "") + week[this.getDay() + ""]);
        }
        for (var k in o) {
            if (new RegExp("(" + k + ")").test(fmt)) {
                fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            }
        }
        return fmt;
    }

    Date.prototype.addDay = function (dayCount) {
        /// <summary><summary>          
        /// <param name="dayCount" type="int">偏移的天数,可为负数</param> 
        this.setTime(this.getTime() + dayCount * (1000 * 60 * 60 * 24));
        return this;
    };

    Date.prototype.addSecond = function (secondCount) {
        /// <summary><summary>          
        /// <param name="secondCount" type="int">偏移的秒数,可为负数</param>
        this.setTime(this.getTime() + secondCount * 1000 );
        return this;
    };

    Date.addDay = function (date, dayCount) {
        if ("string" == typeof date) date = new Date(date);
        return date.addDay(dayCount);
    };


    Date.prototype.isWeekend = function () {
        /// <summary><summary>          
        var week = this.getDay();
        if (week == 0 || week == 6) {
            return true;
        }
        return false;
    };

    Date.isWeekend = function (date) {
        if ("string" == typeof date) date = new Date(date);
        return date.isWeekend();
    };


})(Date);



/**  Object.isNullOrEmpty
  * 说明  : 判定指定对象是否为null,或空对象（例如：{} ）
  * demo  : Object.isNullOrEmpty({})  //true
  * author: Lith
  */
Object.isNullOrEmpty = function (obj) {
    /// <summary>判定指定对象是否为null,或空对象（例如：{} ）</summary>
    /// <param name="obj" type="object"></param>
    /// <returns type="bool">返回指定对象是否为null,或空对象（例如：{} ）</returns>
    if (!obj) return true;
    for (var n in obj) { return false; }
    return true;
};




/**  对lith的扩展。
  * Date  : 2017-09-22
  * author: Lith
  */
; (function (scope) {




    /**  扩展 lith.stringify
      * 说明  : 把json(object 或 Array)转换为字符串。参照 /native/json2.js
      * Date  : 2017-09-22
      * author: Lith
      */
    ; (function (scope) {

        var JSON = {};


        (function () {
            'use strict';

            function f(n) {
                // Format integers to have at least two digits.
                return n < 10 ? '0' + n : n;
            }

            if (typeof Date.prototype.toJSON !== 'function') {

                Date.prototype.toJSON = function () {

                    return isFinite(this.valueOf())
                        ? this.getUTCFullYear() + '-' +
                        f(this.getUTCMonth() + 1) + '-' +
                        f(this.getUTCDate()) + 'T' +
                        f(this.getUTCHours()) + ':' +
                        f(this.getUTCMinutes()) + ':' +
                        f(this.getUTCSeconds()) + 'Z'
                        : null;
                };

                String.prototype.toJSON =
                    Number.prototype.toJSON =
                    Boolean.prototype.toJSON = function () {
                        return this.valueOf();
                    };
            }

            var cx,
                escapable,
                gap,
                indent,
                meta,
                rep;


            function quote(string) {

                // If the string contains no control characters, no quote characters, and no
                // backslash characters, then we can safely slap some quotes around it.
                // Otherwise we must also replace the offending characters with safe escape
                // sequences.

                escapable.lastIndex = 0;
                return escapable.test(string) ? '"' + string.replace(escapable, function (a) {
                    var c = meta[a];
                    return typeof c === 'string'
                        ? c
                        : '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
                }) + '"' : '"' + string + '"';
            }


            function str(key, holder) {

                // Produce a string from holder[key].

                var i,          // The loop counter.
                    k,          // The member key.
                    v,          // The member value.
                    length,
                    mind = gap,
                    partial,
                    value = holder[key];

                // If the value has a toJSON method, call it to obtain a replacement value.

                if (value && typeof value === 'object' &&
                    typeof value.toJSON === 'function') {
                    value = value.toJSON(key);
                }

                // If we were called with a replacer function, then call the replacer to
                // obtain a replacement value.

                if (typeof rep === 'function') {
                    value = rep.call(holder, key, value);
                }

                // What happens next depends on the value's type.

                switch (typeof value) {
                    case 'string':
                        return quote(value);

                    case 'number':

                        // JSON numbers must be finite. Encode non-finite numbers as null.

                        return isFinite(value) ? String(value) : 'null';

                    case 'boolean':
                    case 'null':

                        // If the value is a boolean or null, convert it to a string. Note:
                        // typeof null does not produce 'null'. The case is included here in
                        // the remote chance that this gets fixed someday.

                        return String(value);

                    // If the type is 'object', we might be dealing with an object or an array or
                    // null.

                    case 'object':

                        // Due to a specification blunder in ECMAScript, typeof null is 'object',
                        // so watch out for that case.

                        if (!value) {
                            return 'null';
                        }

                        // Make an array to hold the partial results of stringifying this object value.

                        gap += indent;
                        partial = [];

                        // Is the value an array?

                        if (Object.prototype.toString.apply(value) === '[object Array]') {

                            // The value is an array. Stringify every element. Use null as a placeholder
                            // for non-JSON values.

                            length = value.length;
                            for (i = 0; i < length; i += 1) {
                                partial[i] = str(i, value) || 'null';
                            }

                            // Join all of the elements together, separated with commas, and wrap them in
                            // brackets.

                            v = partial.length === 0
                                ? '[]'
                                : gap
                                    ? '[\n' + gap + partial.join(',\n' + gap) + '\n' + mind + ']'
                                    : '[' + partial.join(',') + ']';
                            gap = mind;
                            return v;
                        }

                        // If the replacer is an array, use it to select the members to be stringified.

                        if (rep && typeof rep === 'object') {
                            length = rep.length;
                            for (i = 0; i < length; i += 1) {
                                if (typeof rep[i] === 'string') {
                                    k = rep[i];
                                    v = str(k, value);
                                    if (v) {
                                        partial.push(quote(k) + (gap ? ': ' : ':') + v);
                                    }
                                }
                            }
                        } else {

                            // Otherwise, iterate through all of the keys in the object.

                            for (k in value) {
                                if (Object.prototype.hasOwnProperty.call(value, k)) {
                                    v = str(k, value);
                                    if (v) {
                                        partial.push(quote(k) + (gap ? ': ' : ':') + v);
                                    }
                                }
                            }
                        }

                        // Join all of the member texts together, separated with commas,
                        // and wrap them in braces.

                        v = partial.length === 0
                            ? '{}'
                            : gap
                                ? '{\n' + gap + partial.join(',\n' + gap) + '\n' + mind + '}'
                                : '{' + partial.join(',') + '}';
                        gap = mind;
                        return v;
                }
            }

            // If the JSON object does not yet have a stringify method, give it one.

            if (typeof JSON.stringify !== 'function') {
                escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g;
                meta = {    // table of character substitutions
                    '\b': '\\b',
                    '\t': '\\t',
                    '\n': '\\n',
                    '\f': '\\f',
                    '\r': '\\r',
                    '"': '\\"',
                    '\\': '\\\\'
                };
                JSON.stringify = function (value) {

                    // The stringify method takes a value and an optional replacer, and an optional
                    // space parameter, and returns a JSON text. The replacer can be a function
                    // that can replace values, or an array of strings that will select the keys.
                    // A default replacer method can be provided. Use of the space parameter can
                    // produce text that is more easily readable.
                    var replacer = arguments[1], space = arguments[2];
                    var i;
                    gap = '';
                    indent = '';

                    // If the space parameter is a number, make an indent string containing that
                    // many spaces.

                    if (typeof space === 'number') {
                        for (i = 0; i < space; i += 1) {
                            indent += ' ';
                        }

                        // If the space parameter is a string, it will be used as the indent string.

                    } else if (typeof space === 'string') {
                        indent = space;
                    }

                    // If there is a replacer, it must be a function or an array.
                    // Otherwise, throw an error.

                    rep = replacer;
                    if (replacer && typeof replacer !== 'function' &&
                        (typeof replacer !== 'object' ||
                            typeof replacer.length !== 'number')) {
                        throw new Error('JSON.stringify');
                    }

                    // Make a fake root object containing our value under the key of ''.
                    // Return the result of stringifying the value.

                    return str('', { '': value });
                };
            }


            // If the JSON object does not yet have a parse method, give it one.

            if (typeof JSON.parse !== 'function') {
                cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g;
                JSON.parse = function (text, reviver) {

                    // The parse method takes a text and an optional reviver function, and returns
                    // a JavaScript value if the text is a valid JSON text.

                    var j;

                    function walk(holder, key) {

                        // The walk method is used to recursively walk the resulting structure so
                        // that modifications can be made.

                        var k, v, value = holder[key];
                        if (value && typeof value === 'object') {
                            for (k in value) {
                                if (Object.prototype.hasOwnProperty.call(value, k)) {
                                    v = walk(value, k);
                                    if (v !== undefined) {
                                        value[k] = v;
                                    } else {
                                        delete value[k];
                                    }
                                }
                            }
                        }
                        return reviver.call(holder, key, value);
                    }


                    // Parsing happens in four stages. In the first stage, we replace certain
                    // Unicode characters with escape sequences. JavaScript handles many characters
                    // incorrectly, either silently deleting them, or treating them as line endings.

                    text = String(text);
                    cx.lastIndex = 0;
                    if (cx.test(text)) {
                        text = text.replace(cx, function (a) {
                            return '\\u' +
                                ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
                        });
                    }

                    // In the second stage, we run the text against regular expressions that look
                    // for non-JSON patterns. We are especially concerned with '()' and 'new'
                    // because they can cause invocation, and '=' because it can cause mutation.
                    // But just to be safe, we want to reject all unexpected forms.

                    // We split the second stage into 4 regexp operations in order to work around
                    // crippling inefficiencies in IE's and Safari's regexp engines. First we
                    // replace the JSON backslash pairs with '@' (a non-JSON character). Second, we
                    // replace all simple value tokens with ']' characters. Third, we delete all
                    // open brackets that follow a colon or comma or that begin the text. Finally,
                    // we look to see that the remaining characters are only whitespace or ']' or
                    // ',' or ':' or '{' or '}'. If that is so, then the text is safe for eval.

                    if (/^[\],:{}\s]*$/
                        .test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@')
                            .replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']')
                            .replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {

                        // In the third stage we use the eval function to compile the text into a
                        // JavaScript structure. The '{' operator is subject to a syntactic ambiguity
                        // in JavaScript: it can begin a block or an object literal. We wrap the text
                        // in parens to eliminate the ambiguity.

                        j = eval('(' + text + ')');

                        // In the optional fourth stage, we recursively walk the new structure, passing
                        // each name/value pair to a reviver function for possible transformation.

                        return typeof reviver === 'function'
                            ? walk({ '': j }, '')
                            : j;
                    }

                    // If the text is not JSON parseable, then a SyntaxError is thrown.

                    throw new SyntaxError('JSON.parse');
                };
            }
        }());


        /// <field name='stringify' type='fucntion'>把json转换为字符串</field>
        scope.stringify = JSON.stringify;


    })(scope);


    /**  扩展 lith.stringNson
     * 说明  : 把nson转换为字符串(目前相对json扩充了function类型。值可为function类型)
     * Date  : 2018-08-02
     * author: Lith
     */
    ; (function (scope) {


        // var toJsonStr = String.toJsonStr;
        //   function dateToStr(v) { return v.pattern('"yyyy-MM-dd hh:mm:ss"'); }


        function toJsonStr(v) {
            /// <summary> 向json键值对数据的字符串型值 转换。 
            /// <para>例如 转换为javascript 代码  var oJson={"name":"ff"};  中json对象的name属性所对应的值（以双引号包围）。</para> 
            /// <para>转换   \b \t \n \f \r \" \\ 为 \\b \\t \\n \\f \\r \\" \\\\</para>
            /// </summary>          
            /// <returns type="string">转换后的字符串</returns>
            return v.replace(/\\/g, "\\\\").replace(/\x08/g, "\\b").replace(/\t/g, "\\t").replace(/\n/g, "\\n").replace(/\f/g, "\\f").replace(/\r/g, "\\r").replace(/\"/g, "\\\"");
        }


        function dateToStr(d) {
            function f(n) {
                return n < 10 ? '0' + n : n;
            }
            return d.getUTCFullYear() + '-' +
                f(d.getUTCMonth() + 1) + '-' +
                f(d.getUTCDate()) + ' ' +
                f(d.getUTCHours()) + ':' +
                f(d.getUTCMinutes()) + ':' +
                f(d.getUTCSeconds());
        }





        function arrayToStr(array) {
            var str = '[';
            for (var p in array) str += stringify(array[p]) + ',';
            if (1 == str.length) return '[]';
            return str.slice(0, -1) + ']';
        }
        function objectToStr(object) {
            if (!object) return 'null';
            var str = '{';
            for (var p in object) str += '"' + toJsonStr(p) + '":' + stringify(object[p]) + ',';
            if (1 == str.length) return '{}';
            return str.slice(0, -1) + '}';
        }


        function stringify(v) {
            if ('string' == typeof (v)) return '"' + toJsonStr(v) + '"';
            if ('object' == typeof (v))
                if (Object.prototype.toString.apply(v) === '[object Array]') return arrayToStr(v);
                else if (v instanceof Date) return dateToStr(v);
                else return objectToStr(v);
            return '' + v;
        }
        scope.stringNson = stringify;

    })(scope);







    /**  扩展 lith.math        (lith.math.mod  lith.math.guid)
     * Date  : 2017-01-04
     * author:Lith
     */
    ; (function (scope) {
        var objName = 'math';
        var obj = (scope[objName] || (scope[objName] = {}));

        obj.mod = function (a, b) {
            /// <summary> a对b取模（余数），结果始终为非负数。 mod(-1,10)结果为9 。（b符号无意义，若为0则始终返回0）<summary>          
            /// <param name="a" type="int">sss</param>
            /// <param name="b" type="int">s</param>
            /// <returns type="int"></returns>
            return b == 0 ? 0 : (a < 0 ? (b - ((-a) % b)) : a % b);
        };


        obj.guid = function (len, radix) {
            /// <summary> 
            /// <para> 获取guid。(若不传参数，则为 rfc4122, version 4 form)              </para>
            /// <para> lith.math.guid() //  "3D6F0925-9535-4A30-9F70-EE57B77095B1"       </para>
            /// <para> lith.math.guid(8, 2) //  "11000101"                               </para>          
            /// </summary>  
            /// <param name="len" type="int">可不指定</param>
            /// <param name="radix" type="int">可不指定,默认16</param>
            /// <returns type="int"></returns>


            var chars = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'.split('');
            var uuid = [], i;
            radix = radix || chars.length;

            if (len) {
                // Compact form
                for (i = 0; i < len; i++) uuid[i] = chars[0 | Math.random() * radix];
            } else {
                // rfc4122, version 4 form
                var r;

                // rfc4122 requires these characters
                uuid[8] = uuid[13] = uuid[18] = uuid[23] = '-';
                uuid[14] = '4';

                // Fill in random data.  At i==19 set the high bits of clock sequence as
                // per rfc4122, sec. 4.1.5
                for (i = 0; i < 36; i++) {
                    if (!uuid[i]) {
                        r = 0 | Math.random() * 16;
                        uuid[i] = chars[(i == 19) ? (r & 0x3) | 0x8 : r];
                    }
                }
            }

            return uuid.join('');
        };

    })(scope);






    /**  扩展 EventForHit
     * Date  : 2017-09-22
     * author:Lith
     */
    scope.EventForHit = function () {
        /// <summary>demo: 
        /// <para> var event=new  lith.EventForHit();</para>
        /// <para> event.doAfterHit(func1);</para>
        /// <para> event.onHit();  //会调用 func1 </para>
        /// <para> event.doAfterHit(func2); //会调用 func2 </para>
        /// <para> event.hited=false;</para>
        /// <para> event.doAfterHit(func3);</para>
        /// <para> event.doAfterHit(func4);</para>
        /// <para> event.onHit();  //会调用 func3,func4 </para>
        /// <para>                                  </para>
        /// <para> var event=new  lith.EventForHit();</para>
        /// <para> event.doAfterHit(func1);</para>
        /// <para> event.addHit('a');event.addHit('b');  </para>
        /// <para> event.onHit('a');  </para>
        /// <para> event.onHit('b');  //会调用 func1 </para>
        /// <para>                                  </para>    
        /// <para> event.doAfterHit(func1);</para>
        /// <para> event.clearCallback();  // 会清除func1
        /// <para> event.doAfterHit(func2);</para>
        /// <para> event.onHit();  //会调用 func2 </para>
        /// </summary>
        var cbList;
        this.hited = false;
        this.doAfterHit = function (callback) {
            if (this.hited) {
                callback();
            } else {
                if (cbList) cbList.push(callback);
                else cbList = [callback];
            }
        };

        var hitNames;

        this.addHit = function (hitName) {
            if (!hitNames) hitNames = {};
            hitNames[hitName] = true;
        };

        function Object_isNullOrEmpty(obj) {
            /// <summary>判定指定对象是否为null,或空对象（例如：{} ）</summary>
            /// <param name="obj" type="object"></param>
            /// <returns type="bool">返回指定对象是否为null,或空对象（例如：{} ）</returns>
            if (!obj) return true;
            for (var n in obj) { return false; }
            return true;
        }

        this.onHit = function (hitName) {

            if (hitName) {
                if (hitNames) {
                    delete hitNames[hitName];
                }
            }

            if (!Object_isNullOrEmpty(hitNames)) {
                return;
            }


            this.hited = true;
            if (cbList) {
                for (var t in cbList) {
                    cbList[t]();
                }
                cbList = null;
            }
        };


        this.clearCallback = function () {
            cbList = null;
        };

    };


    /**  扩展 override、virtual
     * Date  : 2017-09-22
     * author:Lith
     */
    ; (function (scope) {


        scope.override = function (object, funcName, func) {
            /// <signature>
            /// <summary>
            /// 用this函数(func)重写对象(object)的base函数(funcName)，base函数作为this函数的参数传入
            /// <para>注：调用base函数的建议方式为 base.apply(this,arguments) 或 base.call(this,p1,p2...)</para>
            /// </summary>
            /// <param name="object" type="object">对象,若不指定(例如null)则参数funcName必须为函数</param>
            /// <param name="funcName" type="string or function">函数 或 在对象(object)中的函数名称</param>
            /// <param name="func" type="function">重写的函数</param>
            /// <param name="baseAsFirstParam" type="bool">true:旧函数作为新函数的第一个参数传入；    其他：旧函数会作为新函数的最后一个参数传入</param>
            /// <param name="times" type="int">调用的次数（若为正数则在调用times次后 再次调用时就调用base函数）(若为数字，会先取整)</param>
            /// <param name="forceBaseToFunc" type="bool">是否强制base参数为函数，若funcName对应的值不为函数，则把base转换为返回funcName对应的值的无参函数</param>
            /// </signature>

            /// <signature>
            /// <summary>
            /// 用this函数(func)重写对象(object)的base函数(funcName)，base函数作为this函数的参数传入
            /// <para>注：调用base函数的建议方式为 base.apply(this,arguments) 或 base.call(this,p1,p2...)</para>
            /// </summary>
            /// <param name="object" type="object">对象,若不指定(例如null)则参数funcName必须为函数</param>
            /// <param name="funcName" type="string or function">函数 或 在对象(object)中的函数名称</param>
            /// <param name="func" type="function">重写的函数</param>
            /// <param name="param" type="object">
            ///         param.baseAsFirstParam[bool]: true:旧函数作为新函数的第一个参数传入；    其他：旧函数会作为新函数的最后一个参数传入 
            ///         param.times[int]: 调用的次数（若为正数则在调用times次后 再次调用时就调用base函数）(若为数字，会先取整) 
            ///         param.forceBaseToFunc[bool]: 是否强制base参数为函数，若funcName对应的值不为函数，则把base转换为返回funcName对应的值的无参函数
            ///     Demo: {},{baseAsFirstParam:true,times:0,forceBaseToFunc:true},{times:1,forceBaseToFunc:true}
            /// </param>
            /// </signature>

            var param = arguments[3];
            if (!param || 'object' != typeof (param)) {
                param = { baseAsFirstParam: param, times: arguments[4], forceBaseToFunc: arguments[5] };
            }

            var baseValue, base, funcCallBase;
            if ('function' == typeof (funcName)) {
                funcCallBase = base = baseValue = funcName;
            } else if (object && 'object' == typeof (object)) {
                if ('function' == typeof (baseValue = object[funcName])) {
                    funcCallBase = base = baseValue;
                } else {
                    base = param.forceBaseToFunc ? (funcCallBase = function () { return baseValue; }) : baseValue;
                }
            } else {
                throw new Error('override参数不合法，没有正确指定base函数');
            }

            if (isNaN(param.times)) param.times = -1;
            else param.times = parseInt(param.times);

            var funcCur = function () {
                var args = Array.prototype.slice.call(arguments);

                if (true == param.baseAsFirstParam) {
                    args.splice(0, 0, base);
                } else {
                    args.push(base);
                }
                var ret = func.apply(this, args);

                if (param.times > 0) {
                    param.times--;
                    if (0 >= param.times) {
                        funcCur = funcCallBase;
                    }
                }
                return ret;
            }
            var funcRet = function () { return funcCur.apply(this, arguments); };
            if (object && 'object' == typeof (object)) object[funcName] = funcRet;
            return funcRet;
        };

        scope.overrideEasyBase = function (object, funcName, func) {
            /// <signature>
            /// <summary>
            /// 用this函数(func)重写对象(object)的base函数(funcName)，base函数作为this函数的参数传入
            /// <para>若base对应的值不为函数，则把base转换为返回对应的值的无参函数</para>
            /// <para>调用base函数时，函数所在的this对象始终为this函数所在的对象(其参数根据ignoreBaseArgs决定使用this函数的参数还是调用时传入的参数)。</para>
            /// </summary>
            /// <param name="object" type="object">对象,若不指定(例如null)则参数funcName必须为函数</param>
            /// <param name="funcName" type="string or function">函数 或 在对象(object)中的函数名称</param>
            /// <param name="func" type="function">重写的函数</param>
            /// <param name="baseAsFirstParam" type="bool">true:旧函数作为新函数的第一个参数传入；    其他：旧函数会作为新函数的最后一个参数传入</param>
            /// <param name="times" type="int">调用的次数（若为正数则在调用times次后 再次调用时就调用base函数）(若为数字，会先取整)</param>
            /// <param name="ignoreBaseArgs" type="bool">true:忽略base函数调用时传入的参数(无论调用base函数时是否指定参数)而使用this函数的原始参数     其他(false 或 undifined等):使用base函数调用时传入的参数  </param>
            /// </signature>

            /// <signature>
            /// <summary>
            /// 用this函数(func)重写对象(object)的base函数(funcName)，base函数作为this函数的参数传入
            /// <para>若base对应的值不为函数，则把base转换为返回对应的值的无参函数</para>
            /// <para>调用base函数时，函数所在的this对象始终为this函数所在的对象(其参数根据ignoreBaseArgs决定使用this函数的参数还是调用时传入的参数)。</para>
            /// </summary>
            /// <param name="object" type="object">对象,若不指定(例如null)则参数funcName必须为函数</param>
            /// <param name="funcName" type="string or function">函数 或 在对象(object)中的函数名称</param>
            /// <param name="func" type="function">重写的函数</param>
            /// <param name="param" type="object">
            ///         param.baseAsFirstParam[bool]: true:旧函数作为新函数的第一个参数传入；    其他：旧函数会作为新函数的最后一个参数传入 
            ///         param.times[int]: 调用的次数（若为正数则在调用times次后 再次调用时就调用base函数）(若为数字，会先取整) 
            ///         param.ignoreBaseArgs[bool]   true:忽略base函数调用时传入的参数(无论调用base函数时是否指定参数)而使用this函数的原始参数     其他(false 或 undifined等):使用base函数调用时传入的参数      
            ///     Demo: {},{baseAsFirstParam:true,times:0,ignoreBaseArgs:false},{times:1}
            /// </param>
            /// </signature>

            var param = arguments[3];
            if (!param || 'object' != typeof (param)) {
                param = { baseAsFirstParam: param, times: arguments[4], ignoreBaseArgs: arguments[5] };
            }

            var baseValue, funcCallBase;
            if ('function' == typeof (funcName)) {
                funcCallBase = baseValue = funcName;
            } else if (object && 'object' == typeof (object)) {
                if ('function' == typeof (baseValue = object[funcName])) {
                    funcCallBase = baseValue;
                } else {
                    funcCallBase = function () { return baseValue; }
                }
            } else {
                throw new Error('overrideEasyBase参数不合法，没有正确指定base函数');
            }

            if (isNaN(param.times)) param.times = -1;
            else param.times = parseInt(param.times);

            var funcCur = function () {
                var args = Array.prototype.slice.call(arguments);
                var self = this, self_args = arguments, base = function () {
                    return funcCallBase.apply(self, (true == param.ignoreBaseArgs ? self_args : arguments));
                }
                if (true == param.baseAsFirstParam) {
                    args.splice(0, 0, base);
                } else {
                    args.push(base);
                }
                var ret = func.apply(this, args);

                if (param.times > 0) {
                    param.times--;
                    if (0 >= param.times) {
                        funcCur = funcCallBase;
                    }
                }
                return ret;
            }
            var funcRet = function () { return funcCur.apply(this, arguments); };
            if (object && 'object' == typeof (object)) object[funcName] = funcRet;
            return funcRet;
        };

        scope.virtual = function (object, funcName, func) {
            /// <signature>
            /// <summary>继承函数，在调用新函数时会调用旧函数</summary>
            /// <param name="object" type="object">对象,若不指定(例如null)则参数funcName必须为函数</param>
            /// <param name="funcName" type="string or function">函数 或 在对象(object)中的函数名称</param>
            /// <param name="func" type="function">重写的函数</param>
            /// <param name="callThisFirst" type="bool">是否先调用重写的函数.   true:先调用重写的函数，后调用base函数； 其他（例如false）: 先调用base函数，后调用重写的函数。  </param>   
            /// <param name="retBase" type="bool">是否用base函数的返回值作为返回值。 true:base函数的返回值作为返回值； 其他（例如false）: 重写函数的返回值作为返回值。 </param>   
            /// <param name="times" type="int">调用的次数（若为正数则在调用times次后 再次调用时就调用base函数）(若为数字，会先取整)</param>
            /// </signature>

            /// <signature>
            /// <summary>继承函数，在调用新函数时会调用旧函数</summary>
            /// <param name="object" type="object">对象,若不指定(例如null)则参数funcName必须为函数</param>
            /// <param name="funcName" type="string or function">函数 或 在对象(object)中的函数名称</param>
            /// <param name="func" type="function">重写的函数</param>
            /// <param name="param" type="object">  
            ///         param.callThisFirst: 是否先调用重写的函数.   true:先调用重写的函数，后调用base函数； 其他（例如false）: 先调用base函数，后调用重写的函数。 
            ///         param.retBase: 是否用base函数的返回值作为返回值。 true:base函数的返回值作为返回值； 其他（例如false）: 重写函数的返回值作为返回值。 
            ///         param.times:调用的次数（若为正数则在调用times次后 再次调用时就调用base函数）(若为数字，会先取整)
            ///     Demo: {},{callThisFirst:false,retBase:true,times:0},{retBase:true,times:1}
            /// </param>
            /// </signature>

            var param = arguments[3];
            if (!param || 'object' != typeof (param)) {
                param = { callThisFirst: param, retBase: arguments[4], times: arguments[5] };
            }


            var baseValue, funcCallBase;
            if ('function' == typeof (funcName)) {
                funcCallBase = baseValue = funcName;
            } else if (object && 'object' == typeof (object)) {
                if ('function' == typeof (baseValue = object[funcName])) {
                    funcCallBase = baseValue;
                } else {
                    funcCallBase = function () { return baseValue; };
                }
            } else {
                throw new Error('virtual参数不合法，没有正确指定base函数');
            }

            if (isNaN(param.times))
                param.times = -1;
            else
                param.times = parseInt(param.times);

            var funcCur = function () {
                var thisRet, baseRet;

                if (true == param.callThisFirst)
                    thisRet = func.apply(this, arguments);

                baseRet = funcCallBase.apply(this, arguments);

                if (true != param.callThisFirst)
                    thisRet = func.apply(this, arguments);

                if (param.times > 0) {
                    param.times--;
                    if (0 >= param.times) {
                        funcCur = funcCallBase;
                    }
                }
                return true == param.retBase ? baseRet : thisRet;
            };

            var funcRet = function () { return funcCur.apply(this, arguments); };
            if (object && 'object' == typeof (object)) object[funcName] = funcRet;
            return funcRet;
        };


        scope.virtualBaseFirstRetBase = function (object, funcName, func) {
            /// <summary>用新函数func重写对象object的函数funcName，先调用base函数，再调用this函数，返回base函数的返回值</summary>
            /// <param name="object" type="object">对象,若不指定(例如null)则参数funcName必须为函数</param>
            /// <param name="funcName" type="string or function">函数 或 在对象(object)中的函数名称</param>
            /// <param name="func" type="function">重写的函数</param>
            return scope.virtual(object, funcName, func, { callThisFirst: false, retBase: true });
        };

        scope.virtualBaseFirstRetThis = function (object, funcName, func) {
            /// <summary>用新函数func重写对象object的函数funcName，先调用base函数，再调用this函数，返回this函数的返回值</summary>
            /// <param name="object" type="object">对象,若不指定(例如null)则参数funcName必须为函数</param>
            /// <param name="funcName" type="string or function">函数 或 在对象(object)中的函数名称</param>
            /// <param name="func" type="function">重写的函数</param>
            return scope.virtual(object, funcName, func, { callThisFirst: false, retBase: false });
        };

        scope.virtualThisFirstRetBase = function (object, funcName, func) {
            /// <summary>用新函数func重写对象object的函数funcName，先调用this函数，再调用base函数，返回base函数的返回值</summary>
            /// <param name="object" type="object">对象,若不指定(例如null)则参数funcName必须为函数</param>
            /// <param name="funcName" type="string or function">函数 或 在对象(object)中的函数名称</param>
            /// <param name="func" type="function">重写的函数</param>
            return scope.virtual(object, funcName, func, { callThisFirst: true, retBase: true });
        };

        scope.virtualThisFirstRetThis = function (object, funcName, func) {
            /// <summary>用新函数func重写对象object的函数funcName，先调用this函数，再调用base函数，返回this函数的返回值</summary>
            /// <param name="object" type="object">对象,若不指定(例如null)则参数funcName必须为函数</param>
            /// <param name="funcName" type="string or function">函数 或 在对象(object)中的函数名称</param>
            /// <param name="func" type="function">重写的函数</param>
            return scope.virtual(object, funcName, func, { callThisFirst: true, retBase: false });
        };

    })(scope['function'] || (scope['function'] = {}));




    /**  扩展 lith.localStorage 客户端存储
     * Date  : 2019-04-17
     * author:Lith
     */
    ; (function (scope) {

        var objName = 'localStorage';

        var obj = scope[objName] || (scope[objName] = {});

        obj.set = function (key, value, expireSeconds) {
            /// <summary>存储值到对应的key中</summary>
            /// <param name="name" type="string">索引码或者名称,需要唯一.</param>
            /// <param name="value" type="string">具体的内容值</param>
            /// <param name="expireSeconds" type="int">过期秒数,不传则永不失效</param>

            var data = { value: value };

            if (expireSeconds && expireSeconds > 0) {
                data.expireTime = new Date().addSecond(expireSeconds).getTime();
            }

            try {
                localStorage.setItem(key, JSON.stringify(data));
            } catch (e) {

            }
        }

        obj.get = function (key) {
            /// <summary>根据对应的key返回对应值,找不到返回null</summary>
            /// <param name="key" type="string">索引码或者名称,需要唯一.</param>
            try {

                var data = localStorage.getItem(key);
                if (!data) return null;

                data = JSON.parse(data);

                if (data.expireTime) {
                    if (data.expireTime < new Date().getTime()) {
                        localStorage.removeItem(key);
                        return null;
                    }
                }
                return data.value;
            } catch (e) {
                return null;
            }
        }

        obj.delete = function (key) {
            /// <summary>根据对应的key删除值</summary>
            /// <param name="name" type="string">索引码或者名称,需要唯一.</param>
            localStorage.removeItem(key);
        }

    })(scope);


    /**  扩展 lith.cookie 客户端缓存
     * Date  : 2017-09-22
     * author:Lith
     */
    ; (function (scope) {

        var objName = 'cookie';

        var obj = scope[objName] || (scope[objName] = {});

        obj.set = function (name, value, expires) {
            /// <summary>存储值到对应的key中</summary>
            /// <param name="name" type="string">索引码或者名称,需要唯一.</param>
            /// <param name="value" type="string">具体的内容值</param>
            /// <param name="expires" type="int">有效期到期时间,不传则默认有效期为1天.例如:new Date().getTime(),秒数值:1496305713020</param>
            if (!expires) {
                var Days = 1;
                var exp = new Date();
                expires = exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
                expires = new Date(expires);
            }
            document.cookie = name + "=" + escape(value) + ";path=/;expires=" + expires.toGMTString();
        }

        obj.get = function (name) {
            /// <summary>根据对应的key返回对应存放在cookie中的值,找不到返回null</summary>
            /// <param name="name" type="string">索引码或者名称,需要唯一.</param>
            var arr;
            var reg = new RegExp("(^|)" + name + "=([^;]*)(;|$)")
            if (arr = document.cookie.match(reg))
                return unescape(arr[2]);
            else
                return null;
        }

        obj.delete = function (name) {
            /// <summary>根据对应的key删除存放在cookie中的值</summary>
            /// <param name="name" type="string">索引码或者名称,需要唯一.</param>
            var exp = new Date();
            exp.setTime(exp.getTime() - 1);
            exp = new Date(exp);
            var cval = this.get(name);
            if (cval)
                document.cookie = name + "=" + escape(cval) + ";expires=" + exp.toGMTString();
        }


    })(scope);


    /**  扩展 scope.browse 浏览器类别
     * Date  : 2017-09-22
     * author:Lith
     */
    ; (function (scope) {

        scope.browser = function () {
            try {
                var userAgent = navigator.userAgent.toLowerCase();
                // Figure out what browser is being used 
                var browser = {
                    version: (userAgent.match(/.+(?:rv|it|ra|ie)[\/: ]([\d.]+)/) || [])[1],
                    safari: /webkit/.test(userAgent),
                    opera: /opera/.test(userAgent),
                    msie: /msie/.test(userAgent) && !/opera/.test(userAgent),
                    mozilla: /mozilla/.test(userAgent) && !/(compatible|webkit)/.test(userAgent)
                };
                return browser;
            } catch (e) {
                return {};
            }
        }

    })(scope);

    /**  扩展 lith.document（动态加载 js 和 css，打开新窗口，获取url参数等）
     * Date  : 2018-08-02
     * author:Lith
     */
    ; (function (scope) {

        var objName = 'document';

        var obj = scope[objName] || (scope[objName] = {});



        var toXmlStr = function toXmlStr(str) {
            /// <summary> 向xml转换。
            /// <para>例如 转换  &lt;a title=&quot;&quot;&gt;ok&lt;/a&gt;  中a标签的内容体（innerHTML）或 转换  &lt;a title=&quot;&quot;&gt;ok&lt;/a&gt;  中title的值。</para>  
            /// <para>转换     &amp; 双引号 &lt; &gt;     为      &amp;amp; &amp;quot; &amp;lt; &amp;gt;(注： 单引号 对应 &amp;apos; (&amp;#39;) ，但有些浏览器不支持，故此函数不转换。)</para>  
            /// </summary>          
            /// <returns type="string">转换后的字符串</returns>
            return str.replace(/\&/g, "&amp;").replace(/\"/g, "&quot;").replace(/\</g, "&lt;").replace(/\>/g, "&gt;");
        };



        obj.url_GetArg = function (src, key) {
            /// <summary>获取当前src中的参数
            /// <para>demo： var jsName=lith.document.url_GetArg("aaa.html?a=1&amp;b=2",'name');</para>
            /// </summary>
            /// <param name="src" type="string">例如:"?a=1&amp;b=2"</param>
            /// <param name="key" type="string">若不为字符串，则返回把所有参数做为键值对的对象。若为字符串，且获取不到，则返回 null</param>
            /// <returns type="string or object"></returns>

            if (arguments.length == 1) {
                key = src;
                src = location.search;
            }

            if ('string' == typeof key) {
                var v = (src.match(new RegExp("(?:\\?|&)" + key + "=(.*?)(?=&|$)")) || ['', null])[1];
                return v && decodeURIComponent(v);
            } else {
                var reg = /(?:\?|&)(.*?)=(.*?)(?=&|$)/g, temp, res = {};

                while ((temp = reg.exec(src)) != null)
                    res[temp[1]] = decodeURIComponent(temp[2]);
                return res;
            }

            //var src = location.search;
            //if (src.length < 2 || src.charAt(0) != '?') {
            //    return null;
            //}
            //var params = src.substring(1).split('&');
            //var ps = null;
            //for (var i in params) {
            //    ps = params[i].split('=');
            //    if (decodeURIComponent(ps[0]) == name) {
            //        return decodeURIComponent(ps[1]);
            //    }
            //}


            //return null;
        };




        obj.script_getArg = function (key) {
            /// <summary>返回所在脚本src参数。
            /// <para>demo： var jsName=lith.document.script_getArg('name');</para>
            /// <para>不要在方法中调用此方法，否则可能始终获取的是最后一个js的文件的参数</para>
            /// </summary>
            /// <param name="key" type="string">若不为字符串，则返回把所有参数做为键值对的对象。若为字符串，且获取不到，则返回 null</param>



            ////假如上面的js是在这个js1.js的脚本中<script type="text/javascript" src="js1.js?a=abc&b=汉字&c=123"></script>
            var scripts = document.getElementsByTagName("script"),
                //因为当前dom加载时后面的script标签还未加载，所以最后一个就是当前的script
                script = scripts[scripts.length - 1],
                src = script.src;

            return obj.url_GetArg(src, key);
        };






        obj.openWin = function (html) {
            /// <summary>在新页面中显示html</summary>
            /// <param name="html" type="String">html 代码</param>
            /// <returns type="Window"></returns> 
            var oWin = window.open('');
            oWin.document.write(html);
            return oWin;
        };

        obj.openForm = function (param) {
            /// <summary>在新页面中新建Form，发送请求。
            /// <para> demo:lith.document.openForm({url:'http://www.a.com',reqParam:{a:3},type:'post'}); </para>
            /// </summary>
            /// <param name="param" type="object">
            /// <para> demo:{url:'http://www.a.com',reqParam:{a:3},type:'post'} </para>
            /// <para> url[string]:要打开的链接地址。</para>
            /// <para> reqParam[object]:请求参数。</para>
            /// <para> type[string]:请求方式。可为'get'、'post'、'put'等，不指定则为get。</para>           
            /// </param>
            /// <param name="url" type="string"></param>
            /// <param name="postParam" type="object">。</param>
            /// <param name="type" type="string"></param>
            /// <returns type="window"></returns>


            var html = '<!DOC' + 'TYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml"><head ><meta http-equiv="Content-Type" content="text/html;charset=UTF-8" /><tit';
            html += 'le>请稍等</title> </head><body>';
            html += '<h3>请稍等 ...</h3>';
            html += '<form  accept-charset="UTF-8"  name="tempForm"  action="' + toXmlStr(param.url) + '" method="' + (param.type || "get") + '" style="display:none">';
            for (var name in param.reqParam) {
                html += '<input type="hidden" name="' + toXmlStr(name) + '" value="' + toXmlStr(param.reqParam[name]) + '"/>';
            }
            html += '</form>';
            html += '<script type="text/javascript">document.tempForm.submit();</sc' + 'ript>';
            html += '</body></html>';

            return obj.openWin(html);
        };



        obj.iframeSetOnload = function (iframe, event) {
            //if...else...是一种兼容ie的写法
            if (iframe.attachEvent) {
                iframe.attachEvent("onload", event);
            } else {
                iframe.onload = event;
            }
        };


        obj.loadJs = function (jsSrc) {
            /// <summary>载入js文件。在文档加载过程中或已经加载完成后载入js文件。</summary>
            /// <param name="jsSrc" type="string">例如："/Scripts/jquery-easyui/jquery.easyui.min.js"</param>

            if (document.readyState == "loading") {
                loadJs_BeforeDocumentLoaded(jsSrc);
            } else {
                loadJs_AfterDocumentLoaded(jsSrc);
            }


            function loadJs_BeforeDocumentLoaded(jsSrc) {
                /// <summary>载入js文件。在文档加载过程中载入js文件。</summary>
                /// <param name="jsSrc" type="string">例如："/Scripts/jquery-easyui/jquery.easyui.min.js"</param>

                // <script type="text/javascript" src="/Scripts/jquery-easyui/jquery.easyui.min.js"></script>

                document.write('<script type="text/javascript" src="' + toXmlStr('' + jsSrc) + '"></script>');
            }

            function loadJs_AfterDocumentLoaded(jsSrc) {
                /// <summary>载入js文件。在文档已经加载完成后载入js文件。</summary>
                /// <param name="jsSrc" type="string">例如："/Scripts/jquery-easyui/jquery.easyui.min.js"</param>     

                var eJs = document.createElement('script');
                eJs.type = 'text/javascript';
                eJs.language = 'javascript';
                eJs.src = jsSrc;
                document.getElementsByTagName("head")[0].appendChild(eJs);
            }

        };



        obj.loadCss = function (cssSrc) {
            /// <summary>载入css文件。在文档加载过程中或已经加载完成后载入css文件。</summary>
            /// <param name="cssSrc" type="string">例如："/Scripts/jquery-easyui/themes/icon.css"</param>

            if (document.readyState == "loading") {
                loadCss_BeforeDocumentLoaded(cssSrc);
            } else {
                loadCss_AfterDocumentLoaded(cssSrc);
            }

            function loadCss_BeforeDocumentLoaded(cssSrc) {
                /// <summary>载入css文件。在文档加载过程中载入css文件。</summary>
                /// <param name="cssSrc" type="string">例如："/Scripts/jquery-easyui/themes/icon.css"</param>

                // <link rel="stylesheet" type="text/css" href="/Scripts/jquery-easyui/themes/icon.css" />
                document.write('<link rel="stylesheet" type="text/css" href="' + toXmlStr('' + cssSrc) + '" />');
            }


            function loadCss_BeforeDocumentLoaded(cssSrc) {
                /// <summary>载入css文件。在文档已经加载完成后载入css文件。</summary>
                /// <param name="cssSrc" type="string">例如："/Scripts/jquery-easyui/themes/icon.css"</param>

                var eCss = document.createElement('link');
                eCss.rel = 'Stylesheet';
                eCss.type = 'text/css';
                eCss.href = cssSrc;
                document.getElementsByTagName("head")[0].appendChild(eCss);
            }

        };





        obj.addCss = function (cssText) {
            /// <summary>添加新的CSS样式节点。demo: lith.document.addCss('.header{ background-color:#8f8;}');</summary>
            /// <param name="cssText" type="String"></param>

            var style = document.createElement('style');  //创建一个style元素              
            style.type = 'text/css'; //这里必须显示设置style元素的type属性为text/css，否则在ie中不起作用        

            var head = document.head || document.getElementsByTagName('head')[0]; //获取head元素
            head.appendChild(style); //把创建的style元素插入到head中  

            if (style.styleSheet) { //IE

                var func = function () {
                    try {
                        //防止IE中stylesheet数量超过限制而发生错误
                        style.styleSheet.cssText = cssText;
                    } catch (e) { }
                }
                //如果当前styleSheet还不能用，则放到异步中则行
                if (style.styleSheet.disabled) {
                    setTimeout(func, 10);
                } else {
                    func();
                }
            } else { //w3c
                //w3c浏览器中只要创建文本节点插入到style元素中就行了
                var textNode = document.createTextNode(cssText);
                style.appendChild(textNode);
            }
        };




    })(scope);





})('undefined' === typeof (lith) ? lith = {} : lith);