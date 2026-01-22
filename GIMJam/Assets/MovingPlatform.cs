using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    [Header("Settings")]
    public Transform posA, posB;
    public float speed = 3f;
    public float waitTime = 1f; // How long to wait at each stop
    
    private Vector3 _targetPos;
    private Rigidbody2D _rb;
    private bool _isWaiting;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
        if (_isWaiting) 
        {
            _rb.velocity = Vector2.zero; // Tell the player we aren't moving
            return;
        }

        Vector2 nextPos = Vector2.MoveTowards(_rb.position, _targetPos, speed * Time.fixedDeltaTime);
        
        // This line is key: it calculates velocity so the player can "read" it
        _rb.velocity = (nextPos - _rb.position) / Time.fixedDeltaTime;
        
        _rb.MovePosition(nextPos);

        if (Vector2.Distance(_rb.position, _targetPos) < 0.05f)
        {
            _rb.velocity = Vector2.zero;
            StartCoroutine(WaitAtPoint());
        }
    }

    private IEnumerator WaitAtPoint()
    {
        _isWaiting = true;
        
        // Toggle the target
        _targetPos = (_targetPos == posA.position) ? posB.position : posA.position;

        yield return new WaitForSeconds(waitTime);
        
        _isWaiting = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Stick the player to the platform
        if (collision.gameObject.CompareTag("Player"))
        {
            // Optional: Check if player is on top (normal.y < -0.5f)
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Unstick the player
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}