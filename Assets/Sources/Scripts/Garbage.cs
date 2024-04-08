using Fungus;
using UnityEngine;

public class Garbage: MonoBehaviour 
{
    private void Collected()
    {
        Debug.Log("gadsa");
        Destroy(gameObject);
    }
}

