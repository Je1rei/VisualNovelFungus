using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private string _currentSceneName;

    private PlayerStatsData _stats;

    public int ID => _id;
    public string Name => _name;
    public PlayerStatsData Stats => _stats;
    public string CurrentSceneName => _currentSceneName;    

    public PlayerData(int id, string name, string startLocation)
    {
        _id = id;
        _name = name;
        _currentSceneName = startLocation;
        _stats = new PlayerStatsData();
    }

    public void SetCurrentScene(string sceneName) => _currentSceneName = sceneName;

    public void SetStats(PlayerStatsData stats)
    {
        _stats = stats;
    }

    public PlayerStatsData GetStats() => _stats;
}
