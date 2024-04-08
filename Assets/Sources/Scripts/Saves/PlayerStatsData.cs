using System;
using UnityEngine;

[Serializable]
public class PlayerStatsData
{
    [SerializeField] private int _countCollected;
    [SerializeField] private int _closedGames;

    public void SetClosedGames(int value) => _closedGames += value;
    public void SetCountCollected(int value) => _countCollected += value;
}
