using System;
using System.Collections.Generic;

namespace MARECEK
{
	[Serializable]
	public class TestSpecNames
	{
		public Strings Strings;
	}

	[Serializable]
	public class Strings
	{
		public Dictionary<string, string> Engb;
		public Dictionary<string, string> Enru;
	}
}