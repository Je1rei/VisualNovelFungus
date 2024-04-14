using Fungus;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Container : MonoBehaviour
{
    private const string _flowChartTag = "FlowChart";

    private const string _varIsClosed = "isClosed";
    private const string _varTotalCollected = "totalCollected";

    private Flowchart _flowchart;

    public int CountCollected { get; private set; }
    public int ClosedGames { get; private set; }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetCollected()
    {
        CountCollected = _flowchart.GetIntegerVariable(_varTotalCollected);

        if (_flowchart.GetBooleanVariable(_varIsClosed))
        {
            ClosedGames++;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _flowchart = GameObject.FindWithTag(_flowChartTag)?.GetComponent<Flowchart>();
    }
}
