using Fungus;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public void TogglePause()
    {
        Time.timeScale = Time.timeScale == 1f ? 0f : 1f;
    }    
}
