using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private T _prefab;
    [SerializeField] private GameObject _parentPoints;
    [SerializeField] private Button _buttonNext;

    [SerializeField] private float _delaySpawn = 0;
    [SerializeField] private int _spawnCount = 3;
    
    private bool _isSpawning = true;

    private List<SpawnPoint> _spawnPoints;

    private Coroutine _currentCoroutine;

    public int SpawnCount => _spawnCount;

    private void Awake()
    {
        _spawnPoints = new List<SpawnPoint>(_parentPoints.GetComponentsInChildren<SpawnPoint>());

        _spawnPoints = RandomizeSpawnPoints();
    }

    private void Start()
    {
        SetupCoroutine();
    }

    private void OnDisable()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
    }

    private List<SpawnPoint> RandomizeSpawnPoints()
    {
        List<SpawnPoint> tempPoints = new List<SpawnPoint>();

        for (int i = 0; i < _spawnCount; i++)
        {
            int randomIndex = Random.Range(0, _spawnPoints.Count);

            tempPoints.Add(_spawnPoints[randomIndex]);
            _spawnPoints.RemoveAt(randomIndex);
        }

        return tempPoints;
    }

    private void SetupCoroutine()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        _currentCoroutine = StartCoroutine(SpawnOn());
    }

    private IEnumerator SpawnOn()
    {
        var wait = new WaitForSeconds(_delaySpawn);

        while (_isSpawning)
        {
            Spawn();

            yield return wait;
        }

    }

    private void Spawn()
    {
        foreach (SpawnPoint spawnPoint in _spawnPoints)
        {
            Instantiate(_prefab, spawnPoint.transform);

            _isSpawning = false;
        }
    }
}
