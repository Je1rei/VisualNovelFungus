using Fungus;
using UnityEngine;

public class Garbage: MonoBehaviour 
{
    private void Collected()
    {
        Destroy(gameObject);
    }
}

