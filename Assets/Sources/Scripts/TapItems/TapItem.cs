using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapItem<T> : MonoBehaviour, IPointerClickHandler where T : MonoBehaviour
{
    [SerializeField] private T _item;
    [SerializeField] private int _increaseCollected;
    [SerializeField] private float _delay;

    [SerializeField] private Container _container;

    public bool IsClicked { get; private set; } = false;

    public event Action Clicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerPress == gameObject)
        {
            if (IsClicked == false)
            {
                Clicked?.Invoke();
                StartCoroutine(WaitDelay());
                IsClicked = true;
            }
        }
    }

    public IEnumerator WaitDelay()
    {
        yield return new WaitForSeconds(_delay); 

        //_container.SetCollected();
        Destroy(gameObject);
    }
}
