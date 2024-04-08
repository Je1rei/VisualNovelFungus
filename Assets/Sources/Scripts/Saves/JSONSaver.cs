using Fungus;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEngine;

public class JSONSaver : MonoBehaviour
{
    [SerializeField] private const string _filePathName = "playerStats.json";
    [SerializeField] private const string _playerNameVar = "playerName";
    [SerializeField] private Flowchart _flowchart;

    private PlayerStatsData _playerStats;
    private string _filePath;
    private int _nextID = 1;

    private PlayerData _currentplayer;

    private void Awake()
    {
        _filePath = Path.Combine(Application.persistentDataPath, _filePathName);
    }

    private void Start()
    {
        Load();
    }

    // Переместить потом
    public void CreatePlayer()
    {
        FindMaxID();

        string playerName = _flowchart.GetStringVariable(_playerNameVar);

        Debug.Log(playerName);

        _currentplayer = new PlayerData(_nextID, playerName);

        _nextID++;
    }

    private void FindMaxID()
    {
        DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = directory.GetFiles("playerData_*.json");

        foreach (FileInfo file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file.Name);
            int id = int.Parse(fileName.Substring(fileName.IndexOf('_') + 1)); // Получаем ID из имени файла

            if (id >= _nextID)
            {
                _nextID = id + 1; // Устанавливаем _nextID равным максимальному ID плюс один
            }
        }
    }

    private bool PlayerExists(int id)
    {
        if (_currentplayer.ID == id)
        {
            return true;
        }

        return false;
    }

    public void SavePlayerNow()
    {
        SavePlayer(_currentplayer);
    }

    public void SavePlayer(PlayerData playerData)
    {
        string jsonData = JsonUtility.ToJson(playerData);

        string filePath = Application.persistentDataPath + "/playerData_" + playerData.ID + ".json";
        File.WriteAllText(filePath, jsonData);

        Debug.Log("Данные игрока сохранены в файл: " + filePath);
    }

    private void SavePlayerStats(PlayerData playerData)
    {
        Container container = FindObjectOfType<Container>();

        if (container != null)
        {
            PlayerStatsData playerStats = playerData.Stats;

            playerStats.SetClosedGames(container.ClosedGames);
            playerStats.SetCountCollected(container.CountCollected);
        }
        else
        {
            Debug.LogError("Не удалось найти экземпляр класса Container.");
            return;
        }
    }

    private void Load()
    {
        if (File.Exists(_filePath))
        {
            string jsonData = File.ReadAllText(_filePath);

            _playerStats = JsonUtility.FromJson<PlayerStatsData>(jsonData);

            Debug.Log("Данные загружены из файла: " + _filePath);
        }
        else
        {
            Debug.LogWarning("Файл данных игрока не найден: " + _filePath);

            _playerStats = new PlayerStatsData();
        }
    }
}