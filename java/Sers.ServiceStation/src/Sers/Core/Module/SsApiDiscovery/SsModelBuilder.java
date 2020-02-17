package Sers.Core.Module.SsApiDiscovery;

import java.lang.annotation.Annotation;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.lang.reflect.Modifier;
import java.lang.reflect.Parameter;
import java.lang.reflect.ParameterizedType;
import java.lang.reflect.Type;
import java.lang.reflect.TypeVariable;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.Date;
import java.util.HashMap;
import java.util.List;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

import Sers.Core.Module.Api.ApiDesc.SsModel;
import Sers.Core.Module.Api.ApiDesc.SsModelEntity;
import Sers.Core.Module.Api.ApiDesc.SsModelProperty;
import Sers.Core.Module.Api.Data.ApiReturn;
import Sers.Core.Module.Serialization.Serialization;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsArgEntity;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsArgProperty;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsDefaultValue;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsDescription;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsExample;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsName;
import Sers.Core.Module.SsApiDiscovery.ApiDesc.Annotation.SsReturn;
import Sers.Core.Util.Common.CommonHelp;

public class SsModelBuilder {

	// region common
	public static boolean TypeIsValueTypeOrStringType(Class type) {
		if (type == null) {
			return false;
		}
		return type.isPrimitive() || String.class.isAssignableFrom(type);
	}

	// endregion

	public SsModel BuildSsModel_Return(Method methodInfo) {

		Class t = methodInfo.getReturnType();

		Type[] genericTypes = null;
		Type returnType = methodInfo.getGenericReturnType();

		if (returnType instanceof ParameterizedType) {
			ParameterizedType pReturnType = (ParameterizedType) returnType;
			genericTypes = pReturnType.getActualTypeArguments();
		}

		SsModel model = new SsModel();

		ArrayList<SsModelEntity> models = new ArrayList<SsModelEntity>();
		SsModelProperty modelProperty = CreateModelProperty(t,genericTypes, IGetAnnotation.Build(t), models);

		model.type = modelProperty.type;
		model.mode = modelProperty.mode;

		{
			SsReturn ssRet = methodInfo.getAnnotation(SsReturn.class);
			if (ssRet != null) {
				model.description = ssRet.description();
				model.example = ssRet.example();
//				model.defaultValue = ssRet.defaultValue();
			}
		}
		{
			if (CommonHelp.StringIsNullOrEmpty(model.description))
				model.description = modelProperty.description;

			if (null == model.defaultValue)
				model.defaultValue = modelProperty.defaultValue;

			if (null == model.example)
				model.example = modelProperty.example;
		}

		if (models.size() > 0) {
			model.models = models.stream().toArray(SsModelEntity[]::new);
		}
		return model;
	}

	public SsModel BuildSsModel_Arg(Method method) {

		Parameter[] infos = method.getParameters();

		SsModel model = new SsModel();

		// 构建 OnDeserialize

		// region (x.x.1) 空参数 没有参数
		if (null == infos || 0 == infos.length) {
			model.onDeserialize_Set((strValue) -> {
				return null;
			});
			return model;
		}
		// endregion

		// region (x.x.2)函数首个参数 为参数实体
		// 1)第一个参数有 SsArgEntityAttribute 特性
		// 2)参数个数为1，且(mode 为 object 或者 array),且没有SsArgPropertyAttribute特性
		if (null != infos[0].getAnnotation(SsArgEntity.class)
				|| (1 == infos.length && !TypeIsValueTypeOrStringType(infos[0].getType())
						&& null == infos[0].getAnnotation(SsArgProperty.class))) {
			// region
			int argCount = infos.length;
			Parameter info = infos[0];

			Class argType = info.getType();
			model.onDeserialize_Set((bytes) -> {
				Object[] args = new Object[argCount];
				args[0] = Serialization.Instance.deserializeFromBytes(bytes, argType);
				return args;
			});

			ArrayList<SsModelEntity> modelEntitys = new ArrayList<SsModelEntity>();
			SsModelEntity mEntity = CreateEntityByType(argType,null, modelEntitys);

			model.models = modelEntitys.stream().toArray(SsModelEntity[]::new);

			model.mode = mEntity.mode;
			model.type = mEntity.type;

			{
				model.description = CommonHelp.CallWhenNotNull(info.getAnnotation(SsDescription.class), m -> m.value());
				model.example = CommonHelp.CallWhenNotNull(info.getAnnotation(SsExample.class), m -> m.value());
				model.defaultValue = CommonHelp.CallWhenNotNull(info.getAnnotation(SsDefaultValue.class),
						m -> m.value());
			}
			// endregion

			return model;
		}
		// endregion

		// region (x.x.3)函数参数列表 为参数实体
		{
			String[] argNames= Arrays.stream(infos).map(info->{			 
				SsName ssName= info.getAnnotation(SsName.class);
				return  ssName==null?info.getName():ssName.value(); 
			}).toArray(String[]::new);
			
			Class [] types =Arrays.stream(infos).map(info->info.getType()).toArray(Class[]::new);
			
			model.onDeserialize_Set((arraySegment) -> {
				JsonObject jo = Serialization.Instance.deserializeFromBytes(arraySegment, JsonObject.class);

				Object[] arg = new Object[argNames.length];
				if (null != jo) {
					int i = -1;
					for (String argName : argNames) {
						i++;

						JsonElement je = jo.get(argName);
						arg[i] = Serialization.Instance.deserialize(je, types[i]);
					}
				}
				return arg;
			});

			ArrayList<SsModelEntity> models = new ArrayList<SsModelEntity>();
			SsModelEntity mEntity = CreateEntityByParameterInfo(infos, models);

			model.models = models.stream().toArray(SsModelEntity[]::new);
			model.type = mEntity.type;
			model.mode = mEntity.mode;

		}
		return model;
		// endregion

	}

	/////////////////////////////////////////////////////////////////////////////////////////////////

	/**
	 * info 的 mode 可为 array 或 object
	 * 
	 * @param info
	 * @param refModels
	 * @return
	 */
	SsModelEntity CreateEntityByType(Class info,Type[] genericTypes, List<SsModelEntity> refModels) {
		SsModelEntity m = new SsModelEntity();

		Object[] ta = Type_GetMode(info);

		m.mode = (String) ta[0];
		m.type = (String) ta[1];
		Class baseT = (Class) ta[2];

		if (m.mode == "value") {
			return m;
		}

		refModels.add(m);

		if (m.mode == "object") {
			m.propertys = ObjectMode_BuildPropertysByType(baseT,genericTypes, refModels).stream().toArray(SsModelProperty[]::new);
		} else {
			m.propertys = ArrayMode_BuildPropertysByType(info,genericTypes, baseT, refModels).stream()
					.toArray(SsModelProperty[]::new);
		}
		return m;
	}

	List<SsModelProperty> ArrayMode_BuildPropertysByType(Class type,Type[] genericTypes, Class baseT, List<SsModelEntity> refModels) {
		ArrayList<SsModelProperty> propertys = new ArrayList<SsModelProperty>();

		SsModelProperty m = CreateModelProperty(baseT,genericTypes, IGetAnnotation.Build(baseT), refModels);

		propertys.add(m);

		m.name = "0";

		return propertys;
	}

	List<SsModelProperty> ObjectMode_BuildPropertysByType(Class type,Type[] genericTypes,  List<SsModelEntity> refModels) {
		ArrayList propertys = new ArrayList<SsModelProperty>();

		// region (x.1)忽略 JToken(JObject JArray) 泛型 Dictionary 等
		if (JsonElement.class.isAssignableFrom(type) || Collection.class.isAssignableFrom(type))  																									
		{
			// JObject JArray 等
			return null;
		}
		// endregion

		HashMap<String, Type> genericTypeMap = new HashMap<>();
		 
		TypeVariable<?>[] typeparms = type.getTypeParameters();
		if(typeparms!=null && typeparms.length!=0 && genericTypes!=null) {
			String[]  genericTypeNames = Arrays.stream(typeparms).map(m -> m.getName()).toArray(String[]::new);
			for(int t= Math.min(genericTypeNames.length, genericTypes.length)-1;t>=0;t-- ) {
				genericTypeMap.put(genericTypeNames[t], genericTypes[t]);
			}			
		}
   	  
		Field[] fields = type.getFields();

		// region (x.2)构建各个属性
		SsModelProperty m;
		for (Field field : fields) {
			// 排除static属性
			if (0 != (field.getModifiers() & Modifier.STATIC))
				continue;

			Class fieldClazz=field.getType();
			
		   	 Type  fieldType= field.getGenericType();
		   	 
		   	 if(fieldType!= field.getType() ) {
		   	   	String  TypeName= fieldType.getTypeName();		   	   
		   	   	Type genericType=genericTypeMap.get(TypeName);
		   	   	
		   	   	if(genericType!=null && genericType instanceof Class) {
		   	   		fieldClazz= (Class)genericType;
		   	   	}  		 
		   	 }        	
			
			m = CreateModelProperty(fieldClazz,genericTypes, IGetAnnotation.Build(field), refModels);

			propertys.add(m);

			// 获取propertyName
			if (CommonHelp.StringIsNullOrEmpty(m.name))
				m.name = field.getName();
		}
		return propertys;

		// endregion
	}

	interface IGetAnnotation {
		<T extends Annotation> T getAnnotation(Class<T> clazz);

		static IGetAnnotation Build(Class type) {

			IGetAnnotation ga = new IGetAnnotation() {
				@Override
				public <T extends Annotation> T getAnnotation(Class<T> clazz) {
					// TODO Auto-generated method stub
					return (T) type.getAnnotation(clazz);
				}

			};
			return ga;
		}

		static IGetAnnotation Build(Field field) {

			IGetAnnotation ga = new IGetAnnotation() {
				@Override
				public <T extends Annotation> T getAnnotation(Class<T> clazz) {
					// TODO Auto-generated method stub
					return field.getAnnotation(clazz);
				}

			};
			return ga;
		}

		static IGetAnnotation Build(Parameter field) {

			IGetAnnotation ga = new IGetAnnotation() {
				@Override
				public <T extends Annotation> T getAnnotation(Class<T> clazz) {
					// TODO Auto-generated method stub
					return field.getAnnotation(clazz);
				}
			};
			return ga;
		}

	}

	SsModelEntity CreateEntityByParameterInfo(Parameter[] infos, List<SsModelEntity> refModels) {

		SsModelEntity rootEntity = new SsModelEntity();
		rootEntity.type = "arg";
		rootEntity.mode = "object";

		refModels.add(rootEntity);

		ArrayList<SsModelProperty> propertys = new ArrayList<SsModelProperty>();
		for (int t = 0; t < infos.length; t++) {
			Parameter info = infos[t];
			SsModelProperty m = CreateModelProperty(info.getType(),null, IGetAnnotation.Build(info), refModels);
			if (CommonHelp.StringIsNullOrEmpty(m.name))
				m.name = info.getName();

			propertys.add(m);

		}
		rootEntity.propertys = propertys.stream().toArray(SsModelProperty[]::new);
		return rootEntity;
	}

	SsModelProperty CreateModelProperty(Class propertyType,Type[] genericTypes, IGetAnnotation delGetCustomAttribute,
			List<SsModelEntity> refModels) {

		SsModelProperty m = new SsModelProperty();

		m.name = CommonHelp.CallWhenNotNull(delGetCustomAttribute.getAnnotation(SsName.class), a -> a.value());
		m.description = CommonHelp.CallWhenNotNull(delGetCustomAttribute.getAnnotation(SsDescription.class),
				a -> a.value());
		m.example = CommonHelp.CallWhenNotNull(delGetCustomAttribute.getAnnotation(SsExample.class), a -> a.value());
		m.defaultValue = CommonHelp.CallWhenNotNull(delGetCustomAttribute.getAnnotation(SsDefaultValue.class),
				a -> a.value());

		Object[] ta = Type_GetMode(propertyType);

		m.mode = (String) ta[0];
		m.type = (String) ta[1];
		Class baseT = (Class) ta[2];

		if (m.mode == "value") {

		} else {
			if (!refModels.stream().anyMatch((item) -> m.type.equals(item.type))) {
				CreateEntityByType(propertyType,genericTypes, refModels);
			}
		}
		return m;

	}

	/**
	 * return new Object[]{mode,type,baseT }
	 * 
	 * @param t
	 */
	static Object[] Type_GetMode(Class t) {

		String mode = null;
		String type = null;
		Class baseT = null;
		// region (x.0) 二进制数据
		if (byte[].class.isAssignableFrom(t)) {
			mode = "value";
			type = "binary";
			baseT = t;
			return new Object[] { mode, type, baseT };
		}
		// endregion

		// region (x.1)value

		if (TypeIsValueTypeOrStringType(t)) {
			mode = "value";
			baseT = t;
			if (int.class.isAssignableFrom(t)) {
				type = "int32";
			} else if (long.class.isAssignableFrom(t)) {
				type = "int64";
			} else if (float.class.isAssignableFrom(t)) {
				type = "float";
			} else if (double.class.isAssignableFrom(t)) {
				type = "double";
			} else if (boolean.class.isAssignableFrom(t)) {
				type = "bool";
			} else if (Date.class.isAssignableFrom(t)) {
				type = "datetime";
			} else if (String.class.isAssignableFrom(t)) {
				type = "string";
			} else {
				type = "string";
			}
			return new Object[] { mode, type, baseT };
		}
		// endregion

		// region (x.2)array
		if ((baseT = Type_IsArray(t)) != null) {
			type = Type_GetType(t);
			mode = "array";
			return new Object[] { mode, type, baseT };
		}
		// endregion

		// region (x.3)object
		mode = "object";
		type = Type_GetType(t);
		baseT = t;

		return new Object[] { mode, type, baseT };
		// endregion
	}

	static String Type_GetType(Class t) {
		return t.getSimpleName() + "_" + t.hashCode();
	}

	/**
	 * Returns the Class representing the component type of anarray. If this class
	 * does not represent an array class this methodreturns null.
	 * 
	 * @param type
	 * @return
	 */
	static Class Type_IsArray(Class type) {
		return type.getComponentType();
	}

}
