using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private string _currentSceneName;
    [SerializeField, OdinSerialize] private List<string> _levelsPurchased;

    private PlayerStatsData _stats;

    public int ID => _id;
    public string Name => _name;
    public PlayerStatsData Stats => _stats;
    public string CurrentSceneName => _currentSceneName;
    public List<string> LevelsPurchased => _levelsPurchased;

    public PlayerData(int id, string name, string startLocation)
    {
        _id = id;
        _name = name;
        _currentSceneName = startLocation;
        _stats = new PlayerStatsData();
        _levelsPurchased = new List<string>();
    }

    public void SetCurrentScene(string sceneName) => _currentSceneName = sceneName;

    public void SetStats(PlayerStatsData stats)
    {
        _stats = stats;
    }

    public PlayerStatsData GetStats() => _stats;

    public void AddPurchase(string level)
    {
        if(!LevelsPurchased.Contains(level))
            _levelsPurchased.Add(level);
    }
}
