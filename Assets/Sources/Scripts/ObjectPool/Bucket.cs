using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bucket : MonoBehaviour
{
    [SerializeField] private float _lifetime = 4f;

    public event Action<Bucket> Deactivated;

    private void OnEnable()
    {
        StartCoroutine("LifeRoutine");
    }

    private void OnDisable()
    {
        StopCoroutine("LifeRoutine");
    }

    private IEnumerator LifeRoutine()
    {
        yield return new WaitForSecondsRealtime(_lifetime);

        Deactivate();
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
        Deactivated?.Invoke(this);
    }
}
