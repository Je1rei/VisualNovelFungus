using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PriceData
{
    [SerializeField] private List<PriceActivity> _priceActivities = new List<PriceActivity>();

    public List<PriceActivity> PriceActivities => _priceActivities;

    public void Add(string key, int price)
    {
        PriceActivity existingActivity = _priceActivities.Find(x => x.Key == key);

        if (existingActivity != null)
            existingActivity.Value = price;
        else
            _priceActivities.Add(new PriceActivity(key, price));
    }
}
