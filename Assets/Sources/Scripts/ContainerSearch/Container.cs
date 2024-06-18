using Fungus;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Container : MonoBehaviour
{
    private Flowchart _flowchart;
    private int _prize;

    public int CountCollected { get; private set; }
    public int ClosedGames { get; private set; }
    public int ClosedTests { get; private set; }
    public int CoinCollected {  get; private set; }
    public int PriceStars => _flowchart.GetIntegerVariable(ConstantsContainer._varPrice);


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
        CountCollected = _flowchart.GetIntegerVariable(ConstantsContainer._varTotalCollected);

        if (_flowchart.GetBooleanVariable(ConstantsContainer._varIsClosed))
        {
            ClosedGames++;
            CoinCollected++;
        }
        
        if (_flowchart.GetBooleanVariable(ConstantsContainer._varTestIsClosed))
        {
            ClosedTests++;
            CoinCollected++;
        }
    }

    public void DecreaseCoinCollected()
    {
        CoinCollected -= PriceStars;
    }

    public void IncreaseCoinCollected()
    {
        _prize = _flowchart.GetIntegerVariable(ConstantsContainer._varPrize);
        CoinCollected += _prize;
    }

    public void ClearCoinCollected() => CoinCollected = 0;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _flowchart = GameObject.FindWithTag(ConstantsContainer._flowChartTag)?.GetComponent<Flowchart>();
    }
}
