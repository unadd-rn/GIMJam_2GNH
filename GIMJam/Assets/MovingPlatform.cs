using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Settings")]
    public Transform posA, posB;
    public float speed = 3f;
    
    private Vector3 _targetPos;
    private Rigidbody2D _rb;

    private void Awake()
    {
        // We use a Rigidbody2D on the platform set to "Kinematic" 
        // for the smoothest physical movement.
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null) _rb = gameObject.AddComponent<Rigidbody2D>();
        
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Start()
    {
        transform.position = posA.position;
        _targetPos = posB.position;
    }

    private void FixedUpdate()
    {
        // Move the platform using MovePosition for smooth physics interaction
        Vector2 newPos = Vector2.MoveTowards(_rb.position, _targetPos, speed * Time.fixedDeltaTime);
        _rb.MovePosition(newPos);

        // Switch targets when close enough
        if (Vector2.Distance(_rb.position, _targetPos) < 0.1f)
        {
            _targetPos = (_targetPos == posA.position) ? posB.position : posA.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // When the player lands on the platform, make the player a child of the platform
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // When the player jumps or leaves, remove them from the platform hierarchy
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}