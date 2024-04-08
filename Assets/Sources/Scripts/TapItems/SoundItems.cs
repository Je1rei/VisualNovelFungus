using UnityEngine;

public class SoundItems<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private TapItem<T> _item;
    [SerializeField] private AudioSource _audioSource;

    private void OnEnable()
    {
        _item.Clicked += PlaySound;
    }

    private void OnDisable()
    {
        _item.Clicked -= PlaySound;
    }

    private void PlaySound()
    {
        _audioSource.Play();
    }
}
