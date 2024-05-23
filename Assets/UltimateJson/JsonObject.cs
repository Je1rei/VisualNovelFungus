using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using fastJSON;
using fastJSON.CustomTypesFactory;
using JetBrains.Annotations;

namespace UltimateJson
{
	[Serializable]
	public class JsonObject : IEnumerable<JsonObject>, IEnumerable<KeyValuePair<string, JsonObject>>
	{
		private static bool _initialiseTypesMark;

		[SerializeField] private Dictionary<string, JsonObject> _dic;
		[SerializeField] private List<JsonObject> _list;
		[SerializeField] private JsonNodeObject _nodeObject;

		#region SystemTypes

		private static Type _longType;
		private static Type _doubleType;
		private static Type _boolType;
		private static Type _stringType;
		private static Type _listType;
		private static Type _dictionaryType;

		#endregion

		#region public Methods

		[NotNull]
		public JsonObject this[string i]
		{
			get
			{
				if (_dic != null)
				{
					return _dic[i];
				}

				Debug.LogWarning("JSONObject is null");
				return new JsonObject();
			}
			set
			{
				SetDicValue(i, value);
				SetFinishObjNewValue(i, value);
			}
		}

		private void SetDicValue(string key, JsonObject obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");

			if (_dic == null)
			{
				_dic = new Dictionary<string, JsonObject>();
			}

			if (!_dic.ContainsKey(key))
			{
				_dic.Add(key, obj);
			}
			else
			{
				_dic[key] = obj;
			}
		}

		private void SetFinishObjNewValue(string key, JsonObject obj)
		{
			if (!_initialiseTypesMark)
				InitialiseTypes();
			
			if (_nodeObject.ElementType != JsonElementType.Dictionary)
			{
				throw new ArgumentException("Pls change parent of this node to dictionary: " + _nodeObject.ElementType);
			}

			var valueObj = obj._nodeObject.FinishObj;
			var tempDic = (Dictionary<string, object>) _nodeObject.FinishObj;
			if (!tempDic.ContainsKey(key))
			{
				tempDic.Add(key, valueObj);
			}
			else
			{
				tempDic[key] = valueObj;
			}
		}

		public JsonObject this[int i]
		{
			get
			{
				if (_list != null)
				{
					return _list[i];
				}

				Debug.LogWarning("JSONObject is null");
				return new JsonObject();
			}

			set
			{
				SetListValue(i, value);

				SetFinishObjNewValue(i, value);
			}
		}

		private void SetListValue(int key, JsonObject obj)
		{
			if (obj == null) throw new ArgumentNullException("value");

			if (_list == null)
			{
				_list = new List<JsonObject>();
			}
			
			_list.Insert(key, obj);
		}

		private void SetFinishObjNewValue(int key, JsonObject obj)
		{
			if (!_initialiseTypesMark)
				InitialiseTypes();
			
			if (_nodeObject.ElementType != JsonElementType.List)
			{
				throw new ArgumentException("Pls change parent of this node to list: " + _nodeObject.ElementType);
			}

			var valueObj = obj._nodeObject.FinishObj;
			var tempList = (List<object>) _nodeObject.FinishObj;

			tempList.Insert(key, valueObj);
		}

		public override string ToString()
		{
			var objToString = _nodeObject.FinishObj;
			if (_nodeObject.ElementType == JsonElementType.String)
			{
				return objToString.ToString();
			}
			
			var result = objToString != null ? Json.ToJson(objToString) : "null";
			return result;
		}

		public T TryGetValue<T>()
		{
			var type = typeof(T);
			
			if (_nodeObject.ElementType == JsonElementType.Long)
			{
				if (type == typeof(int))
				{
					return (T)(object)(int)(long)_nodeObject.FinishObj;
				}

				if (type == typeof(long))
				{
					return (T)_nodeObject.FinishObj;
				}
				
				Debug.Log(type.Name);
			}

			if (_nodeObject.ElementType == JsonElementType.Bool)
			{
				if (type == typeof(bool))
				{
					return (T)_nodeObject.FinishObj;
				}
				
				Debug.Log(type.Name);
			}

			if (_nodeObject.ElementType == JsonElementType.Dictionary)
			{
				if (type == typeof(Dictionary<string, object>))
				{
					return (T) _nodeObject.FinishObj;
				}

				if (type == typeof(StringDictionary))
				{
					return (T) (object) ClassFactory.GetStringDictionaryValue(_nodeObject.FinishObj);
				}

				if (type == typeof(Dictionary<string, string>))
				{
					return (T) (object) ClassFactory.GetDictionaryStringStringValue(_nodeObject.FinishObj);
				}
				
				if (type == typeof(NameValueCollection))
				{
					return (T) (object) ClassFactory.GetNameValueCollectionValue(_nodeObject.FinishObj);
				}

				#region UNITY_TYPES
				if (type == typeof(Vector2))
				{
					return (T) (object)VectorFactory.CreateVector2((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				
				if (type == typeof(Vector2Int))
				{
					return (T) (object)VectorFactory.CreateVector2Int((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				
				if (type == typeof(Vector3))
				{
					return (T) (object)VectorFactory.CreateVector3((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				
				if (type == typeof(Vector3Int))
				{
					return (T) (object)VectorFactory.CreateVector3Int((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				
				if (type == typeof(Vector4))
				{
					return (T) (object)VectorFactory.CreateVector4((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				
				if (type == typeof(Color))
				{
					return (T) (object)UnityClassesFactoryMain.CreateColor((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				
				if (type == typeof(Color32))
				{
					return (T) (object)UnityClassesFactoryMain.CreateColor32((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				
				if (type == typeof(Rect))
				{
					return (T) (object)UnityClassesFactoryMain.CreateRect((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				
				if (type == typeof(RectInt))
				{
					return (T) (object)UnityClassesFactoryMain.CreateRectInt((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				
				if (type == typeof(Bounds))
				{
					return (T) (object)UnityClassesFactoryMain.CreateBounds((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				
				if (type == typeof(BoundsInt))
				{
					return (T) (object)UnityClassesFactoryMain.CreateBoundsInt((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				
				if (type == typeof(Ray))
				{
					return (T) (object)UnityClassesFactoryMain.CreateRay((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				
				if (type == typeof(Ray2D))
				{
					return (T) (object)UnityClassesFactoryMain.CreateRay2D((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				
				if (type == typeof(Quaternion))
				{
					return (T) (object)UnityClassesFactoryMain.CreateQuaternion((Dictionary<string, object>)_nodeObject.FinishObj);
				}
				#endregion
				
				var result = (T) CustomClassFactory.ParseCustomClass(_nodeObject.FinishObj, type);
				return result;
			}

			if (_nodeObject.ElementType == JsonElementType.List)
			{
				if (type == typeof(List<object>))
				{
					return (T)_nodeObject.FinishObj;
				}
				
				if (type == typeof(Hashtable))
				{
					return (T) (object) ClassFactory.GetHashtableValue(_nodeObject.FinishObj);
				}

				if (type == typeof(Array))
				{
					return (T) (object) ClassFactory.GetArrayValue(_nodeObject.FinishObj);
				}
				
				var result = (T) CustomClassFactory.ParseCustomClass(_nodeObject.FinishObj, type);
				return result;
			}
			
			if (_nodeObject.ElementType != JsonElementType.String)
			{
				//Debug.Log(_nodeObject.ElementType);
				return default(T);
			}
			
			var str = (string)_nodeObject.FinishObj;
			
			
			if (type == typeof(string))
			{
				return (T)(object)str;
			}
			
			if (type == typeof(DateTime))
			{
				var result = ClassFactory.GetDateTime(str);
				return (T)(object)result;
			}
			
			if (type == typeof(Guid))
			{
				var result = ClassFactory.GetGuidValue(str);
				return (T)(object)result;
			}
			
			if (type.IsEnum)
			{
				if (type == typeof(Enum))
				{
					Debug.LogError("Pls get concrete enum type, not abstract enum");
					return default(T);
				}
				
				var result = ClassFactory.GetEnumValue<T>(str);
				return result;
			}

			if (type == typeof(byte[]))
			{
				var result = ClassFactory.GetByteArrayValue(str);
				return (T)(object)result;
			}

			var result1 = (T) CustomClassFactory.ParseCustomClass(_nodeObject.FinishObj, type);
			return result1;
		}

		public IEnumerator GetEnumerator()
		{
			if (_list != null)
			{
				return ((IEnumerable<JsonObject>) this).GetEnumerator();
			}

			if (_dic != null)
			{
				return ((IEnumerable<KeyValuePair<string, JsonObject>>) this).GetEnumerator();
			}

			return null;
		}

		#endregion

		#region Constructors

		public JsonObject(long value)
		{
			_nodeObject = new JsonNodeObject(value);
		}

		public JsonObject(bool value)
		{
			_nodeObject = new JsonNodeObject(value);
		}

		public JsonObject(double value)
		{
			_nodeObject = new JsonNodeObject(value);
		}

		public JsonObject(string value)
		{
			_nodeObject = new JsonNodeObject(value);
		}
		
		//todo add constructors with stringDictionary

		private static List<object> GetCopy(IEnumerable<object> source)
		{
			if (!_initialiseTypesMark)
				InitialiseTypes();
			
			var newList = new List<object>();
			foreach (var keyValue in source)
			{
				var value = keyValue;
				var valueType = value.GetType();
				
				if (valueType == _dictionaryType)
				{
					var valueDic = (Dictionary<string, object>)value;
					value = GetCopy(valueDic);
				}
				else if (valueType == _listType)
				{
					var valueList = (List<object>)value;
					value = GetCopy(valueList);
				}
				
				newList.Add(value);
			}
			
			return newList;
		}

		private static Dictionary<string, object> GetCopy(Dictionary<string, object> source)
		{
			if (!_initialiseTypesMark)
				InitialiseTypes();
			
			var newDic = new Dictionary<string, object>();
			foreach (var keyValue in source)
			{
				var key = keyValue.Key;
				var value = keyValue.Value;
				var valueType = value.GetType();

				if (valueType == _dictionaryType)
				{
					var valueDic = (Dictionary<string, object>)value;
					value = GetCopy(valueDic);
				}
				else if (valueType == _listType)
				{
					var valueList = (List<object>)value;
					value = GetCopy(valueList);
				}

				newDic[key] = value;
			}
			
			return newDic;
		}
		
		public JsonObject(JsonObject value)
		{
			switch (value._nodeObject.ElementType)
			{
				case JsonElementType.Dictionary:
					var prevDic = (Dictionary<string, object>)value._nodeObject.FinishObj;
					var newDic = GetCopy(prevDic);
					_nodeObject = new JsonNodeObject(newDic);
					return;
				case JsonElementType.List:
					var prevList = (List<object>)value._nodeObject.FinishObj;
					var newList = GetCopy(prevList);
					_nodeObject = new JsonNodeObject(newList);
					return;
				case JsonElementType.Long:
				case JsonElementType.Double:
				case JsonElementType.Bool:
				case JsonElementType.String:
					_nodeObject = new JsonNodeObject(value._nodeObject);
					break;
				case JsonElementType.Null:
					throw new ArgumentOutOfRangeException();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private JsonObject()
		{
		}

		#endregion

		#region private Methods

		private void CreateListJsonObject(object jsonObj)
		{
			if (_list == null)
			{
				_list = new List<JsonObject>();
			}

			var obj = (List<object>) jsonObj;
			foreach (var t in obj)
			{
				var jObject = new JsonObject();
				_list.Add(jObject);
				jObject.CreateJsonObject(t);
			}
		}

		private void CreateDictionaryJsonObject(object jsonObj)
		{
			if (_dic == null)
			{
				const int len = 40;
				_dic = new Dictionary<string, JsonObject>(len, StringComparer.Ordinal);
			}

			var obj = (Dictionary<string, object>) jsonObj;

			foreach (var pair in obj)
			{
				var jObject = new JsonObject();
				_dic.Add(pair.Key, jObject);
				jObject.CreateJsonObject(pair.Value);
			}
		}

		private void CreateJsonObject(object jsonObj)
		{
			var elem = GetTypeOfElement(jsonObj);
			//Debug.Log("elem: " + elem);
			var node = GetJsonFinishNode(elem);
			//Debug.Log("node: " + node);
			
			if (node == JsonFinishNode.Null)
			{
				return;
			}

			_nodeObject = new JsonNodeObject
			{
				FinishObj = jsonObj,
				ElementType = elem
			};

			switch (node)
			{
				case JsonFinishNode.List:
					CreateListJsonObject(jsonObj);
					break;
				case JsonFinishNode.Dict:
					CreateDictionaryJsonObject(jsonObj);
					break;
				case JsonFinishNode.Finish:
					break;
				case JsonFinishNode.Null:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static void InitialiseTypes()
		{
			_longType = typeof(long);
			_doubleType = typeof(double);
			_boolType = typeof(bool);
			_stringType = typeof(string);
			_listType = typeof(List<object>);
			_dictionaryType = typeof(Dictionary<string, object>);

			_initialiseTypesMark = true;
		}

		private static JsonElementType GetTypeOfElement(object element)
		{
			if (element == null)
			{
				return JsonElementType.Null;
			}

			var elementType = element.GetType();
			//Debug.Log(elementType.Name + ": " + element);
			return TypeToJsonType(elementType);
		}

		private static JsonElementType TypeToJsonType(Type type)
		{
			if (!_initialiseTypesMark)
				InitialiseTypes();

			var valueBool = (type == _longType);
			if (valueBool)
				return JsonElementType.Long;

			valueBool = (type == _doubleType);
			if (valueBool)
				return JsonElementType.Double;

			valueBool = (type == _boolType);
			if (valueBool)
				return JsonElementType.Bool;

			valueBool = (type == _stringType);
			if (valueBool)
				return JsonElementType.String;

			valueBool = (type == _listType);
			if (valueBool)
				return JsonElementType.List;

			valueBool = (type == _dictionaryType);
			return valueBool ? JsonElementType.Dictionary : JsonElementType.Null;
		}

		private static JsonFinishNode GetJsonFinishNode(JsonElementType elem)
		{
			switch (elem)
			{
				case JsonElementType.Bool:
					return JsonFinishNode.Finish;
				case JsonElementType.Dictionary:
					return JsonFinishNode.Dict;
				case JsonElementType.Double:
					return JsonFinishNode.Finish;
				case JsonElementType.List:
					return JsonFinishNode.List;
				case JsonElementType.Long:
					return JsonFinishNode.Finish;
				case JsonElementType.Null:
					return JsonFinishNode.Finish;
				case JsonElementType.String:
					return JsonFinishNode.Finish;
				default:
					return JsonFinishNode.Null;
			}
		}

		IEnumerator<JsonObject> IEnumerable<JsonObject>.GetEnumerator()
		{
			if (_list == null)
			{
				return null;
			}

			return _list.GetEnumerator();
		}

		IEnumerator<KeyValuePair<string, JsonObject>> IEnumerable<KeyValuePair<string, JsonObject>>.GetEnumerator()
		{
			if (_dic == null)
			{
				return null;
			}

			return _dic.GetEnumerator();
		}

		#endregion

		#region public static methods

		public static string Serialise(object obj, bool addAssemblyTypes = false)
		{
			var jsonParams = new JsonParameters
			{
				UseExtensions = addAssemblyTypes,
			};

			return Json.ToJson(obj, jsonParams);
		}

		public static JsonObject Deserialise(string jsonString)
		{
			var jsonObject = new JsonObject();

			var jsonData = Json.Parse(jsonString);
			jsonObject.CreateJsonObject(jsonData);

			return jsonObject;
		}

		public static T Deserialise<T>(string jsonString, bool addAssemblyTypes = false) where T : class
		{
			var jsonParams = new JsonParameters
			{
				UseExtensions = addAssemblyTypes,
			};

			return Json.ToObject<T>(jsonString, jsonParams);
		}

		/// <summary>
		/// Register custom type handlers for your own types not natively handled by UltimateJSON
		/// </summary>
		/// <param name="type"></param>
		/// <param name="serializer"></param>
		/// <param name="deserializer"></param>
		public static void RegisterCustomType(Type type, Serialize serializer, Deserialize deserializer)
		{
			Reflection.Instance.RegisterCustomType(type, serializer, deserializer);
		}

		#endregion
	}


	public enum JsonElementType
	{
		Long,
		Double,
		Bool,
		String,
		List,
		Dictionary,
		Null
	}

	public enum JsonFinishNode
	{
		Finish,
		List,
		Dict,
		Null
	}
}