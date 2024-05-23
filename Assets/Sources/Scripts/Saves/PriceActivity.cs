using System;
using UnityEngine;

[Serializable]
public class PriceActivity
{
    [SerializeField] private string _key;
    [SerializeField] private int _price;

    public string Key { get => _key; set => _key = value; }
    public int Value { get => _price; set => _price = value; }

    public PriceActivity(string key, int price)
    {
        _key = key;
        _price = price;
    }
}
