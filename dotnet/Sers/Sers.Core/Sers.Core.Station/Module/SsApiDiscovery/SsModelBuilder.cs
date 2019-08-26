﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Log;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Core.Util.XmlComment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sers.Core.Module.SsApiDiscovery
{
    public class SsModelBuilder
    {
        public XmlMng xmlMng;

        #region SsModel
        public SsModel BuildSsModel_Return(MethodInfo methodInfo)
        {
            Type t = methodInfo.ReturnType;

            SsModel model = new SsModel();

            var models = new List<SsModelEntity>();
            var modelProperty = CreateModelProperty(t, t.GetCustomAttribute, models);

            model.type = modelProperty.type;
            model.mode = modelProperty.mode;

            {
                model.description = methodInfo.ReturnParameter.GetCustomAttribute<SsDescriptionAttribute>()?.Value;
                model.example = methodInfo.ReturnParameter.GetCustomAttribute<SsExampleAttribute>()?.Value;
                model.defaultValue = methodInfo.ReturnParameter.GetCustomAttribute<SsDefaultValueAttribute>()?.Value;
            }
            {
                if (String.IsNullOrEmpty(model.description))
                    model.description = modelProperty.description;

                if (null == model.defaultValue)
                    model.defaultValue = modelProperty.defaultValue;

                if (null == model.example)
                    model.example = modelProperty.example;
            }

            if (models.Count > 0)
            {
                model.models = models;
            }
            return model;
        }


        public SsModel BuildSsModel_Arg(MethodInfo method, MethodComment comment)
        {
            ParameterInfo[] infos = method.GetParameters();

            SsModel model = new SsModel();

            //构建 OnDeserialize

            #region (x.x.1) 空参数 没有参数
            if (null == infos || 0 == infos.Length)
            {
                model.OnDeserialize = (strValue) =>
                {
                    return null;
                };
                return model;
            }
            #endregion


            #region (x.x.2)函数首个参数 为参数实体
            // 1)第一个参数有 SsArgEntityAttribute 特性
            // 2)参数个数为1，且(mode 为 object 或者 array),且没有SsArgPropertyAttribute特性
            if (
                null != infos[0].GetCustomAttribute<SsArgEntityAttribute>() ||
                (1 == infos.Length && !infos[0].ParameterType.TypeIsValueTypeOrStringType() && null == infos[0].GetCustomAttribute<SsArgPropertyAttribute>())
            )
            {
                #region      
                int argCount = infos.Length;
                var argType = infos[0].ParameterType;
                model.OnDeserialize = (bytes) =>
                {
                    var args = new object[argCount];
                    args[0] = bytes.DeserializeFromBytes(argType);
                    return args;
                };

                var modelEntitys = new List<SsModelEntity>();
                var mEntity = CreateEntityByType(argType, modelEntitys);

                model.models = modelEntitys;

                model.mode = mEntity.mode;
                model.type = mEntity.type;

                {
                    model.description = infos[0].GetCustomAttribute<SsDescriptionAttribute>()?.Value;
                    model.example = infos[0].GetCustomAttribute<SsExampleAttribute>()?.Value;
                    model.defaultValue = infos[0].GetCustomAttribute<SsDefaultValueAttribute>()?.Value;
                }
                #endregion

                return model;
            }
            #endregion



            #region (x.x.3)函数参数列表 为参数实体
            {
                model.OnDeserialize = (bytes) =>
                {
                    var jo = bytes.DeserializeFromBytes<JObject>();

                    var arg = new object[infos.Length];
                    if (null != jo)
                    {
                        int i = 0;
                        foreach (var info in infos)
                        {
                            arg[i++] = jo[info.Name].Deserialize(info.ParameterType);
                        }
                    }
                    return arg;
                };

                var models = new List<SsModelEntity>();
                var mEntity = CreateEntityByParameterInfo(infos, comment, models);

                model.models = models;
                model.type = mEntity.type;
                model.mode = mEntity.mode;

            }
            return model;
            #endregion

         
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="infos">length大于0</param>
        /// <param name="comment"></param>
        /// <param name="refModels"></param> 
        /// <returns></returns>
        SsModelEntity CreateEntityByParameterInfo(ParameterInfo[] infos, MethodComment comment, List<SsModelEntity> refModels)
        { 
            var rootEntity = new SsModelEntity();
            rootEntity.type = "arg";
            rootEntity.mode = "object";

            refModels.Add(rootEntity);

            var propertys = rootEntity.propertys = new List<SsModelProperty>();
            for (var t = 0; t < infos.Length; t++)
            {
                var info = infos[t];
                SsModelProperty m = CreateModelProperty(info.ParameterType, info.GetCustomAttribute, refModels);
                if (String.IsNullOrWhiteSpace(m.name)) m.name = info.Name;

                try
                {
                    if (String.IsNullOrWhiteSpace(m.description))
                    {
                        m.description = comment?.param[t]?.comment;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                propertys.Add(m);

            }
            return rootEntity;
        }


        SsModelProperty CreateModelProperty(Type propertyType, Func<Type, Attribute> delGetCustomAttribute, List<SsModelEntity> refModels)
        {
            #region GetCustomAttribute
            T GetCustomAttribute<T>()
                where T : Attribute
            {
                return delGetCustomAttribute(typeof(T)) as T;
            }
            #endregion 

            SsModelProperty m = new SsModelProperty();

            m.name = GetCustomAttribute<SsNameAttribute>()?.Value;
            m.description = GetCustomAttribute<SsDescriptionAttribute>()?.Value;
            m.example = GetCustomAttribute<SsExampleAttribute>()?.Value;
            m.defaultValue = GetCustomAttribute<SsDefaultValueAttribute>()?.Value;


            Type_GetMode(propertyType, out m.mode, out m.type, out Type baseT);

            if (m.mode == "value")
            {

            }
            else
            {
                if (!refModels.Exists((item) => item.type == m.type))
                {
                    CreateEntityByType(propertyType, refModels);
                }
            }
            return m;

        }


        /// <summary>
        ///  info mode  可为 array 或 object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="refModels"></param>
        /// <returns></returns>
        SsModelEntity CreateEntityByType(Type info, List<SsModelEntity> refModels)
        {
            var m = new SsModelEntity();

            Type_GetMode(info, out m.mode, out m.type, out Type baseT);

            if (m.mode == "value")
            {
                return m;
            }

            refModels.Add(m);

            if (m.mode == "object")
            {
                m.propertys = ObjectMode_BuildPropertysByType(baseT, refModels);
            }
            else
            {
                m.propertys = ArrayMode_BuildPropertysByType(info, baseT, refModels);
            }
            return m;
        }

        List<SsModelProperty> ArrayMode_BuildPropertysByType(Type type, Type baseT, List<SsModelEntity> refModels)
        {
            var propertys = new List<SsModelProperty>();

            #region MyRegion
            var m = CreateModelProperty(baseT, baseT.GetCustomAttribute, refModels);

            propertys.Add(m);

            m.name = "0";

            #endregion

            return propertys;
        }

        List<SsModelProperty> ObjectMode_BuildPropertysByType(Type type, List<SsModelEntity> refModels)
        {
            var propertys = new List<SsModelProperty>();

            #region (x.1)忽略 JToken(JObject JArray)等
            if (typeof(JToken).IsAssignableFrom(type))
            {
                //JObject JArray 等
                return null;
            }
            #endregion

            #region (x.2)泛型 忽略 Dictionary 等  (List已作为数组处理)
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                //(x.2) IDictionary
                if (typeof(IDictionary<,>).IsAssignableFrom(genericType))
                {
                    return null;
                }
            }
            #endregion



            var pi = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);


            #region (x.3)json 序列化 特性
            var attr = type.GetCustomAttribute<JsonObjectAttribute>();
            if (null != attr)
            {
                //    [JsonObject(MemberSerialization.OptIn)]
                if (attr.MemberSerialization == MemberSerialization.OptIn)
                {
                    var list = from info in pi
                               where null != info.GetCustomAttribute<JsonPropertyAttribute>()
                               select info;
                    pi = list.ToArray();

                    var list2 = from info in fields
                                where null != info.GetCustomAttribute<JsonPropertyAttribute>()
                                select info;
                    fields = list2.ToArray();
                }
                //    [JsonObject(MemberSerialization.Fields)]
                else if (attr.MemberSerialization == MemberSerialization.Fields)
                {
                    pi = new PropertyInfo[0];

                    fields = fields.Where(field => null == field.GetCustomAttribute<JsonIgnoreAttribute>()).ToArray();
                }
                //    [JsonObject(MemberSerialization.OptOut)]
                else if (attr.MemberSerialization == MemberSerialization.OptOut)
                {
                    pi = pi.Where(field => null == field.GetCustomAttribute<JsonIgnoreAttribute>()).ToArray();

                    fields = fields.Where(field => null == field.GetCustomAttribute<JsonIgnoreAttribute>()).ToArray();
                }
            }

            #endregion


            #region (x.4)构建各个属性  
            var xmlHelp = xmlMng.GetXmlHelp(type);
            SsModelProperty m;
            foreach (var property in pi)
            {
                m = CreateModelProperty(property.PropertyType, property.GetCustomAttribute, refModels);

                propertys.Add(m);

                #region 获取propertyName               
                // 以JsonPropertyAttribute为主，若为空则使用反射的名称
                String propertyName = property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
                if (String.IsNullOrEmpty(propertyName))
                {
                    propertyName = property.Name;
                }
                #endregion              

                if (String.IsNullOrWhiteSpace(m.name)) m.name = propertyName;

                if (String.IsNullOrWhiteSpace(m.description))
                {
                    m.description = xmlHelp?.Property_GetSummary(property);
                }

            }
            foreach (var field in fields)
            {
                m = CreateModelProperty(field.FieldType, field.GetCustomAttribute, refModels);

                propertys.Add(m);


                #region 获取propertyName               
                // 以JsonPropertyAttribute为主，若为空则使用反射的名称
                String propertyName = field.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
                if (String.IsNullOrEmpty(propertyName))
                {
                    propertyName = field.Name;
                }
                #endregion

                if (String.IsNullOrWhiteSpace(m.name)) m.name = propertyName;

                if (String.IsNullOrWhiteSpace(m.description))
                {
                    m.description = xmlHelp?.Field_GetSummary(field);
                }
            }
            return propertys;

            #endregion
        }



        static void Type_GetMode(Type t, out String mode, out String type, out Type baseT)
        {

            #region (x.0) 二进制数据
            if (typeof(byte[]) == t)
            {
                mode = "value";
                type = "binary";
                baseT = t;
                return;
            }
            #endregion


            #region (x.1)value

            if (t.TypeIsValueTypeOrStringType())
            {
                mode = "value";
                baseT = t.GetUnderlyingTypeIfNullable();
                if (typeof(int) == baseT)
                {
                    type = "int32";
                }
                else if (typeof(long) == baseT)
                {
                    type = "int64";
                }
                else if (typeof(float) == baseT)
                {
                    type = "float";
                }
                else if (typeof(double) == baseT)
                {
                    type = "double";
                }
                else if (typeof(bool) == baseT)
                {
                    type = "bool";
                }
                else if (typeof(DateTime) == baseT)
                {
                    type = "datetime";
                }
                else if (typeof(String) == baseT)
                {
                    type = "string";
                }
                else
                {
                    type = "string";
                }
                return;
            }
            #endregion



            #region (x.2)array

            if (Type_IsArray(t, out baseT))
            {
                type = Type_GetType(t);
                mode = "array";
                return;
            }

            #endregion


            #region (x.3)object
            mode = "object";
            type = Type_GetType(t);
            baseT = t;
            return;
            #endregion            
        }

        static String Type_GetType(Type t)
        {
            return t.Name + "_" + t.GetHashCode();
        }

        static bool Type_IsArray(Type type, out Type elementType)
        {

            //(x.1)数组
            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }


            //(x.2)泛型
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                // List<>
                if (typeof(List<>).IsAssignableFrom(genericType))
                {
                    elementType = type.GetGenericArguments()[0];
                    return true;
                }
            }

            //(x.3) JArray
            if (typeof(JArray).IsAssignableFrom(type))
            {
                elementType = typeof(object);
                return true;
            }

            elementType = type;
            return false;
        }


        #endregion
    }
}
