using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class UFOController : MonoBehaviour
{
    [Header("Movement Settings (Seconds)")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float secondsToTravel = 240f; // Exactly how long one way takes

    [Header("Abduction Settings")]
    [SerializeField] private float liftSpeed = 1f; 
    [SerializeField] private float maxPullStrength = 6f;
    [SerializeField] private float pullRadius = 5f; 
    [SerializeField] private string playerTag = "Player";

    [SerializeField] private CinemachineImpulseSource impulseSource;

    private bool _isAbducting = false;
    private float _movementProgress = 0f;
    private bool _movingToB = true;
    
    private Transform _playerTransform;
    private RobotController.RobotController _playerScript;
    private Rigidbody2D _playerRb;

    private void Update()
    {
        if (!_isAbducting)
        {
            MoveUFO();
            HandleMagneticPull();
        }
        else
        {
            PerformAbduction();
        }
    }

    private void MoveUFO()
    {
        // Calculate how much to move this frame based on total time
        float step = Time.deltaTime / secondsToTravel;

        if (_movingToB)
            _movementProgress += step;
        else
            _movementProgress -= step;

        // Swap directions when we hit the ends
        if (_movementProgress >= 1f) { _movementProgress = 1f; _movingToB = false; }
        else if (_movementProgress <= 0f) { _movementProgress = 0f; _movingToB = true; }

        // Use SmoothStep for a heavy, drifting feel at the turns
        float smoothedT = Mathf.SmoothStep(0f, 1f, _movementProgress);
        transform.position = Vector3.Lerp(pointA.position, pointB.position, smoothedT);
    }

    private void HandleMagneticPull()
    {
        if (_playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player) _playerTransform = player.transform;
            return;
        }

        float horizontalDist = Mathf.Abs(transform.position.x - _playerTransform.position.x);

        if (horizontalDist < pullRadius)
        {
            float closeness = 1f - (horizontalDist / pullRadius);
            float pullPower = Mathf.Pow(closeness, 3) * maxPullStrength;

            // 1. HORIZONTAL PULL (To the middle)
            float xDirection = Mathf.Sign(transform.position.x - _playerTransform.position.x);
            float xMovement = xDirection * pullPower * Time.deltaTime;

            // 2. VERTICAL PULL (Sucking them up)
            // We only pull UP, never down. 
            // This creates the "anti-gravity" feel inside the beam.
            float yMovement = 0f;
            if (_playerTransform.position.y < transform.position.y)
            {
                // The closer to the center, the more they defy gravity
                yMovement = pullPower * 0.5f * Time.deltaTime; 
            }
            
            _playerTransform.position += new Vector3(xMovement, yMovement, 0);
        }
    }

    private void PerformAbduction()
    {
        Vector3 targetPos = transform.position;
        // Move the player position
        _playerTransform.position = Vector3.MoveTowards(_playerTransform.position, targetPos, liftSpeed * Time.deltaTime);

        // NO ROTATION HERE - AbductionVisual handles the sprite tilt
        
        if (Vector2.Distance(_playerTransform.position, targetPos) < 0.2f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            _isAbducting = true;

            // 1. STOP ALL OTHER SHAKES IMMEDIATELY
            // This kills the door shake if it just started
            CinemachineImpulseManager.Instance.Clear();

            _playerRb = other.GetComponent<Rigidbody2D>();
            _playerScript = other.GetComponent<RobotController.RobotController>();

            if (_playerScript != null) _playerScript.enabled = false;
            
            if (_playerRb != null)
            {
                _playerRb.velocity = Vector2.zero;
                _playerRb.isKinematic = true; 
            }

            // 2. TRIGGER THE UFO SHAKE ONLY (on Layer 1)
            if (impulseSource != null) 
            {
                impulseSource.GenerateImpulse(); 
            }

            if (other.TryGetComponent(out AbductionVisual visuals))
            {
                visuals.PlayAbductionSequence();
            }
        }
    }
}