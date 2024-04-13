using UnityEngine;

public class MoveTo : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;

    private Vector3 _startPosition;

    private bool _isMoving = true;

    public void Awake()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        if (_isMoving)
        {
            Move();
        }
    }

    public void Move()
    {
        float distanceToTarget = Vector3.Distance(transform.position, _target.position);

        if (distanceToTarget < 0.1f)
        {
            transform.position = _startPosition;
        }

        float step = _speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, _target.position, step);
    }

    public void StopMoving()
    {
        _isMoving = false;
    }
}
