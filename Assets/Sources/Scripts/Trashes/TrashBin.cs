using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private T _prefab;

    [SerializeField] private int _needCollect;
    [SerializeField] private int _increaseValue;

    private int _currentCollected;

    private void IncreaseCollected()
    {
        if (_currentCollected < _needCollect)
        {
            _currentCollected += _increaseValue;
        }
    }
}

