using System;
using System.Collections.Generic;
using UnityEngine;

namespace MARECEK
{
	[Serializable]
	public class PlayerStat
	{
		public string statName = string.Empty;
		public string displayValue = string.Empty;
		public bool resetOnLevelRestart = true;
		public float startingValue = 1;
		public float minValue = 0;
		public float maxValue = 1;
		public bool depleteOverTime;
		public float depletionTime = 240;
		public bool recoverOverTime;
		public float recoveryTime = 240;
		[SerializeField] float _value;
		bool enableOnResetEvent;
		string onResetEventName;
		bool enableOnChangeEvent;
		string onChangeEventName;

		public int intValue
		{
			get { return (int) _value; }
		}

		public float percentValue
		{
			get { return _value / maxValue; }
		}

		public float value
		{
			get { return _value; }
			set
			{
				_value = value;
				if (_value < minValue)
				{
					_value = minValue;
				}

				if (_value > maxValue)
				{
					_value = maxValue;
				}
			}
		}

		public PlayerStat()
		{
		}
	}
}