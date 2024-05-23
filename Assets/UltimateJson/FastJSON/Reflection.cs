using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

namespace fastJSON
{
    internal struct Getters
    {
        public string Name;
        public string LcName;
        public Reflection.GenericGetter Getter;
    }

    internal enum MyPropInfoType
    {
        Int,
        Long,
        String,
        Bool,
        DateTime,
        Enum,
        Guid,
		
		Float,
		Char,
		Byte,
		Decimal,
		Double,
		Short,

        Array,
        ByteArray,
		List,
        Dictionary,
        StringKeyDictionary,
        NameValue,
        StringDictionary,
        Hashtable,
        Custom,
        Unknown,
        
		
		Vector2,
		Vector2Int,
        Vector3,
		Vector3Int,
		Vector4,
		
		Color,
		Color32,
		
		Rect,
		RectInt,
		
		Bounds,
		BoundsInt,
		
        Quaternion,
		Ray,
		Ray2D
    }

    internal struct MyPropInfo
    {
        public Type Pt;
        public Type Bt;
        public Type ChangeType;
        public Reflection.GenericSetter Setter;
        public Reflection.GenericGetter Getter;
        public Type[] GenericTypes;
        public string Name;
        public MyPropInfoType Type;
        public bool CanWrite;

        public bool IsClass;
        public bool IsValueType;
        public bool IsGenericType;
        public bool IsStruct;
        public bool IsInterface;
    }

    internal sealed class Reflection
    {
        // Sinlgeton pattern 4 from : http://csharpindepth.com/articles/general/singleton.aspx
		// ReSharper disable once InconsistentNaming
        private static readonly Reflection instance = new Reflection();
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Reflection()
        {
        }
        private Reflection()
        {
        }
        public static Reflection Instance { get { return instance; } }

        internal delegate object GenericSetter(object target, object value);
        internal delegate object GenericGetter(object obj);
        private delegate object CreateObject();

        private SafeDictionary<Type, string> _tyname = new SafeDictionary<Type, string>();
        private SafeDictionary<string, Type> _typecache = new SafeDictionary<string, Type>();
        private SafeDictionary<Type, CreateObject> _constrcache = new SafeDictionary<Type, CreateObject>();
        private SafeDictionary<Type, Getters[]> _getterscache = new SafeDictionary<Type, Getters[]>();
        private SafeDictionary<string, Dictionary<string, MyPropInfo>> _propertycache = new SafeDictionary<string, Dictionary<string, MyPropInfo>>();
        private SafeDictionary<Type, Type[]> _genericTypes = new SafeDictionary<Type, Type[]>();
        private SafeDictionary<Type, Type> _genericTypeDef = new SafeDictionary<Type, Type>();
		
		private const BindingFlags Bf = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

        #region json custom types
        // JSON custom
        internal readonly SafeDictionary<Type, Serialize> CustomSerializer = new SafeDictionary<Type, Serialize>();
		private readonly SafeDictionary<Type, Deserialize> _customDeserializer = new SafeDictionary<Type, Deserialize>();

        internal object CreateCustom(string v, Type type)
        {
            Deserialize d;
            _customDeserializer.TryGetValue(type, out d);
            return d(v);
        }

        internal void RegisterCustomType(Type type, Serialize serializer, Deserialize deserializer)
        {
			if (type == null || serializer == null || deserializer == null) return;
			
			CustomSerializer.Add(type, serializer);
			_customDeserializer.Add(type, deserializer);
			// reset property cache
			Instance.ResetPropertyCache();
		}

        internal bool IsTypeRegistered(Type t)
        {
            if (CustomSerializer.Count == 0)
                return false;
			
            Serialize s;
            return CustomSerializer.TryGetValue(t, out s);
        }
        #endregion

        public Type GetGenericTypeDefinition(Type t)
        {
            Type tt;
			
            if (_genericTypeDef.TryGetValue(t, out tt))
                return tt;

			tt = t.IsGenericType ? t.GetGenericTypeDefinition() : t;
			_genericTypeDef.Add(t, tt);
			
			return tt;
		}

        public Type[] GetGenericArguments(Type t)
        {
            Type[] tt;
            if (_genericTypes.TryGetValue(t, out tt))
                return tt;

			tt = t.GetGenericArguments();
			_genericTypes.Add(t, tt);
			
			return tt;
		}

		private static readonly char[] RemovedCharacters = {'-'};
		
		public static string CleanUpName(string name)
		{
			return new string(name.Select(char.ToLower).Where(x=>!RemovedCharacters.Contains(x)).ToArray());
		}

        public Dictionary<string, MyPropInfo> Getproperties(Type type, string typename)
        {
            Dictionary<string, MyPropInfo> sd;
            if (_propertycache.TryGetValue(typename, out sd))
            {
                return sd;
            }

			sd = new Dictionary<string, MyPropInfo>();
			
			
			var pr = type.GetProperties(Bf);
			foreach (var p in pr)
			{
				if (p.GetIndexParameters().Length > 0)// Property is an indexer
					continue;

				//Debug.Log(p.PropertyType);
				var d = CreateMyProp(p.PropertyType, p.Name);
				d.Setter = CreateSetMethod(type, p);
				if (d.Setter != null)
					d.CanWrite = true;
				d.Getter = CreateGetMethod(type, p);
				var cleanedName = CleanUpName(p.Name);
				sd.Add(cleanedName, d);
			}
			
			var fi = type.GetFields(Bf);
			foreach (var f in fi)
			{
				var d = CreateMyProp(f.FieldType, f.Name);
				if (f.IsLiteral) continue;
				
				d.Setter = CreateSetField(type, f);
				if (d.Setter != null)
					d.CanWrite = true;
				
				d.Getter = CreateGetField(type, f);
				var cleanedName = CleanUpName(f.Name);
				sd.Add(cleanedName, d);
			}

			_propertycache.Add(typename, sd);
			return sd;
		}

        private MyPropInfo CreateMyProp(Type t, string name)
        {
            var d = new MyPropInfo();
            var dType = MyPropInfoType.Unknown;

			//Debug.Log(name + ": " + t);
            if (t == typeof(int) || t == typeof(int?)) dType = MyPropInfoType.Int;
            else if (t == typeof(long) || t == typeof(long?)) dType = MyPropInfoType.Long;
			
			else if (t == typeof(float) || t == typeof(float?)) dType = MyPropInfoType.Float;
			else if (t == typeof(char) || t == typeof(char?)) dType = MyPropInfoType.Char;
			else if (t == typeof(byte) || t == typeof(byte?)) dType = MyPropInfoType.Byte;
			else if (t == typeof(decimal) || t == typeof(decimal?)) dType = MyPropInfoType.Decimal;
			else if (t == typeof(double) || t == typeof(double?)) dType = MyPropInfoType.Double;
			else if (t == typeof(short) || t == typeof(short?)) dType = MyPropInfoType.Short;
			
			else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>)) dType = MyPropInfoType.List;
			
            else if (t == typeof(string)) dType = MyPropInfoType.String;
            else if (t == typeof(bool) || t == typeof(bool?)) dType = MyPropInfoType.Bool;
            else if (t == typeof(DateTime) || t == typeof(DateTime?)) dType = MyPropInfoType.DateTime;
            else if (t.IsEnum) dType = MyPropInfoType.Enum;
            else if (t == typeof(Guid) || t == typeof(Guid?)) dType = MyPropInfoType.Guid;
            else if (t == typeof(StringDictionary)) dType = MyPropInfoType.StringDictionary;
            else if (t == typeof(NameValueCollection)) dType = MyPropInfoType.NameValue;
            else if (t.IsArray)
            {
                d.Bt = t.GetElementType();
                dType = t == typeof(byte[]) ? MyPropInfoType.ByteArray : MyPropInfoType.Array;
            }
            else if (t.Name.Contains("Dictionary"))
            {
                d.GenericTypes = Instance.GetGenericArguments(t);
                if (d.GenericTypes.Length > 0 && d.GenericTypes[0] == typeof(string))
                    dType = MyPropInfoType.StringKeyDictionary;
                else
                    dType = MyPropInfoType.Dictionary;
            }
            else if (t == typeof(Hashtable)) dType = MyPropInfoType.Hashtable;
            
            #region Unity_Build-in_Classes
			
			else if (t == typeof(Vector2)) dType = MyPropInfoType.Vector2;
			else if (t == typeof(Vector2Int)) dType = MyPropInfoType.Vector2Int;
			else if (t == typeof(Vector3)) dType = MyPropInfoType.Vector3;
			else if (t == typeof(Vector3Int)) dType = MyPropInfoType.Vector3Int;
			else if (t == typeof(Vector4)) dType = MyPropInfoType.Vector4;
			
			else if (t == typeof(Color)) dType = MyPropInfoType.Color;
			else if (t == typeof(Color32)) dType = MyPropInfoType.Color32;
			
			else if (t == typeof(Rect)) dType = MyPropInfoType.Rect;
			else if (t == typeof(RectInt)) dType = MyPropInfoType.RectInt;
			
			else if (t == typeof(Bounds)) dType = MyPropInfoType.Bounds;
			else if (t == typeof(BoundsInt)) dType = MyPropInfoType.BoundsInt;
			
            else if (t == typeof(Quaternion)) dType = MyPropInfoType.Quaternion;
            else if (t == typeof(Ray)) dType = MyPropInfoType.Ray;
            else if (t == typeof(Ray2D)) dType = MyPropInfoType.Ray2D;
			
            #endregion
            
            else if (IsTypeRegistered(t))
                dType = MyPropInfoType.Custom;

            if (t.IsValueType && !t.IsPrimitive && !t.IsEnum && t != typeof(decimal))
                d.IsStruct = true;

            d.IsInterface = t.IsInterface;
            d.IsClass = t.IsClass;
            d.IsValueType = t.IsValueType;
            if (t.IsGenericType)
            {
                d.IsGenericType = true;
                d.Bt = t.GetGenericArguments()[0];
            }

            d.Pt = t;
            d.Name = name;
            d.ChangeType = GetChangeType(t);
            d.Type = dType;

            return d;
        }

        private static Type GetChangeType(Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
                return Instance.GetGenericArguments(conversionType)[0];

            return conversionType;
        }

        #region [   PROPERTY GET SET   ]

        internal string GetTypeAssemblyName(Type t)
        {
            string val;
            if (_tyname.TryGetValue(t, out val))
                return val;
			var s = t.AssemblyQualifiedName;
			_tyname.Add(t, s);
			return s;
		}

        internal Type GetTypeFromCache(string typename)
        {
            Type val;
            if (_typecache.TryGetValue(typename, out val))
                return val;
			
			var t = Type.GetType(typename);
			_typecache.Add(typename, t);
			return t;
		}

        internal object FastCreateInstance(Type objtype)
        {
            try
            {
                CreateObject c;
                if (_constrcache.TryGetValue(objtype, out c))
                {
                    return c();
                }

				if (objtype.IsClass)
				{
					var dynMethod = new DynamicMethod("_", objtype, null);
					var ilGen = dynMethod.GetILGenerator();

					var constructor = objtype.GetConstructor(Type.EmptyTypes);
					if (constructor == null)
					{
						Debug.LogError("Add Default Empty Constructor in: " + objtype);
						throw new Exception();
					}

					ilGen.Emit(OpCodes.Newobj, constructor);
					ilGen.Emit(OpCodes.Ret);
					c = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
					_constrcache.Add(objtype, c);
				}
				else // structs
				{
					var dynMethod = new DynamicMethod("_", typeof(object), null);
					var ilGen = dynMethod.GetILGenerator();
					var lv = ilGen.DeclareLocal(objtype);
					ilGen.Emit(OpCodes.Ldloca_S, lv);
					ilGen.Emit(OpCodes.Initobj, objtype);
					ilGen.Emit(OpCodes.Ldloc_0);
					ilGen.Emit(OpCodes.Box, objtype);
					ilGen.Emit(OpCodes.Ret);
					c = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
					_constrcache.Add(objtype, c);
				}
				return c();
			}
            catch (Exception exc)
            {
                throw new Exception(string.Format("Failed to fast create instance for type '{0}' from assembly '{1}'\n'{2}'",
					objtype.FullName, objtype.AssemblyQualifiedName, exc));
            }
        }

		private static GenericSetter CreateSetField(Type type, FieldInfo fieldInfo)
        {
            var arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);

            var dynamicSet = new DynamicMethod("_", typeof(object), arguments, type);

            var il = dynamicSet.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldarg_1);
				il.Emit(fieldInfo.FieldType.IsClass ? OpCodes.Castclass : OpCodes.Unbox_Any, fieldInfo.FieldType);
				il.Emit(OpCodes.Stfld, fieldInfo);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
                il.Emit(OpCodes.Ret);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                il.Emit(OpCodes.Stfld, fieldInfo);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ret);
            }
            return (GenericSetter)dynamicSet.CreateDelegate(typeof(GenericSetter));
        }

		private static GenericSetter CreateSetMethod(Type type, PropertyInfo propertyInfo)
        {
            var setMethod = propertyInfo.GetSetMethod();
            if (setMethod == null)
                return null;

            var arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);

            var setter = new DynamicMethod("_", typeof(object), arguments);
            var il = setter.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldarg_1);
				il.Emit(propertyInfo.PropertyType.IsClass ? OpCodes.Castclass : OpCodes.Unbox_Any, propertyInfo.PropertyType);
				il.EmitCall(OpCodes.Call, setMethod, null);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
            }
            else
            {
                if (!setMethod.IsStatic)
                {
                    il.Emit(OpCodes.Ldarg_0);
					if (propertyInfo.DeclaringType == null)
					{
						Debug.LogError("propertyInfoDeclaring type is NULL: " + propertyInfo);
						return null;
					}
					
                    il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
                    il.Emit(OpCodes.Ldarg_1);
					il.Emit(propertyInfo.PropertyType.IsClass ? OpCodes.Castclass : OpCodes.Unbox_Any, propertyInfo.PropertyType);
					il.EmitCall(OpCodes.Callvirt, setMethod, null);
                    il.Emit(OpCodes.Ldarg_0);
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
					il.Emit(propertyInfo.PropertyType.IsClass ? OpCodes.Castclass : OpCodes.Unbox_Any, propertyInfo.PropertyType);
					il.Emit(OpCodes.Call, setMethod);
                }
            }

            il.Emit(OpCodes.Ret);

            return (GenericSetter)setter.CreateDelegate(typeof(GenericSetter));
        }

		private static GenericGetter CreateGetField(Type type, FieldInfo fieldInfo)
        {
            var dynamicGet = new DynamicMethod("_", typeof(object), new[] { typeof(object) }, type);

            var il = dynamicGet.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldfld, fieldInfo);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Box, fieldInfo.FieldType);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldInfo);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Box, fieldInfo.FieldType);
            }

            il.Emit(OpCodes.Ret);

            return (GenericGetter)dynamicGet.CreateDelegate(typeof(GenericGetter));
        }

		private static GenericGetter CreateGetMethod(Type type, PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
                return null;

            var getter = new DynamicMethod("_", typeof(object), new[] { typeof(object) }, type);

            var il = getter.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.EmitCall(OpCodes.Call, getMethod, null);
                if (propertyInfo.PropertyType.IsValueType)
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }
            else
            {
                if (!getMethod.IsStatic)
                {
                    il.Emit(OpCodes.Ldarg_0);
					if (propertyInfo.DeclaringType == null)
					{
						Debug.LogError("propertyInfoDeclaring type is NULL: " + propertyInfo);
						return null;
					}
					
                    il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
                    il.EmitCall(OpCodes.Callvirt, getMethod, null);
                }
                else
                    il.Emit(OpCodes.Call, getMethod);

                if (propertyInfo.PropertyType.IsValueType)
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }

            il.Emit(OpCodes.Ret);

            return (GenericGetter)getter.CreateDelegate(typeof(GenericGetter));
        }

        internal Getters[] GetGetters(Type type, bool showReadOnlyProperties, List<Type> ignoreAttributes)
        {
            Getters[] val;
            if (_getterscache.TryGetValue(type, out val))
                return val;

            //bool isAnonymous = IsAnonymousType(type);
			
            //if (ShowReadOnlyProperties)
            //    bf |= BindingFlags.NonPublic;
            var props = type.GetProperties(Bf);
            var getters = new List<Getters>();
            foreach (var p in props)
            {
                if (p.GetIndexParameters().Length > 0)
                {// Property is an indexer
                    continue;
                }
                if (!p.CanWrite && (showReadOnlyProperties == false))//|| isAnonymous == false))
                    continue;
                if (ignoreAttributes != null)
                {
                    var found = false;
                    foreach (var ignoreAttr in ignoreAttributes)
                    {
						if (!p.IsDefined(ignoreAttr, false)) continue;
						
						found = true;
						break;
					}
                    if (found)
                        continue;
                }
                var g = CreateGetMethod(type, p);
                if (g != null)
                    getters.Add(new Getters { Getter = g, Name = p.Name, LcName = p.Name.ToLower() });
            }

            var fi = type.GetFields(Bf);
            foreach (var f in fi)
            {
                if (ignoreAttributes != null)
                {
                    var found = false;
                    foreach (var ignoreAttr in ignoreAttributes)
                    {
						if (!f.IsDefined(ignoreAttr, false)) continue;
						
						found = true;
						break;
					}
                    if (found)
                        continue;
                }

				if (f.IsLiteral) continue;
				
				var g = CreateGetField(type, f);
				if (g != null)
					getters.Add(new Getters { Getter = g, Name = f.Name, LcName = f.Name.ToLower() });
			}
            val = getters.ToArray();
            _getterscache.Add(type, val);
            return val;
        }

        //private static bool IsAnonymousType(Type type)
        //{
        //    // may break in the future if compiler defined names change...
        //    const string CS_ANONYMOUS_PREFIX = "<>f__AnonymousType";
        //    const string VB_ANONYMOUS_PREFIX = "VB$AnonymousType";

        //    if (type == null)
        //        throw new ArgumentNullException("type");

        //    if (type.Name.StartsWith(CS_ANONYMOUS_PREFIX, StringComparison.Ordinal) || type.Name.StartsWith(VB_ANONYMOUS_PREFIX, StringComparison.Ordinal))
        //    {
        //        return type.IsDefined(typeof(CompilerGeneratedAttribute), false);
        //    }

        //    return false;
        //}
        #endregion

		private void ResetPropertyCache()
        {
            _propertycache = new SafeDictionary<string, Dictionary<string, MyPropInfo>>();
        }

        internal void ClearReflectionCache()
        {
            _tyname = new SafeDictionary<Type, string>();
            _typecache = new SafeDictionary<string, Type>();
            _constrcache = new SafeDictionary<Type, CreateObject>();
            _getterscache = new SafeDictionary<Type, Getters[]>();
            _propertycache = new SafeDictionary<string, Dictionary<string, MyPropInfo>>();
            _genericTypes = new SafeDictionary<Type, Type[]>();
            _genericTypeDef = new SafeDictionary<Type, Type>();
        }
    }
}
