using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;
//using fastJSON;
using System.Linq;
using UltimateJson;

[Serializable]
public class Person
{
	public int _id;
	public string _name;
	public string _surname;

	public Person(string surname, string name, int id)
	{
		_surname = surname;
		_name = name;
		_id = id;
	}

	public Person()
	{
	}

	public override string ToString()
	{
		return "Person: " + _id + ", NS:" + _surname + " " + _name;
	}
}

[Serializable]
public class PersonList
{
	public List<Person> personList;

	public PersonList()
	{	
	}

	public void AddPerson(Person p)
	{
		if (personList == null)
		{
			personList = new List<Person>();
		}

		personList.Add(p);
	}
}

#if UNITY_EDITOR
public class CreatePersonListScriptable
{
	private const int Maxvalue = 20000;

	[UnityEditor.MenuItem("Assets/Create/PersonList")]
	public static PersonListScriptable Create()
	{
		var asset = ScriptableObject.CreateInstance<PersonListScriptable>();

		for (var i = 0; i < Maxvalue; i++)
		{
			var person = new Person("Zalunin", "Sergey", i);
			asset.AddPerson(person);
		}

		UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/Resources/ScriptableObjects/PersonListScriptable.asset");
		UnityEditor.AssetDatabase.SaveAssets();
		return asset;
	}
}
#endif

public class testJSON : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
		//StartTest();
		TestSerilize();
		ScriptableObjectTime();
	}

	void StartTest()
	{
		string line = loadAndroid("game");
		System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
		s.Start();
		var jObject = JsonObject.Deserialise(line);
		s.Stop();
		Debug.Log("time:" + s.ElapsedMilliseconds);
		var personages = jObject["personages"]["1"];
		foreach (var field in personages)
		{
			Debug.Log(field);
		}
		personages["preview"] = new JsonObject("ff");
		foreach (var field in personages)
		{
			Debug.Log(field);
		}
		Debug.Log(jObject["personages"]["1"]["preview"].ToString());
	}

	private const int Maxvalue = 20000;
	private void TestSerilize()
	{
		var personList = new PersonList();
		for (var i = 0; i < Maxvalue; i++)
		{
			var person = new Person("Zalunin", "Sergey", i);
			personList.AddPerson(person);
		}

		var s = new System.Diagnostics.Stopwatch();
		var personStr = JsonObject.Serialise(personList);

		s.Start();
		var personListDes = JsonObject.Deserialise<PersonList>(personStr);
		var idList = personListDes.personList.Select(per => per._id).ToList();
		s.Stop();

		foreach (var id in idList)
		{
			//Debug.Log(id);
			Console.Write(id);
		}
		Debug.Log("time:" + s.ElapsedMilliseconds);
		Debug.Log("person:" + personListDes.personList.Count + " [0]: " + personListDes.personList[0]);
	}

	private void ScriptableObjectTime()
	{
		var s = new System.Diagnostics.Stopwatch();
		s.Start();
		var personList = Resources.Load("ScriptableObjects/PersonListScriptable") as PersonListScriptable;
		if (personList != null)
		{
			var idList = personList.personList.Select(per => per._id).ToList();
			s.Stop();

			foreach (var id in idList)
			{
				Console.Write(id);
			}
		}
		Debug.Log("time:" + s.ElapsedMilliseconds);
		Debug.Log("person:" + personList.personList.Count + " [0]: " + personList.personList[1500]);
	}

	private string Load(string fileName)
	{
		string line = "";

		//fileName = Application.dataPath + "/" + fileName;
		// Handle any problems that might arise when reading the text
		try
		{
			string lineTemp;
			// Create a new StreamReader, tell it which file to read and what encoding the file
			// was saved as
			StreamReader theReader = new StreamReader(fileName, Encoding.UTF8);
			// Immediately clean up the reader after this block of code is done.
			// You generally use the "using" statement for potentially memory-intensive objects
			// instead of relying on garbage collection.
			// (Do not confuse this with the using directive for namespace at the 
			// beginning of a class!)
			using (theReader)
			{
				// While there's lines left in the text file, do this:
				do
				{
					lineTemp = theReader.ReadLine();
					if (lineTemp != null)
					{
						line = lineTemp;
					}
				}
				while (lineTemp != null);
				// Done reading, close the reader and return true to broadcast success    
				theReader.Close();
			}
		}
		// If anything broke in the try block, we throw an exception with information
		// on what didn't work
		catch (System.Exception e)
		{
			Debug.Log(e.Message + "\n");
			return "";
		}

		return line;
	}

	private string loadAndroid(string path)
	{
		TextAsset text = Resources.Load(path) as TextAsset;

		string linesFromfile = text.text;

		return linesFromfile;
	}
}