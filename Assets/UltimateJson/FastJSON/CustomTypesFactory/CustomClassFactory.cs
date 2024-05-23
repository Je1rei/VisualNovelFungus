using System;
using System.Collections;
using System.Collections.Generic;

namespace fastJSON.CustomTypesFactory
{
	public class CustomClassFactory
	{
		public static object ParseCustomClass(object obj, Type type)
		{
			//UnityEngine.Debug.Log(type);
			var genericType = Reflection.Instance.GetGenericTypeDefinition(type);
			//UnityEngine.Debug.Log(genericType);
			if (genericType == typeof(List<>))
			{
				return CreateList(obj, type);
			}
			if (genericType.IsArray)
			{
				var elementType = type.GetElementType();
				return CreateArray(obj, elementType);
			}

			var result = CreateClassFromDic((Dictionary<string, object>)obj, type, null);
			return result;
		}
		
		private static IList CreateList(object obj, Type type)
		{
			var genericTypes = Reflection.Instance.GetGenericArguments(type);
			var listElementType = genericTypes[0];

			return CreateList(obj, type, listElementType);
		}

		private static IList CreateList(object obj, Type type, Type listElementType)
		{
			var result = (IList)Reflection.Instance.FastCreateInstance(type);
			
			foreach (var listElement in (IList)obj)
			{
				var listElementResult = CreateClassFromDic((Dictionary<string, object>)listElement, listElementType, null);
				result.Add(listElementResult);
			}
			
			return result;
		}

		private static IList CreateArray(object obj, Type listElementType)
		{
			var objList = (IList) obj;
			var objSize = objList.Count;
			var result = Array.CreateInstance(listElementType, objSize);

			for (var i = 0; i < objSize; i++)
			{
				var listElement = objList[i];
				var objListElementType = listElement.GetType();
				if (objListElementType == typeof(string))
				{
					var listElementResult = listElement.ToString();
					result.SetValue(listElementResult, i);
				}
				else
				{
					var listElementResult = CreateClassFromDic((Dictionary<string, object>)listElement, listElementType, null);
					result.SetValue(listElementResult, i);
				}
			}
			
			return result;
		}

		private static object CreateClassFromDic(Dictionary<string, object> dic, Type type, object parent)
		{
			var typename = type.FullName;
			var result = parent ?? Reflection.Instance.FastCreateInstance(type);
			
			var props = Reflection.Instance.Getproperties(type, typename);
			foreach (var kv in dic)
			{
				var key = kv.Key;
				var value = kv.Value;
				var keyName = key.ToLower();

				var containsNameInProps = props.ContainsKey(keyName);
				if (!containsNameInProps)
				{
					continue;
				}
				var propInfo = props[keyName];
				object oset = null;
				
				switch (propInfo.Type)
				{
					case MyPropInfoType.Int: oset = (int)(long)value; break;					
					case MyPropInfoType.Float: oset = (float)Deserializer.TryGetDoubleValue(value); break;
					case MyPropInfoType.String: oset = (string)value; break;
					case MyPropInfoType.Bool: oset = (bool)value; break;
					
					case MyPropInfoType.Char: oset = ((string)value)[0]; break;
					case MyPropInfoType.Byte: oset = (byte)(long)value; break;
					case MyPropInfoType.Decimal: oset = (decimal)Deserializer.TryGetDoubleValue(value); break;
					case MyPropInfoType.Double: oset = Deserializer.TryGetDoubleValue(value); break;
					case MyPropInfoType.Short: oset = (short)(long)value; break;
					
					case MyPropInfoType.Long: oset = (long)value; break;
					case MyPropInfoType.DateTime: oset = Deserializer.CreateDateTime((string)value); break;
					case MyPropInfoType.Enum: oset = Deserializer.CreateEnum(propInfo.Pt, value); break;
					case MyPropInfoType.Guid: oset = Deserializer.CreateGuid((string)value); break;
					
					case MyPropInfoType.List: oset = CreateList((List<object>) value, propInfo.Pt); break;
					
					case MyPropInfoType.Array: oset = Deserializer.CreateArray((List<object>)value, propInfo.Bt, null); break;
					case MyPropInfoType.ByteArray: oset = Convert.FromBase64String((string)value); break;
					case MyPropInfoType.Hashtable: // same case as Dictionary
					case MyPropInfoType.Dictionary: oset = Deserializer.CreateDictionary((List<object>)value, propInfo.Pt, propInfo.GenericTypes, null); break;
					case MyPropInfoType.StringKeyDictionary: oset = Deserializer.CreateStringKeyDictionary((Dictionary<string, object>)value, propInfo.Pt, propInfo.GenericTypes, null); break;
					case MyPropInfoType.NameValue: oset = Deserializer.CreateNv((Dictionary<string, object>)value); break;
					case MyPropInfoType.StringDictionary: oset = Deserializer.CreateSd((Dictionary<string, object>)value); break;
					
					#region Unity_Build-in_Classes
					case MyPropInfoType.Vector2:
						oset = VectorFactory.CreateVector2((Dictionary<string, object>)value);
						break;
					case MyPropInfoType.Vector2Int:
						oset = VectorFactory.CreateVector2Int((Dictionary<string, object>)value);
						break;
					case MyPropInfoType.Vector3:
						oset = VectorFactory.CreateVector3((Dictionary<string, object>)value);
						break;
					case MyPropInfoType.Vector3Int:
						oset = VectorFactory.CreateVector3Int((Dictionary<string, object>)value);
						break;
					case MyPropInfoType.Vector4:
						oset = VectorFactory.CreateVector4((Dictionary<string, object>)value);
						break;
					case MyPropInfoType.Color:
						oset = UnityClassesFactoryMain.CreateColor((Dictionary<string, object>)value);
						break;
					case MyPropInfoType.Color32:
						oset = UnityClassesFactoryMain.CreateColor32((Dictionary<string, object>)value);
						break;
					case MyPropInfoType.Rect:
						oset = UnityClassesFactoryMain.CreateRect((Dictionary<string, object>)value);
						break;
					case MyPropInfoType.RectInt:
						oset = UnityClassesFactoryMain.CreateRectInt((Dictionary<string, object>)value);
						break;
					case MyPropInfoType.Bounds:
						oset = UnityClassesFactoryMain.CreateBounds((Dictionary<string, object>)value);
						break;
					case MyPropInfoType.BoundsInt:
						oset = UnityClassesFactoryMain.CreateBoundsInt((Dictionary<string, object>)value);
						break;
					case MyPropInfoType.Ray:
						oset = UnityClassesFactoryMain.CreateRay((Dictionary<string, object>)value);
						break;
					case MyPropInfoType.Ray2D:
						oset = UnityClassesFactoryMain.CreateRay2D((Dictionary<string, object>)value);
						break;
					case MyPropInfoType.Quaternion:
						oset = UnityClassesFactoryMain.CreateQuaternion((Dictionary<string, object>)value);
						break;
					
					#endregion
					case MyPropInfoType.Custom: oset = Reflection.Instance.CreateCustom((string)value, propInfo.Pt); break;
					case MyPropInfoType.Unknown:
						UnityEngine.Debug.LogError("Your class is unsupported: " + propInfo.Pt + ". Pls register with JSONObject.RegisterCustomType().");
						break;
					default:
					{
						if (propInfo.IsGenericType && propInfo.IsValueType == false && value is List<object>)
							oset = CreateList((List<object>)value, propInfo.Pt);

						else if ((propInfo.IsClass || propInfo.IsStruct || propInfo.IsInterface) && value is Dictionary<string, object>)
							oset = CreateClassFromDic((Dictionary<string, object>)value, propInfo.Pt, propInfo.Getter(result));

						else if (value is List<object>)
							oset = Deserializer.CreateArray((List<object>)value, typeof(object), null);

						else if (propInfo.IsValueType)
							oset = Deserializer.ChangeType(value, propInfo.ChangeType);

						else
							oset = value;
					}
						break;
				}

				result = propInfo.Setter(result, oset);
			}
			
			return result;
		}
	}
}