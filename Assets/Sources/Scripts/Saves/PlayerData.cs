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
    [SerializeField, OdinSerialize] private string _adminPassword;

    private PlayerStatsData _stats;

    public int ID => _id;
    public string Name => _name;
    public PlayerStatsData Stats => _stats;
    public string CurrentSceneName => _currentSceneName;
    public List<string> LevelsPurchased => _levelsPurchased;
    public string AdminPassword => _adminPassword; 

    public PlayerData(int id, string name, string startLocation, string adminPassword)
    {
        _id = id;
        _name = name;
        _currentSceneName = startLocation;
        _stats = new PlayerStatsData();
        _levelsPurchased = new List<string>();
        _adminPassword = adminPassword;
    }

    public void SetCurrentScene(string sceneName) => _currentSceneName = sceneName;

    public void SetStats(PlayerStatsData stats)
    {
        _stats = stats;
    }

    public PlayerStatsData GetStats() => _stats;

    public string SetAdminPassword(string value) => _adminPassword = value;

    public void AddPurchase(string level)
    {
        if(!LevelsPurchased.Contains(level))
            _levelsPurchased.Add(level);
    }
}
