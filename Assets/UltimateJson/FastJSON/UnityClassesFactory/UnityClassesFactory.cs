using System.Collections.Generic;
using UnityEngine;

namespace fastJSON
{
	public static class UnityClassesFactoryMain
	{
		public static Color CreateColor(Dictionary<string, object> v)
		{
			float x = 0, y = 0, z = 0, w = 0;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;

				switch (key)
				{
					case "r":
						x = GetFloatFromObject(value);
						continue;
					case "g":
						y = GetFloatFromObject(value);
						continue;
					case "b":
						z = GetFloatFromObject(value);
						continue;
					case "a":
						w = GetFloatFromObject(value);
						continue;
					default:
						continue;
				}
			}
            
			return new Color(x, y, z, w);
		}
		
		public static Color32 CreateColor32(Dictionary<string, object> v)
		{
			byte x = 0, y = 0, z = 0, w = 0;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;
				
				switch (key)
				{
					case "r":
						x = GetByteFromObject(value);
						continue;
					case "g":
						y = GetByteFromObject(value);
						continue;
					case "b":
						z = GetByteFromObject(value);
						continue;
					case "a":
						w = GetByteFromObject(value);
						continue;
					default:
						continue;
				}
			}
            
			return new Color32(x, y, z, w);
		}
		
		public static Rect CreateRect(Dictionary<string, object> v)
		{
			float x = 0, y = 0, z = 0, w = 0;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;

				switch (key)
				{
					case "x":
						x = GetFloatFromObject(value);
						continue;
					case "y":
						y = GetFloatFromObject(value);
						continue;
					case "width":
						z = GetFloatFromObject(value);
						continue;
					case "height":
						w = GetFloatFromObject(value);
						continue;
					default:
						continue;
				}
			}
            
			return new Rect(x, y, z, w);
		}
		
		public static RectInt CreateRectInt(Dictionary<string, object> v)
		{
			int x = 0, y = 0, z = 0, w = 0;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;

				switch (key)
				{
					case "xMin":
						x = GetIntFromObject(value);
						continue;
					case "yMin":
						y = GetIntFromObject(value);
						continue;
					case "width":
						z = GetIntFromObject(value);
						continue;
					case "height":
						w = GetIntFromObject(value);
						continue;
					default:
						continue;
				}
			}
            
			return new RectInt(x, y, z, w);
		}
		
		public static Bounds CreateBounds(Dictionary<string, object> v)
		{
			Vector3 center = Vector3.zero, size = Vector3.zero;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;

				switch (key)
				{
					case "center":
						center = VectorFactory.CreateVector3((Dictionary<string, object>)value);
						continue;
					case "size":
						size = VectorFactory.CreateVector3((Dictionary<string, object>)value);
						continue;
					default:
						continue;
				}
			}

			return new Bounds(center, size);
		}
		
		public static BoundsInt CreateBoundsInt(Dictionary<string, object> v)
		{
			Vector3Int center = Vector3Int.zero, size = Vector3Int.zero;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;

				switch (key)
				{
					case "position":
						center = VectorFactory.CreateVector3Int((Dictionary<string, object>)value);
						continue;
					case "size":
						size = VectorFactory.CreateVector3Int((Dictionary<string, object>)value);
						continue;
					default:
						continue;
				}
			}

			return new BoundsInt(center, size);
		}
		
		public static Ray CreateRay(Dictionary<string, object> v)
		{
			Vector3 origin = Vector3.zero, direction = Vector3.zero;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;

				switch (key)
				{
					case "origin":
						origin = VectorFactory.CreateVector3((Dictionary<string, object>)value);
						continue;
					case "direction":
						direction = VectorFactory.CreateVector3((Dictionary<string, object>)value);
						continue;
					default:
						continue;
				}
			}

			return new Ray(origin, direction);
		}
		
		public static Ray2D CreateRay2D(Dictionary<string, object> v)
		{
			Vector2 origin = Vector2.zero, direction = Vector2.zero;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;

				switch (key)
				{
					case "origin":
						origin = VectorFactory.CreateVector2((Dictionary<string, object>)value);
						continue;
					case "direction":
						direction = VectorFactory.CreateVector2((Dictionary<string, object>)value);
						continue;
					default:
						continue;
				}
			}

			return new Ray2D(origin, direction);
		}
		
		public static Quaternion CreateQuaternion(Dictionary<string, object> v)
		{
			float x = 0, y = 0, z = 0, w = 0;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;

				switch (key)
				{
					case "x":
						x = GetFloatFromObject(value);
						continue;
					case "y":
						y = GetFloatFromObject(value);
						continue;
					case "z":
						z = GetFloatFromObject(value);
						continue;
					case "w":
						w = GetFloatFromObject(value);
						continue;
					default:
						continue;
				}
			}
            
			return new Quaternion(x, y, z, w);
		}

		public static float GetFloatFromObject(object obj)
		{
			var objType = obj.GetType();
			if (objType == typeof(long))
			{
				return (long)obj;
			}

			if (objType == typeof(double))
			{
				return (float)(double)obj;
			}

			Debug.LogError("not supported this type: " + objType);
			return -1;
		}
		
		public static int GetIntFromObject(object obj)
		{
			var objType = obj.GetType();
			if (objType == typeof(long))
			{
				return (int)(long)obj;
			}

			Debug.LogError("not supported this type: " + objType);
			return -1;
		}
		
		public static byte GetByteFromObject(object obj)
		{
			var objType = obj.GetType();
			if (objType == typeof(long))
			{
				return (byte)(long)obj;
			}

			Debug.LogError("not supported this type: " + objType);
			return 0;
		}
	}
}