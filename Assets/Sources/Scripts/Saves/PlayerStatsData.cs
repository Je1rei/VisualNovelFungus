using System;
using UnityEngine;

[Serializable]
public class PlayerStatsData
{
    [SerializeField] private int _countCollected;
    [SerializeField] private int _closedGames;
    [SerializeField] private int _closedTests;
    [SerializeField] private int _coinCollected;

    public int CountCollected => _countCollected;
    public int ClosedGames => _closedGames; 
    public int ClosedTests => _closedTests;
    public int CoinCollected => _coinCollected; 

    public void SetClosedGames(int value) => _closedGames += value;

    public void SetCountCollected(int value) => _countCollected += value;

    public void SetCountClosedTests(int value) => _closedTests += value;    

    public void SetCoinCollected(int value) => _coinCollected += value;
}
