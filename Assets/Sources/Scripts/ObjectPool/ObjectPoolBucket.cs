using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolBucket : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private float _directionX;
    [SerializeField] private bool _isMoving = false;

    private ObjectPool<Bucket> _pool;
    private List<Bucket> _activeBuckets = new List<Bucket>();

    private void Start()
    {
        List<Bucket> buckets = new List<Bucket>();

        foreach (Transform child in transform)
        {
            Bucket bucket = child.GetComponent<Bucket>();

            if (bucket != null)
            {
                buckets.Add(bucket);
                bucket.Deactivated += OnBucketDeactivated;
            }
        }

        _pool = new ObjectPool<Bucket>(buckets);
    }

    private void Update()
    {
        while(_isMoving)
            ReleasePool();
    }

    public void SetIsMoving() => _isMoving = true;

    public void SetIsFalseMoving() => _isMoving = false;

    public void ReleasePool()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            SetupBucket(CreateBucket());
        }

        _isMoving = false;
    }

    private Bucket CreateBucket()
    {
        Bucket bucket = _pool.GetFreeElement();
        _activeBuckets.Add(bucket);
        bucket.transform.position = bucket.transform.parent.position;

        SpriteRenderer spriteRenderer = bucket.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color32 currentColor = spriteRenderer.color;

            Color32 targetColor = new Color32(currentColor.r, currentColor.g, currentColor.b, 255);

            spriteRenderer.color = Color32.Lerp(currentColor, targetColor, 1);
        }

        return bucket;
    }

    private void SetupBucket(Bucket bucket)
    {
        Rigidbody2D rigidbody2D = bucket.GetComponent<Rigidbody2D>();

        if (rigidbody2D != null)
        {
            rigidbody2D.velocity = new Vector2(_speed * _directionX, 0f);
        }
    }

    private void OnBucketDeactivated(Bucket bucket)
    {
        _activeBuckets.Remove(bucket);

        if (_activeBuckets.Count == 0)
        {
            SetupPool();
        }
    }

    private void SetupPool()
    {
        _isMoving = true;
    }
}
