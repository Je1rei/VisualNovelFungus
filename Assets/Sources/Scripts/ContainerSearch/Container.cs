﻿using Fungus;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Container : MonoBehaviour
{
    private const string _flowChartTag = "FlowChart";

    private const string _varIsClosed = "isClosed";
    private const string _varTestIsClosed = "completedTest";
    private const string _varTotalCollected = "totalCollected";
    private const string _varCoinCollected = "coinCollected";
    private const string _varPrice = "priceStars";
    private const string _varPrize = "prize";

    private Flowchart _flowchart;
    private int _prize;

    public int CountCollected { get; private set; }
    public int ClosedGames { get; private set; }
    public int ClosedTests { get; private set; }
    public int CoinCollected {  get; private set; }
    public int PriceStars => _flowchart.GetIntegerVariable(_varPrice);


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetCollected()
    {
        CountCollected = _flowchart.GetIntegerVariable(_varTotalCollected);

        if (_flowchart.GetBooleanVariable(_varIsClosed))
        {
            ClosedGames++;
            CoinCollected++;
        }
        
        if (_flowchart.GetBooleanVariable(_varTestIsClosed))
        {
            ClosedTests++;
            CoinCollected++;
        }
    }

    public void DecreaseCoinCollected()
    {
        CoinCollected -= PriceStars;

        Debug.Log($"DECREASE {CoinCollected} -= {PriceStars} ()");
    }

    public void IncreaseCoinCollected()
    {
        _prize = _flowchart.GetIntegerVariable(_varPrize);
        CoinCollected += _prize;
    }

    public void ClearCoinCollected() => CoinCollected = 0;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _flowchart = GameObject.FindWithTag(_flowChartTag)?.GetComponent<Flowchart>();
    }
}
