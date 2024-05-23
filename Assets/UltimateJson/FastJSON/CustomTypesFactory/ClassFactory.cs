using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using UnityEngine;

namespace fastJSON.CustomTypesFactory
{
	public class ClassFactory
	{
		public static DateTime GetDateTime(string obj)
		{
			var ms = 0;

			var year = CreateInteger(obj, 0, 4);
			var month = CreateInteger(obj, 5, 2);
			var day = CreateInteger(obj, 8, 2);
			var hour = CreateInteger(obj, 11, 2);
			var min = CreateInteger(obj, 14, 2);
			var sec = CreateInteger(obj, 17, 2);
			if (obj.Length > 21 && obj[19] == '.')
				ms = CreateInteger(obj, 20, 3);

			return new DateTime(year, month, day, hour, min, sec, ms, DateTimeKind.Utc).ToLocalTime();
		}

		public static T GetEnumValue<T>(string obj)
		{
			var t = typeof(T);
			return (T)Enum.Parse(t, obj);
		}

		public static Guid GetGuidValue(string obj)
		{
			return obj.Length > 30 ? new Guid(obj) : new Guid(Convert.FromBase64String(obj));
		}

		public static StringDictionary GetStringDictionaryValue(object obj)
		{
			var dic = (Dictionary<string, object>)obj;
			var stringDic = new StringDictionary();
			foreach (var keyValue in dic)
			{
				stringDic.Add(keyValue.Key, (string)keyValue.Value);
			}
			return stringDic;
		}
		
		public static Dictionary<string, string> GetDictionaryStringStringValue(object obj)
		{
			var dic = (Dictionary<string, object>)obj;
			var stringDic = new Dictionary<string, string>();
			foreach (var keyValue in dic)
			{
				stringDic.Add(keyValue.Key, (string)keyValue.Value);
			}
			return stringDic;
		}

		public static NameValueCollection GetNameValueCollectionValue(object obj)
		{
			var dic = (Dictionary<string, object>)obj;
			var nameValue = new NameValueCollection();
			foreach (var keyValue in dic)
			{
				nameValue.Add(keyValue.Key, (string)keyValue.Value);
			}
			return nameValue;
		}
		
		public static Hashtable GetHashtableValue(object obj)
		{
			var result = new Hashtable();
			var list = (List<object>) obj;
			foreach (var el in list)
			{
				var dicEl = (Dictionary<string, object>)el;
				var key = dicEl["k"];
				var value = dicEl["v"];
				result.Add(key, value);
			}
			return result;
		}

		public static Array GetArrayValue(object obj)
		{
			var list = (List<object>) obj;
			var result = Array.CreateInstance(list.GetType().GetGenericArguments()[0], list.Count);
			for (var i = 0; i < list.Count; i++)
			{
				var listValue = list[i];
				result.SetValue(listValue, i);
			}
			return result;
		}

		public static byte[] GetByteArrayValue(string obj)
		{
			return Convert.FromBase64String(obj);
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
	}
}