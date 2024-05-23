using System;
using System.Collections;
using System.Collections.Generic;
using UltimateJson;
using UnityEngine;

[Serializable]
public class PersonVector3
{
	public Vector2 V2;
	public Vector2Int Vector2Int;
	public Vector3Int Vector3Int;
	public Vector4 Vector4;
	
	public Color Color;
	public Color32 Color32;
	
	public Rect Rect;
	public RectInt RectInt;
	
	public Bounds Bounds;
	public BoundsInt BoundsInt;
	
	public Vector3 Pos;
	public Quaternion Rot;
	public Ray Ray;
	public Ray2D Ray2D;

	public PersonVector3()
	{
	}

	public override string ToString()
	{
		var str = "V2: " + V2 + "\n";
		str += "Vector2Int: " + Vector2Int + "\n";
		str += "Vector3Int: " + Vector3Int + "\n";
		str += "Vector4: " + Vector4 + "\n";
		
		str += "Color: " + Color + "\n";
		str += "Color32: " + Color32 + "\n";
		
		str += "Rect: " + Rect + "\n";
		str += "RectInt: " + RectInt + "\n";
		
		str += "Bounds: " + Bounds + "\n";
		str += "BoundsInt: " + BoundsInt + "\n";
		
		str += "Ray: " + Ray + "\n";
		str += "Ray2D: " + Ray2D + "\n";
		
		str += "Pos: " + Pos + "\n";
		str += "Rot: " + Rot + "\n";
		
		return str;
	}
}

public class testVector3 : MonoBehaviour
{
	// Use this for initialization
	private void Start () 
	{
		var person = new PersonVector3
		{
			V2 = new Vector2(0.1f, 6.8f),
			Vector2Int = new Vector2Int(5,8),
			Vector3Int = new Vector3Int(5, 8, 6),
			Vector4 = new Vector4(5.6f, 3, 2.8f, 0.99999f),
			
			Color = new Color(0.15f, 0.2f, 0.6f),
			Color32 = new Color32(255, 0, 0, 255),
			
			Rect = new Rect(10.5f, 11.5f, 1920.78f, 1080.666f),
			RectInt = new RectInt(45, 67, 800, 600),
			
			Bounds = new Bounds(new Vector3(5.2f, 0.2f, 0.1f), new Vector3(580.77f, 620.89f, 0)),
			BoundsInt = new BoundsInt(0, 0, 0, 1920, 1080, 0),
			
			Ray = new Ray(new Vector3(0.2f, 0.67f, 0.456f), Vector3.up),
			Ray2D = new Ray2D(new Vector2(0.245f, 78.895f), Vector2.left),
			
			Pos = new Vector3(0.2f, 5, 1.5f),
			Rot = Quaternion.Euler(90, 0, 0)
		};

		var str = JsonObject.Serialise(person);
		Debug.Log(str);

		var personDes = JsonObject.Deserialise<PersonVector3>(str);
		Debug.Log(personDes);
	}
}
