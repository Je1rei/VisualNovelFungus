using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateJson
{
	[Serializable]
	public class JsonNodeObject
	{
		[SerializeField] public object FinishObj;
		[SerializeField] public JsonElementType ElementType;

		public JsonNodeObject(bool obj)
		{
			FinishObj = obj;
			ElementType = JsonElementType.Bool;
		}

		public JsonNodeObject(double obj)
		{
			FinishObj = obj;
			ElementType = JsonElementType.Double;
		}

		public JsonNodeObject(long obj)
		{
			FinishObj = obj;
			ElementType = JsonElementType.Long;
		}

		public JsonNodeObject(string obj)
		{
			FinishObj = obj;
			ElementType = JsonElementType.String;
		}

		public JsonNodeObject(Dictionary<string, object> obj)
		{
			FinishObj = obj;
			ElementType = JsonElementType.Dictionary;
		}
		
		public JsonNodeObject(List<object> obj)
		{
			FinishObj = obj;
			ElementType = JsonElementType.List;
		}

		public JsonNodeObject(JsonNodeObject node)
		{
			FinishObj = node.FinishObj;
			ElementType = node.ElementType;
		}
		
		public JsonNodeObject() {}
	}
}