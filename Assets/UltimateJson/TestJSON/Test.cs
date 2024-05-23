using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using MARECEK;
using UltimateJson;
using UnityEngine;

public class Test : MonoBehaviour
{
	public List<PlayerStat> LoadedStats;

	private void Start()
	{
		//TestSer();
		//TestStringArray();
		//TestDictionaryStringString();
		TestUtf8();
//		
//		var s = PlayerPrefs.GetString("save");
//		if (string.IsNullOrEmpty(s)) return;
//		
//		print("Loaded " + s);
//		var jo = JsonObject.Deserialise(s);
//		print("loaded " + jo);
//		
//		var statLst = jo["stats"];
//		print("loaded " + statLst);
//		
//		var stat1Test = statLst.TryGetValue<List<PlayerStat>>();
//
//		print("loaded " + stat1Test.Count + " stats");
//		LoadedStats = stat1Test;
	}

	private void TestSer()
	{
		var str = "{\"strings\":{\"en-gb\":{\"welcome\":\"Welcome\",\"chosenLanguage\":\"You have chosen English.\"}," +
				"\"en-ru\":{\"welcome\":\"Добро Пожаловать\",\"chosenLanguage\":\"Вы выбрали русский язык.\"}}}";
		var result = JsonObject.Deserialise<TestSpecNames>(str);
		print(result);
		print(result.Strings);
		print(result.Strings.Enru);
		print(result.Strings.Enru["welcome"]);
	}

	private void TestStringArray()
	{
		var str = "{\"array\":[\"ABC\",\"DEF\"]}";
		var strDes = JsonObject.Deserialise(str);
		var strArray = strDes["array"];
		print(strArray);
		
		var result = strArray.TryGetValue<string[]>();
		print(result.Length);
		print(result[0]);
		print(result[1]);
	}

	private static void TestDictionaryStringString()
	{
		var theDict = new Dictionary<string, string> ();
		theDict ["hi"] = "nobody";
		theDict ["yo"] = "happy";
		var j = JsonObject.Serialise(theDict);
		var theObject = JsonObject.Deserialise (j);
		var aDict = theObject.TryGetValue<StringDictionary>();
		Debug.Log ("Deserialize got me " + aDict.Count + "Keys");
		foreach(string myKey in aDict.Keys) 
		{
			Debug.Log (myKey + " : " + aDict[myKey]);
		}

		var bDict = theObject.TryGetValue<Dictionary<string,string>>();
		Debug.Log ("Deserialize got me " + bDict.Count + "Keys");
		foreach(var kvc in bDict) 
		{
			Debug.Log (kvc.Key + " : " + kvc.Value);
		}
	}

	private void TestUtf8()
	{
		var decodedString = "{\"name\":\"李东泽\"}";
		var jObject = JsonObject.Deserialise(decodedString);
		Debug.Log(jObject["name"]);
	}

	private void OnApplicationQuit()
	{
//		var dict = new Dictionary<string, object>();
//		dict["stats"] = LoadedStats;
//		var s = JsonObject.Serialise(dict);
//		PlayerPrefs.SetString("save", s);
//		PlayerPrefs.Save();
//		print("Saved " + s);
	}
}