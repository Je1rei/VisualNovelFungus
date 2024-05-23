using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PersonListScriptable : ScriptableObject
{
	public List<Person> personList;

	public PersonListScriptable()
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