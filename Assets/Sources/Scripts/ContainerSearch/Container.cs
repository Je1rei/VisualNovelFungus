using Fungus;
using UnityEngine;

public class Container : MonoBehaviour
{
    private const string _varIsClosed = "isClosed";
    private const string _varTotalCollected = "totalCollected";

    [SerializeField] private Flowchart _flowchart;

    public int CountCollected { get; private set; }
    public int ClosedGames { get; private set; }

    public void SetCollected()
    {
        CountCollected = _flowchart.GetIntegerVariable(_varTotalCollected);

        if (_flowchart.GetBooleanVariable(_varIsClosed))
        {
            Debug.Log("++");
            ClosedGames++;
        }
    }
}
