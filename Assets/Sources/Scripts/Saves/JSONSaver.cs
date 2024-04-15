using Fungus;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JSONSaver : MonoBehaviour
{
    private const string _mainMenuScene = "Main Menu";
    private const string _startLocationScene = "StartLocation";

    private const string _flowChartTag = "FlowChart";
    private const string _playerDataFileName = "playerData";
    private const string _playerStatsFileName = "playerStats";

    private const string _totalCollected = "totalCollectedGame";
    private const string _closedGames = "closedGames";
    private const string _closedTests = "closedTests";
    private const string _playerNameVar = "playerName";
    private const string _isLoadedVar = "isLoaded";
    private const string _lastClosedEpisodeVar = "lastClosedEpisode";

    private Flowchart _flowchart;

    private string _filePath;
    private int _nextID = 1;

    private PlayerData _currentPlayer;
    private PlayerStatsData _currentPlayerStats;
    private Container _container;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Awake()
    {
        _filePath = Path.Combine(Application.persistentDataPath, _playerDataFileName);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void CreatePlayer()
    {
        FindMaxID();

        string playerName = _flowchart.GetStringVariable(_playerNameVar);

        _currentPlayer = new PlayerData(_nextID, playerName, _startLocationScene);
        _currentPlayerStats = _currentPlayer.GetStats();

        _nextID++;
    }

    public void DeleteSaves()
    {
        DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = directory.GetFiles($"*.json");

        foreach (FileInfo file in files)
        {
            file.Delete();
        }

        _currentPlayer = null;
        _currentPlayerStats = null;

        SaveInt(_totalCollected, 0);
        SaveInt(_closedGames, 0);
    }

    public void SavePlayerNow()
    {
        if (_container != null)
        {
            _container.SetCollected();
        }

        SavePlayer(_currentPlayer);
        SavePlayerStats(_currentPlayer);
    }

    public void SavePlayer(PlayerData playerData)
    {
        if (SceneManager.GetActiveScene().name != _mainMenuScene)
            playerData.SetCurrentScene(SceneManager.GetActiveScene().name);

        string jsonDataStats = JsonUtility.ToJson(playerData);

        string playerFilePath = Path.Combine(Application.persistentDataPath, $"{_playerDataFileName}_{playerData.ID}.json");
        File.WriteAllText(playerFilePath, jsonDataStats);

        Debug.Log("Данные игрока сохранены в файл: " + playerFilePath);
    }

    public void SaveBool(string value, bool boolValue)
    {
        _flowchart.SetBooleanVariable(value, boolValue);
    }

    public void SaveInt(string key, int value)
    {
        int valueSave = value;

        _flowchart.SetIntegerVariable(key, valueSave);
    }

    public void SaveString(string key, string value)
    {
        string valueSave = value;

        _flowchart.SetStringVariable(key, valueSave);
    }

    public void LoadScene()
    {
        if (_currentPlayer.CurrentSceneName != null)
            SceneManager.LoadScene(_currentPlayer.CurrentSceneName);
    }

    public void Load()
    {
        FindMaxID();

        string playerFilePath = Path.Combine(Application.persistentDataPath, $"{_playerDataFileName}_{_nextID - 1}.json");

        if (File.Exists(playerFilePath))
        {
            string jsonDataPlayer = File.ReadAllText(playerFilePath);

            _currentPlayer = JsonUtility.FromJson<PlayerData>(jsonDataPlayer);

            LoadPlayerStats(_currentPlayer);

            Debug.Log("Данные загружены из файла: " + playerFilePath);

            SaveBool(_isLoadedVar, true);

            SaveString(_playerNameVar, _currentPlayer.Name);
            SaveString(_lastClosedEpisodeVar, _currentPlayer.CurrentSceneName);

            SaveInt(_totalCollected, _currentPlayerStats.CountCollected);
            SaveInt(_closedGames, _currentPlayerStats.ClosedGames);
            SaveInt(_closedTests, _currentPlayerStats.ClosedTests);
        }
        else
        {
            Debug.LogWarning("Файл даты игрока не найден: " + playerFilePath);
            SaveBool(_isLoadedVar, false);
        }
    }

    private void FindMaxID()
    {
        DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = directory.GetFiles($"{_playerDataFileName}_*.json");

        foreach (FileInfo file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file.Name);
            int id = int.Parse(fileName.Substring(fileName.IndexOf('_') + 1));

            if (id >= _nextID)
            {
                _nextID = id + 1;
            }
        }
    }

    private void SavePlayerStats(PlayerData playerData)
    {
        _currentPlayerStats.SetClosedGames(_container.ClosedGames);
        _currentPlayerStats.SetCountCollected(_container.CountCollected);
        _currentPlayerStats.SetCountClosedTests(_container.ClosedTests);

        string jsonDataStats = JsonUtility.ToJson(_currentPlayerStats);
        string playerStatsFilePath = Path.Combine(Application.persistentDataPath, $"{_playerStatsFileName}_{playerData.ID}.json");

        File.WriteAllText(playerStatsFilePath, jsonDataStats);

        Debug.Log("Файл данных статистики игрока сохранен: " + playerStatsFilePath);
    }

    private void LoadPlayerStats(PlayerData playerData)
    {
        string playerStatsFilePath = Path.Combine(Application.persistentDataPath, $"{_playerStatsFileName}_{playerData.ID}.json");

        if (File.Exists(playerStatsFilePath))
        {
            string jsonDataStats = File.ReadAllText(playerStatsFilePath);
            _currentPlayerStats = JsonUtility.FromJson<PlayerStatsData>(jsonDataStats);
        }
        else
        {
            Debug.LogWarning("Файл данных статистики игрока не найден" + playerStatsFilePath);

            _currentPlayerStats = new PlayerStatsData();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _container = FindObjectOfType<Container>();
        _flowchart = GameObject.FindWithTag(_flowChartTag)?.GetComponent<Flowchart>();

        Load();
    }
}