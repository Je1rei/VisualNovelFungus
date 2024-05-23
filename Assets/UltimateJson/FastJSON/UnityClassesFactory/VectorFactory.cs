using System;
using System.Collections.Generic;
using UnityEngine;

namespace fastJSON
{
	public class VectorFactory
	{
		public static Vector2 CreateVector2(Dictionary<string, object> v)
		{
			float x = 0, y = 0;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;

				switch (key)
				{
					case "x":
						x = UnityClassesFactoryMain.GetFloatFromObject(value);
						continue;
					case "y":
						y = UnityClassesFactoryMain.GetFloatFromObject(value);
						continue;
					default:
						continue;
				}
			}
            
			return new Vector2(x, y);
		}
		
		public static Vector2Int CreateVector2Int(Dictionary<string, object> v)
		{
			int x = 0, y = 0;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;
				
				switch (key)
				{
					case "x":
						x = UnityClassesFactoryMain.GetIntFromObject(value);
						continue;
					case "y":
						y = UnityClassesFactoryMain.GetIntFromObject(value);
						continue;
					default:
						continue;
				}
			}
            
			return new Vector2Int(x, y);
		}
		
		public static Vector3 CreateVector3(Dictionary<string, object> v)
		{
			float x = 0, y = 0, z = 0;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;

				switch (key)
				{
					case "x":
						x = UnityClassesFactoryMain.GetFloatFromObject(value);
						continue;
					case "y":
						y = UnityClassesFactoryMain.GetFloatFromObject(value);
						continue;
					case "z":
						z = UnityClassesFactoryMain.GetFloatFromObject(value);
						continue;
					default:
						continue;
				}
			}
            
			return new Vector3(x, y, z);
		}
		
		public static Vector3Int CreateVector3Int(Dictionary<string, object> v)
		{
			int x = 0, y = 0, z = 0;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;

				switch (key)
				{
					case "x":
						x = UnityClassesFactoryMain.GetIntFromObject(value);
						continue;
					case "y":
						y = UnityClassesFactoryMain.GetIntFromObject(value);
						continue;
					case "z":
						z = UnityClassesFactoryMain.GetIntFromObject(value);
						continue;
					default:
						continue;
				}
			}
            
			return new Vector3Int(x, y, z);
		}
		
		public static Vector4 CreateVector4(Dictionary<string, object> v)
		{
			float x = 0, y = 0, z = 0, w = 0;
			foreach (var keyvalue in v)
			{
				var key = keyvalue.Key;
				var value = keyvalue.Value;

				switch (key)
				{
					case "x":
						x = UnityClassesFactoryMain.GetFloatFromObject(value);
						continue;
					case "y":
						y = UnityClassesFactoryMain.GetFloatFromObject(value);
						continue;
					case "z":
						z = UnityClassesFactoryMain.GetFloatFromObject(value);
						continue;
					case "w":
						w = UnityClassesFactoryMain.GetFloatFromObject(value);
						continue;
					default:
						continue;
				}
			}
            
			return new Vector4(x, y, z, w);
		}
	}
}