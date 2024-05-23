using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UltimateJson;
using UnityEngine;

public class testJsonObject : MonoBehaviour
{
	// Use this for initialization
	private void Start ()
	{
		TestSimpleTypes();
		TestComplexTypes();
		TestUnityTypes();
	}

	private void TestSimpleTypes()
	{
		var a1 = 5;
		var a2 = 5789798798798779879;
		var a3 = "Test string";
		var a4 = true;
		var a5 = DateTime.Now;
		var a6 = Guid.NewGuid();
		Enum a7 = TestEnum.Test2;

		var aDic = new Dictionary<string, object>();
		aDic["int"] = a1;
		aDic["long"] = a2;
		aDic["string"] = a3;
		aDic["bool"] = a4;
		aDic["dateTime"] = a5;
		aDic["guid"] = a6;
		aDic["enum"] = a7;
		var j = JsonObject.Serialise(aDic);
		Debug.Log(j);

		var jo = JsonObject.Deserialise(j);
		Debug.Log(j);
		var j1 = jo["int"];
		var j2 = jo["long"];
		var j3 = jo["string"];
		var j4 = jo["bool"];
		var j5 = jo["dateTime"];
		var j6 = jo["guid"];
		var j7 = jo["enum"];

		a1 = j1.TryGetValue<int>();
		a2 = j2.TryGetValue<long>();
		a3 = j3.TryGetValue<string>();
		a4 = j4.TryGetValue<bool>();
		a5 = j5.TryGetValue<DateTime>();
		a6 = j6.TryGetValue<Guid>();
		a7 = j7.TryGetValue<TestEnum>();

		aDic = new Dictionary<string, object>();
		aDic["int"] = a1;
		aDic["long"] = a2;
		aDic["string"] = a3;
		aDic["bool"] = a4;
		aDic["dateTime"] = a5;
		aDic["guid"] = a6;
		aDic["enum"] = a7;
		j = JsonObject.Serialise(aDic);
		Debug.Log(j);
	}

	private void TestComplexTypes()
	{
		var a1 = new StringDictionary 
		{
			{"red", "rojo"}, 
			{"green", "verde"}, 
			{"blue", "azul"}
		};
		var a2 = new NameValueCollection
		{
			{"red", "red"},
			{"red", "rojo"},
			{"blue", "azul"}
		};
		// ReSharper disable once UseArrayCreationExpression.1
		var a3 = Array.CreateInstance( typeof(int), 2);
		for ( var i = a3.GetLowerBound(0); i <= a3.GetUpperBound(0); i++ )
			a3.SetValue( i+1, i );
		var a4 = new byte[] { 255, 255, 255, 0};
		var a5 = new Dictionary<string, object>
		{
			{"red", 255},
			{"green", 156},
			{"blue", 0}
		};
		var a6 = new List<object>()
		{
			255,
			156,
			0
		};
		var a7 = new Hashtable
		{
			{"red", "rojo"},
			{"green", "verde"},
			{"blue", "azul"}
		};
		
		var aDic = new Dictionary<string, object>();
		aDic["stringDictionary"] = a1;
		aDic["nameValueCollection"] = a2;
		aDic["array"] = a3;
		aDic["byteArray"] = a4;
		aDic["dictionary"] = a5;
		aDic["list"] = a6;
		aDic["hashtable"] = a7;
		var j = JsonObject.Serialise(aDic);
		Debug.Log(j);
		
		var jo = JsonObject.Deserialise(j);
		Debug.Log(j);
		var j1 = jo["stringDictionary"];
		var j2 = jo["nameValueCollection"];
		var j3 = jo["array"];
		var j4 = jo["byteArray"];
		var j5 = jo["dictionary"];
		var j6 = jo["list"];
		var j7 = jo["hashtable"];
		
		a1 = j1.TryGetValue<StringDictionary>();
		a2 = j2.TryGetValue<NameValueCollection>();
		a3 = j3.TryGetValue<Array>();
		a4 = j4.TryGetValue<byte[]>();
		a5 = j5.TryGetValue<Dictionary<string, object>>();
		a6 = j6.TryGetValue<List<object>>();
		a7 = j7.TryGetValue<Hashtable>();
		
		aDic = new Dictionary<string, object>();
		aDic["stringDictionary"] = a1;
		aDic["nameValueCollection"] = a2;
		aDic["array"] = a3;
		aDic["byteArray"] = a4;
		aDic["dictionary"] = a5;
		aDic["list"] = a6;
		aDic["hashtable"] = a7;
		j = JsonObject.Serialise(aDic);
		Debug.Log(j);
	}

	private void TestUnityTypes()
	{
		var a1 = new Vector2Int(4, 5);
		var a2 = new Vector2(4.6f, 5.2f);
		var a3 = new Vector3(4.6f, 5.2f, 67.7f);
		var a4 = new Vector3Int(4, 5, 2);
		var a5 = new Vector4(4.78f, 5.7896f, 2.453f, 7.981f);
		var a6 = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		var a7 = new Color32(0, 255, 0, 255);
		var a8 = new Rect(0.5f, 0.5f, 450f, 800f);
		var a9 = new RectInt(5, 5, 1920, 1080);
		var a10 = new Bounds(new Vector3(2.2f, 2.2f, 2.2f), new Vector3(10.2f, 10.2f, 10.2f));
		var a11 = new BoundsInt(new Vector3Int(0, 0, 5), new Vector3Int(200, 200, 500));
		var a12 = new Quaternion(2.2f, 55.78f, 6.7f, 2.3f);
		var a13 = new Ray(new Vector3(55.67f, 24.7654f, 5.25943243f), new Vector3(1.2f, 0, 0));
		var a14 = new Ray2D(new Vector2(0.5f, 0.5f), new Vector2(56.67f, 89.23f));
		
		var aDic = new Dictionary<string, object>();
		aDic["Vector2Int"] = a1;
		aDic["Vector2"] = a2;
		aDic["Vector3"] = a3;
		aDic["Vector3Int"] = a4;
		aDic["Vector4"] = a5;
		aDic["Color"] = a6;
		aDic["Color32"] = a7;
		aDic["Rect"] = a8;
		aDic["RectInt"] = a9;
		aDic["Bounds"] = a10;
		aDic["BoundsInt"] = a11;
		aDic["Quaternion"] = a12;
		aDic["Ray"] = a13;
		aDic["Ray2D"] = a14;
		var j = JsonObject.Serialise(aDic);
		Debug.Log(j);
		
		var jo = JsonObject.Deserialise(j);
		Debug.Log(j);
		var j1 = jo["Vector2Int"];
		var j2 = jo["Vector2"];
		var j3 = jo["Vector3"];
		var j4 = jo["Vector3Int"];
		var j5 = jo["Vector4"];
		var j6 = jo["Color"];
		var j7 = jo["Color32"];
		var j8 = jo["Rect"];
		var j9 = jo["RectInt"];
		var j10 = jo["Bounds"];
		var j11 = jo["BoundsInt"];
		var j12 = jo["Quaternion"];
		var j13 = jo["Ray"];
		var j14 = jo["Ray2D"];
		
		a1 = j1.TryGetValue<Vector2Int>();
		a2 = j2.TryGetValue<Vector2>();
		a3 = j3.TryGetValue<Vector3>();
		a4 = j4.TryGetValue<Vector3Int>();
		a5 = j5.TryGetValue<Vector4>();
		a6 = j6.TryGetValue<Color>();
		a7 = j7.TryGetValue<Color32>();
		a8 = j8.TryGetValue<Rect>();
		a9 = j9.TryGetValue<RectInt>();
		a10 = j10.TryGetValue<Bounds>();
		a11 = j11.TryGetValue<BoundsInt>();
		a12 = j12.TryGetValue<Quaternion>();
		a13 = j13.TryGetValue<Ray>();
		a14 = j14.TryGetValue<Ray2D>();
		
		aDic = new Dictionary<string, object>();
		aDic["Vector2Int"] = a1;
		aDic["Vector2"] = a2;
		aDic["Vector3"] = a3;
		aDic["Vector3Int"] = a4;
		aDic["Vector4"] = a5;
		aDic["Color"] = a6;
		aDic["Color32"] = a7;
		aDic["Rect"] = a8;
		aDic["RectInt"] = a9;
		aDic["Bounds"] = a10;
		aDic["BoundsInt"] = a11;
		aDic["Quaternion"] = a12;
		aDic["Ray"] = a13;
		aDic["Ray2D"] = a14;
		j = JsonObject.Serialise(aDic);
		Debug.Log(j);
	}
}

public enum TestEnum
{
	Test1,
	Test2
}