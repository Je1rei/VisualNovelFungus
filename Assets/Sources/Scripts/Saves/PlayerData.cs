using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [SerializeField] private int _id;
    [SerializeField] private string _name;

    private PlayerStatsData _stats;

    public int ID => _id;
    public string Name => _name;
    public PlayerStatsData Stats => _stats;

    public PlayerData(int id, string name)
    {
        _id = id;
        _name = name;
        _stats = new PlayerStatsData();
    }

    public void SetStats(PlayerStatsData stats)
    {
        _stats = stats;
    }
}
