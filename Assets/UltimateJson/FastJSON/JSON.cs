using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Collections.Specialized;
using UnityEngine;

namespace fastJSON
{
    public delegate string Serialize(object data);
    public delegate object Deserialize(string data);

    public sealed class JsonParameters
    {
		/// <summary>
        /// Use the $types extension to optimise the output json (default = True)
        /// </summary>
        public bool UsingGlobalTypes = true;

		/// <summary>
        /// Enable fastJSON extensions $types, $type, $map (default = True)
        /// </summary>
        public bool UseExtensions = true;
		/// <summary>
		/// Use escaped unicode i.e. \uXXXX format for non ASCII characters (default = True)
		/// </summary>
		public const bool UseEscapedUnicode = true;

		/// <summary>
        /// Ignore attributes to check for (default : XmlIgnoreAttribute, NonSerialized)
        /// </summary>
        public readonly List<Type> IgnoreAttributes = new List<Type> { typeof(System.Xml.Serialization.XmlIgnoreAttribute), typeof(NonSerializedAttribute) };

		/// <summary>
		/// Maximum depth for circular references in inline mode (default = 20)
		/// </summary>
		public const byte SerializerMaxDepth = 20;
		/// <summary>
        /// Inline circular or already seen objects instead of replacement with $i (default = False) 
        /// </summary>
        public bool InlineCircularReferences;

		public void FixValues()
        {
			if (UseExtensions) return;
			
			UsingGlobalTypes = false;
			InlineCircularReferences = true;
		}
    }

    public static class Json
    {
        /// <summary>
        /// Globally set-able parameters for controlling the serializer
        /// </summary>
		private static readonly JsonParameters Parameters = new JsonParameters();

		/// <summary>
        /// Create a json representation for an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(object obj)
        {
            return ToJson(obj, Parameters);
        }
        /// <summary>
        /// Create a json representation for an object with parameter override on this call
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ToJson(object obj, JsonParameters param)
        {
            param.FixValues();
            Type t = null;

            if (obj == null)
                return "null";

            if (obj.GetType().IsGenericType)
                t = Reflection.Instance.GetGenericTypeDefinition(obj.GetType());
            if (t == typeof(Dictionary<,>) || t == typeof(List<>))
                param.UsingGlobalTypes = false;

			//Debug.Log(obj.GetType().Name);
            return new JsonSerializer(param).ConvertToJson(obj);
        }
        /// <summary>
        /// Parse a json string and generate a Dictionary&lt;string,object&gt; or List&lt;object&gt; structure
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object Parse(string json)
        {
            return new JsonParser(json).Decode();
        }

		/// <summary>
        /// Create a typed generic object from the json with parameter override on this call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T ToObject<T>(string json, JsonParameters param)
        {
            return new Deserializer(param).ToObject<T>(json);
        }

		internal static long CreateLong(string s, int index, int count)
        {
            long num = 0;
            var neg = false;
            for (var x = 0; x < count; x++, index++)
			{
				var cc = s[index];

				switch (cc)
				{
					case '-':
						neg = true;
						break;
					case '+':
						neg = false;
						break;
					default:
						num *= 10;
						num += cc - '0';
						break;
				}
			}
            if (neg) num = -num;

            return num;
        }
    }

    internal class Deserializer
    {
        public Deserializer(JsonParameters param)
        {
            _params = param;
        }

        private readonly JsonParameters _params;
        private bool _usingglobals;
        private readonly Dictionary<object, int> _circobj = new Dictionary<object, int>();

        public T ToObject<T>(string json)
        {
            var t = typeof(T);
            var o = ToObject(json, t);

			if (!t.IsArray) return (T) o;
			
			if (((ICollection) o).Count != 0) return (T) o;
				
			var tt = t.GetElementType();
			if (tt == null) return (T) o;
				
			object oo = Array.CreateInstance(tt, 0);
			return (T)oo;
		}

		private object ToObject(string json, Type type = null)
        {
            //_params = Parameters;
            _params.FixValues();
            Type t = null;
            if (type != null && type.IsGenericType)
                t = Reflection.Instance.GetGenericTypeDefinition(type);
            if (t == typeof(Dictionary<,>) || t == typeof(List<>))
                _params.UsingGlobalTypes = false;
            _usingglobals = _params.UsingGlobalTypes;

            var o = new JsonParser(json).Decode();
			
            if (o == null)
                return null;
            if (o is IDictionary)
            {
                if (type != null && t == typeof(Dictionary<,>)) // deserialize a dictionary
                    return RootDictionary(o, type);
                return ParseDictionary(o as Dictionary<string, object>, null, type, null);
            }

            var list = o as List<object>;
            if (list != null)
            {
                if (type != null && t == typeof(Dictionary<,>)) // kv format
                    return RootDictionary(list, type);
				if (type != null && t == typeof(List<>)) // deserialize to generic list
				{
					//Debug.Log("rootList");
					return RootList(list, type);
				}  
                if (type != null && type.IsArray)
                    return RootArray(list, type);
                return type == typeof(Hashtable) ? RootHashTable(list) : list.ToArray();
            }

            if (type != null && o.GetType() != type)
                return ChangeType(o, type);

            return o;
        }

        #region [   p r i v a t e   m e t h o d s   ]
        private object RootHashTable(IEnumerable<object> o)
        {
            var h = new Hashtable();

            foreach (Dictionary<string, object> values in o)
            {
                var key = values["k"];
                var val = values["v"];
                if (key is Dictionary<string, object>)
                    key = ParseDictionary((Dictionary<string, object>)key, null, typeof(object), null);

                if (val is Dictionary<string, object>)
                    val = ParseDictionary((Dictionary<string, object>)val, null, typeof(object), null);

                h.Add(key, val);
            }

            return h;
        }

        public static object ChangeType(object value, Type conversionType)
        {
            if (conversionType == typeof(int))
            {
                var s = value as string;
                if (s == null)
                    return (int)((long)value);
				
				return CreateInteger(s, 0, s.Length);
			}

			if (conversionType == typeof(long))
			{
				var s = value as string;
				if (s == null)
					return (long)value;
				
				return Json.CreateLong(s, 0, s.Length);
			}

			if (conversionType == typeof(string))
				return (string)value;

			if (conversionType.IsEnum)
				return CreateEnum(conversionType, value);

			if (conversionType == typeof(DateTime))
				return CreateDateTime((string)value);

			if (conversionType == typeof(DateTimeOffset))
				return CreateDateTimeOffset((string)value);

			if (Reflection.Instance.IsTypeRegistered(conversionType))
				return Reflection.Instance.CreateCustom((string)value, conversionType);

			// 8-30-2014 - James Brooks - Added code for nullable types.
            if (IsNullable(conversionType))
            {
                if (value == null)
                    return null;
                conversionType = UnderlyingTypeOf(conversionType);
            }

            // 8-30-2014 - James Brooks - Nullable Guid is a special case so it was moved after the "IsNullable" check.
            if (conversionType == typeof(Guid))
                return CreateGuid((string)value);

            // 2016-04-02 - Enrico Padovani - proper conversion of byte[] back from string
            return conversionType == typeof(byte[]) ? Convert.FromBase64String((string)value) : 
				Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
		}

        public static object CreateDateTimeOffset(string value)
        {
            //                   0123456789012345678 9012 9/3 0/4  1/5
            // datetime format = yyyy-MM-ddTHH:mm:ss .nnn  _   +   00:00
			var ms = 0;

			var year = CreateInteger(value, 0, 4);
            var month = CreateInteger(value, 5, 2);
            var day = CreateInteger(value, 8, 2);
            var hour = CreateInteger(value, 11, 2);
            var min = CreateInteger(value, 14, 2);
            var sec = CreateInteger(value, 17, 2);

            if (value.Length > 21 && value[19] == '.')
                ms = CreateInteger(value, 20, 3);
            var p = 20;
            if (ms > 0)
                p = 24;
            var th = CreateInteger(value, p + 1, 2);
            var tm = CreateInteger(value, p + 1 + 2 + 1, 2);

            if (value[p] == '-')
                th = -th;

            return new DateTimeOffset(year, month, day, hour, min, sec, ms, new TimeSpan(th,tm,0));
        }

        private static bool IsNullable(Type t)
        {
            if (!t.IsGenericType) return false;
            var g = t.GetGenericTypeDefinition();
            return g == typeof(Nullable<>);
        }

        private static Type UnderlyingTypeOf(Type t)
        {
            return t.GetGenericArguments()[0];
        }

        private object RootList(object parse, Type type)
        {
            var gtypes = Reflection.Instance.GetGenericArguments(type);
            var o = (IList)Reflection.Instance.FastCreateInstance(type);
            DoParseList(parse, gtypes[0], o);
            return o;
        }

        private void DoParseList(object parse, Type elementType, IList o)
        {
            foreach (var k in (IList)parse)
            {
				//Debug.Log(k.GetType());
                _usingglobals = false;
				var objects = k as Dictionary<string, object>;
				object v;
				if (objects != null)
				{
					//Debug.Log("dict");
					v = ParseDictionary(objects, null, elementType, null);
				}
				else
				{
					//Debug.Log("not Dict");
					v = ChangeType(k, elementType);
				}

                o.Add(v);
            }
        }

        private object RootArray(object parse, Type type)
        {
            var it = type.GetElementType();
            var o = (IList)Reflection.Instance.FastCreateInstance(typeof(List<>).MakeGenericType(it));
            DoParseList(parse, it, o);
			if (it == null) return null;
			
			var array = Array.CreateInstance(it, o.Count);
			o.CopyTo(array, 0);

			return array;
		}

        private static object RootDictionary(object parse, Type type)
        {
            var gtypes = Reflection.Instance.GetGenericArguments(type);
            Type t1 = null;
            Type t2 = null;
            if (gtypes != null)
            {
                t1 = gtypes[0];
                t2 = gtypes[1];
            }

			if (t2 != null)
			{
				var arraytype = t2.GetElementType();
				var kvs = parse as Dictionary<string, object>;
				if (kvs != null)
				{
					var o = (IDictionary)Reflection.Instance.FastCreateInstance(type);
					foreach (var kv in kvs)
					{
						object v;
						var k = ChangeType(kv.Key, t1);

						var objects = kv.Value as Dictionary<string, object>;
						if (objects != null)
							v = ParseDictionary(objects, null, t2, null);

						else if (t2.IsArray && t2 != typeof(byte[]))
							v = CreateArray((List<object>)kv.Value, arraytype, null);

						else if (kv.Value is IList)
							v = CreateGenericList((List<object>)kv.Value, t2, t1, null);

						else
							v = ChangeType(kv.Value, t2);

						o.Add(k, v);
					}

					return o;
				}
			}

			var list = parse as List<object>;
			return list != null ? CreateDictionary(list, type, gtypes, null) : null;
		}

		private static object ParseDictionary(Dictionary<string, object> d, Dictionary<string, object> globaltypes, Type type, object input)
        {
			//Debug.Log(type);
			if (type == typeof(NameValueCollection))
                return CreateNv(d);
            if (type == typeof(StringDictionary))
                return CreateSd(d);

            if (type == null)
                throw new Exception("Cannot determine type");

            var typename = type.FullName;
            var o = input ?? (Reflection.Instance.FastCreateInstance(type));

            var props = Reflection.Instance.Getproperties(type, typename);//, Reflection.Instance.IsTypeRegistered(type));
			//Debug.Log(d.Count);
            foreach (var kv in d)
            {
                var n = kv.Key;
                var v = kv.Value;
				var name = Reflection.CleanUpName(n);
                if (name == "$map")
                {
                    ProcessMap(o, props, (Dictionary<string, object>)d[name]);
                    continue;
                }
                MyPropInfo pi;
				if (props.TryGetValue(name, out pi) == false)
				{
					Debug.Log(name);
					continue;
				}

                if (!pi.CanWrite) continue;

                if (v == null) continue;
                object oset = null;
				
				//Debug.Log(name);
				//Debug.Log(v.GetType());
				//Debug.Log(pi.Type);
                switch (pi.Type)
                {
                    case MyPropInfoType.Int: oset = (int)(long)v; break;
					
                    case MyPropInfoType.Float: oset = (float)TryGetDoubleValue(v); break;
                    case MyPropInfoType.Char: oset = ((string)v)[0]; break;
                    case MyPropInfoType.Byte: oset = (byte)(long)v; break;
                    case MyPropInfoType.Decimal: oset = (decimal)TryGetDoubleValue(v); break;
                    case MyPropInfoType.Double: oset = TryGetDoubleValue(v); break;
                    case MyPropInfoType.Short: oset = (short)(long)v; break;
					
                    case MyPropInfoType.Long: oset = (long)v; break;
                    case MyPropInfoType.String: oset = (string)v; break;
                    case MyPropInfoType.Bool: oset = (bool)v; break;
                    case MyPropInfoType.DateTime: oset = CreateDateTime((string)v); break;
                    case MyPropInfoType.Enum: oset = CreateEnum(pi.Pt, v); break;
                    case MyPropInfoType.Guid: oset = CreateGuid((string)v); break;
					
					case MyPropInfoType.List: oset = CreateGenericList((List<object>) v, pi.Pt, pi.Bt, globaltypes); break;
					
                    case MyPropInfoType.Array:
                        if (!pi.IsValueType)
                            oset = CreateArray((List<object>)v, pi.Bt, globaltypes);
                        // what about 'else'?
                        break;
                    case MyPropInfoType.ByteArray: oset = Convert.FromBase64String((string)v); break;
                    case MyPropInfoType.Hashtable: // same case as Dictionary
                    case MyPropInfoType.Dictionary: oset = CreateDictionary((List<object>)v, pi.Pt, pi.GenericTypes, globaltypes); break;
                    case MyPropInfoType.StringKeyDictionary: oset = CreateStringKeyDictionary((Dictionary<string, object>)v, pi.Pt, pi.GenericTypes, globaltypes); break;
                    case MyPropInfoType.NameValue: oset = CreateNv((Dictionary<string, object>)v); break;
                    case MyPropInfoType.StringDictionary: oset = CreateSd((Dictionary<string, object>)v); break;
                        
                    #region Unity_Build-in_Classes
					case MyPropInfoType.Vector2:
						oset = VectorFactory.CreateVector2((Dictionary<string, object>)v);
						break;
					case MyPropInfoType.Vector2Int:
						oset = VectorFactory.CreateVector2Int((Dictionary<string, object>)v);
						break;
					case MyPropInfoType.Vector3:
						oset = VectorFactory.CreateVector3((Dictionary<string, object>)v);
						break;
					case MyPropInfoType.Vector3Int:
						oset = VectorFactory.CreateVector3Int((Dictionary<string, object>)v);
						break;
					case MyPropInfoType.Vector4:
						oset = VectorFactory.CreateVector4((Dictionary<string, object>)v);
						break;
					case MyPropInfoType.Color:
						oset = UnityClassesFactoryMain.CreateColor((Dictionary<string, object>)v);
						break;
					case MyPropInfoType.Color32:
						oset = UnityClassesFactoryMain.CreateColor32((Dictionary<string, object>)v);
						break;
					case MyPropInfoType.Rect:
						oset = UnityClassesFactoryMain.CreateRect((Dictionary<string, object>)v);
						break;
					case MyPropInfoType.RectInt:
						oset = UnityClassesFactoryMain.CreateRectInt((Dictionary<string, object>)v);
						break;
					case MyPropInfoType.Bounds:
						oset = UnityClassesFactoryMain.CreateBounds((Dictionary<string, object>)v);
						break;
					case MyPropInfoType.BoundsInt:
						oset = UnityClassesFactoryMain.CreateBoundsInt((Dictionary<string, object>)v);
						break;
					case MyPropInfoType.Ray:
						oset = UnityClassesFactoryMain.CreateRay((Dictionary<string, object>)v);
						break;
                    case MyPropInfoType.Ray2D:
                        oset = UnityClassesFactoryMain.CreateRay2D((Dictionary<string, object>)v);
                        break;
                    case MyPropInfoType.Quaternion:
                        oset = UnityClassesFactoryMain.CreateQuaternion((Dictionary<string, object>)v);
                        break;
					
                    #endregion
                    
                    case MyPropInfoType.Custom: oset = Reflection.Instance.CreateCustom((string)v, pi.Pt); break;
//                    case MyPropInfoType.Unknown:
//                        Debug.LogError("Your class is unsupported: " + pi.Pt + ". Pls register with JSONObject.RegisterCustomType().");
//                        break;
                    default:
                    {
                        if (pi.IsGenericType && pi.IsValueType == false && v is List<object>)
                            oset = CreateGenericList((List<object>)v, pi.Pt, pi.Bt, globaltypes);

                        else if ((pi.IsClass || pi.IsStruct || pi.IsInterface) && v is Dictionary<string, object>)
						{
							//Debug.Log("ParseDictionary");
							oset = ParseDictionary((Dictionary<string, object>)v, globaltypes, pi.Pt, pi.Getter(o));
						}
                        else if (v is List<object>)
                            oset = CreateArray((List<object>)v, typeof(object), globaltypes);

                        else if (pi.IsValueType)
                            oset = ChangeType(v, pi.ChangeType);

                        else
                            oset = v;
                    }
                        break;
                }

                o = pi.Setter(o, oset);
            }
            return o;
        }

		public static double TryGetDoubleValue(object v)
		{
			var vType = v.GetType();
			if (vType == typeof(long))
			{
				return (long) v;
			}

			return (double) v;
		}

		public static StringDictionary CreateSd(Dictionary<string, object> d)
        {
            var nv = new StringDictionary();

            foreach (var o in d)
                nv.Add(o.Key, (string)o.Value);

            return nv;
        }

		public static NameValueCollection CreateNv(Dictionary<string, object> d)
        {
            var nv = new NameValueCollection();

            foreach (var o in d)
                nv.Add(o.Key, (string)o.Value);

            return nv;
        }

        private static void ProcessMap(object obj, IDictionary<string, MyPropInfo> props, Dictionary<string, object> dic)
        {
            foreach (var kv in dic)
            {
                var p = props[kv.Key];
                var o = p.Getter(obj);
                var t = Type.GetType((string)kv.Value);
                if (t == typeof(Guid))
                    p.Setter(obj, CreateGuid((string)o));
            }
        }

		private static int CreateInteger(string s, int index, int count)
        {
            var num = 0;
            var neg = false;
            for (var x = 0; x < count; x++, index++)
            {
                var cc = s[index];

                switch (cc)
                {
                    case '-':
                        neg = true;
                        break;
                    case '+':
                        neg = false;
                        break;
                    default:
                        num *= 10;
                        num += cc - '0';
                        break;
                }
            }
            if (neg) num = -num;

            return num;
        }

		public static object CreateEnum(Type pt, object v)
        {
            // FEATURE : optimize create enum
            return Enum.Parse(pt, v.ToString());
        }

		public static Guid CreateGuid(string s)
        {
            return s.Length > 30 ? new Guid(s) : new Guid(Convert.FromBase64String(s));
        }

        public static DateTime CreateDateTime(string value)
        {
			//                   0123456789012345678 9012 9/3
            // datetime format = yyyy-MM-ddTHH:mm:ss .nnn  Z
            var ms = 0;

            var year = CreateInteger(value, 0, 4);
            var month = CreateInteger(value, 5, 2);
            var day = CreateInteger(value, 8, 2);
            var hour = CreateInteger(value, 11, 2);
            var min = CreateInteger(value, 14, 2);
            var sec = CreateInteger(value, 17, 2);
            if (value.Length > 21 && value[19] == '.')
                ms = CreateInteger(value, 20, 3);

			return new DateTime(year, month, day, hour, min, sec, ms, DateTimeKind.Utc).ToLocalTime();
        }

        public static object CreateArray(IList<object> data, Type bt, Dictionary<string, object> globalTypes)
        {
            if (bt == null)
                bt = typeof(object);

            var col = Array.CreateInstance(bt, data.Count);
            var arraytype = bt.GetElementType();
            // create an array of objects
            for (var i = 0; i < data.Count; i++)
            {
                var ob = data[i];
                if (ob == null)
                {
                    continue;
                }
                if (ob is IDictionary)
                    col.SetValue(ParseDictionary((Dictionary<string, object>)ob, globalTypes, bt, null), i);
                else if (ob is ICollection)
                    col.SetValue(CreateArray((List<object>)ob, arraytype, globalTypes), i);
                else
                    col.SetValue(ChangeType(ob, bt), i);
            }

            return col;
        }

        private static object CreateGenericList(List<object> data, Type pt, Type bt, Dictionary<string, object> globalTypes)
        {
			if (pt == typeof(object)) return data;
			
			var col = (IList)Reflection.Instance.FastCreateInstance(pt);
			var it = pt.GetGenericArguments()[0];
			// create an array of objects
			foreach (var ob in data)
			{
				if (ob is IDictionary)
					col.Add(ParseDictionary((Dictionary<string, object>)ob, globalTypes, bt, null));

				else if (ob is List<object>)
				{
					if (bt.IsGenericType)
						col.Add((List<object>)ob);
					else
						col.Add(((List<object>)ob).ToArray());
				}
				else
					col.Add(ChangeType(ob, it));
			}
			return col;
		}

		public static object CreateStringKeyDictionary(Dictionary<string, object> reader, Type pt, IList<Type> types, Dictionary<string, object> globalTypes)
        {
            var col = (IDictionary)Reflection.Instance.FastCreateInstance(pt);
			Type t2 = null;
            if (types != null)
                t2 = types[1];

            Type generictype = null;
			if (t2 != null)
			{
				var ga = t2.GetGenericArguments();
				if (ga.Length > 0)
					generictype = ga[0];
			}

			if (t2 == null) return col;
			
			var arraytype = t2.GetElementType();

			foreach (var values in reader)
			{
				var key = values.Key;
				object val;

				var value = values.Value as Dictionary<string, object>;
				if (value != null)
					val = ParseDictionary(value, globalTypes, t2, null);

				else if (t2.IsArray)
				{
					val = values.Value is Array ? values.Value : 
						CreateArray((List<object>) values.Value, arraytype, globalTypes);
				}
				else if (values.Value is IList)
					val = CreateGenericList((List<object>) values.Value, t2, generictype, globalTypes);

				else
					val = ChangeType(values.Value, t2);

				col.Add(key, val);
			}

			return col;
        }

        public static object CreateDictionary(IEnumerable<object> reader, Type pt, IList<Type> types, Dictionary<string, object> globalTypes)
        {
            var col = (IDictionary)Reflection.Instance.FastCreateInstance(pt);
            Type t1 = null;
            Type t2 = null;
            if (types != null)
            {
                t1 = types[0];
                t2 = types[1];
            }

            foreach (Dictionary<string, object> values in reader)
            {
                var key = values["k"];
                var val = values["v"];

				var objects = key as Dictionary<string, object>;
				key = objects != null ? ParseDictionary(objects, globalTypes, t1, null) : ChangeType(key, t1);

                if (typeof(IDictionary).IsAssignableFrom(t2))
                    val = RootDictionary(val, t2);
                else if (val is Dictionary<string, object>)
                    val = ParseDictionary((Dictionary<string, object>)val, globalTypes, t2, null);
                else
                    val = ChangeType(val, t2);

                col.Add(key, val);
            }

            return col;
        }
		
        #endregion
    }

}