using Fungus;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JSONSaver : MonoBehaviour
{
    private string _purchasedVariable = "";

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
        _filePath = Path.Combine(Application.persistentDataPath, ConstantsSavers._playerDataFileName);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void CreatePlayer()
    {
        FindMaxID();

        string playerName = _flowchart.GetStringVariable(ConstantsSavers._playerNameVar);

        _currentPlayer = new PlayerData(_nextID, playerName, ConstantsSavers._startLocationScene);
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

        SaveInt(ConstantsSavers._totalCollected, 0);
        SaveInt(ConstantsSavers._closedGames, 0);
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
        if (SceneManager.GetActiveScene().name != ConstantsSavers._mainMenuScene)
            playerData.SetCurrentScene(SceneManager.GetActiveScene().name);

        string jsonDataStats = JsonUtility.ToJson(playerData);

        string playerFilePath = Path.Combine(Application.persistentDataPath, $"{ConstantsSavers._playerDataFileName}_{playerData.ID}.json");
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

        string playerFilePath = Path.Combine(Application.persistentDataPath, $"{ConstantsSavers._playerDataFileName}_{_nextID - 1}.json");

        if (File.Exists(playerFilePath))
        {
            string jsonDataPlayer = File.ReadAllText(playerFilePath);

            _currentPlayer = JsonUtility.FromJson<PlayerData>(jsonDataPlayer);

            LoadPlayerStats(_currentPlayer);
            LoadPlayerIsPurchased();

            Debug.Log("Данные загружены из файла: " + playerFilePath);

            SaveBool(ConstantsSavers._isLoadedVar, true);

            SaveString(ConstantsSavers._playerNameVar, _currentPlayer.Name);
            SaveString(ConstantsSavers._lastClosedEpisodeVar, _currentPlayer.CurrentSceneName);

            SaveInt(ConstantsSavers._totalCollected, _currentPlayerStats.CountCollected);
            SaveInt(ConstantsSavers._closedGames, _currentPlayerStats.ClosedGames);
            SaveInt(ConstantsSavers._closedTests, _currentPlayerStats.ClosedTests);
            SaveInt(ConstantsSavers._coinCollected, _currentPlayerStats.CoinCollected);

        }
        else
        {
            Debug.LogWarning("Файл даты игрока не найден: " + playerFilePath);
            SaveBool(ConstantsSavers._isLoadedVar, false);
        }
    }

    private void FindMaxID()
    {
        DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = directory.GetFiles($"{ConstantsSavers._playerDataFileName}_*.json");

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

    public void SavePurchased()
    {
        string level = _flowchart.GetStringVariable(ConstantsSavers._purchasedVar);
        _currentPlayer.AddPurchase(level);
    }

    private void SavePlayerStats(PlayerData playerData)
    {
        _currentPlayerStats.SetClosedGames(_container.ClosedGames);
        _currentPlayerStats.SetCountCollected(_container.CountCollected);
        _currentPlayerStats.SetCountClosedTests(_container.ClosedTests);
        _currentPlayerStats.SetCoinCollected(_container.CoinCollected);

        string jsonDataStats = JsonUtility.ToJson(_currentPlayerStats);
        string playerStatsFilePath = Path.Combine(Application.persistentDataPath, $"{ConstantsSavers._playerStatsFileName}_{playerData.ID}.json");

        File.WriteAllText(playerStatsFilePath, jsonDataStats);

        Debug.Log("Файл данных статистики игрока сохранен: " + playerStatsFilePath);
    }

    private void LoadPlayerStats(PlayerData playerData)
    {
        string playerStatsFilePath = Path.Combine(Application.persistentDataPath, $"{ConstantsSavers._playerStatsFileName}_{playerData.ID}.json");

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

    private void LoadPlayerIsPurchased()
    {
        if (_currentPlayer != null && _flowchart != null)
        {
            foreach (string purchasedLevel in _currentPlayer.LevelsPurchased)
            {
                if (CheckIfVariableExists(purchasedLevel) && CheckIfVariableExists($"{purchasedLevel}"))
                {
                    _flowchart.SetBooleanVariable($"{purchasedLevel}", true);
                }
            }
        }
    }

    private bool CheckIfVariableExists(string variableName)
    {
        return _flowchart.HasVariable(variableName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _container = FindObjectOfType<Container>();
        _flowchart = GameObject.FindWithTag(ConstantsSavers._flowChartTag)?.GetComponent<Flowchart>();

        Load();
    }
}

public static class ConstantsSavers
{
    public const string _mainMenuScene = "Main Menu";
    public const string _startLocationScene = "StartLocation";

    public const string _flowChartTag = "FlowChart";
    public const string _playerDataFileName = "playerData";
    public const string _playerStatsFileName = "playerStats";

    public const string _totalCollected = "totalCollectedGame";
    public const string _closedGames = "closedGames";
    public const string _closedTests = "closedTests";
    public const string _coinCollected = "coinCollected";
    public const string _playerNameVar = "playerName";
    public const string _isLoadedVar = "isLoaded";
    public const string _lastClosedEpisodeVar = "lastClosedEpisode";
    public const string _purchasedVar = "purchasedVar";

    public const string _firstMiniGame = "isPurchased_MG_1";
    public const string _secondMiniGame = "isPurchased_MG_2";
    public const string _thirdMiniGame = "isPurchased_MG_3";
    public const string _fourthMiniGame = "isPurchased_MG_4";
    public const string _fifthMiniGame = "isPurchased_MG_5";

    public const string _firstTest = "isPurchased_Test_1";
    public const string _secondTest = "isPurchased_Test_2";
    public const string _thirdTest = "isPurchased_Test_3";

    public static JSONSaver JSONSaver
    {
        get => default;
        set
        {
        }
    }
}
