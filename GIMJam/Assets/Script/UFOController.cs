using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class UFOController : MonoBehaviour, IPausable
{
    [Header("Movement Settings (Seconds)")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float secondsToTravel = 30f;

    [Header("Abduction Settings")]
    [SerializeField] private float liftSpeed = 1f; 
    [SerializeField] private float maxPullStrength = 6f;
    [SerializeField] private float pullRadius = 5f; 
    [SerializeField] private string playerTag = "Player";

    [SerializeField] private CinemachineImpulseSource impulseSource;
    public RobotHealth playerHealth;

    private bool _isAbducting = false;
    private float _movementProgress = 0f;
    private bool _movingToB = true;

    private bool _paused;
    
    private Transform _playerTransform;
    private RobotController.RobotController _playerScript;
    private Rigidbody2D _playerRb;

    private void Update()
    {
        if (_paused) return; 
        if (!_isAbducting)
        {
            if (!DialogueManager.GetInstance().dialogueIsPlaying)
            {
                MoveUFO();
            }
            HandleMagneticPull();
        }
        else
        {
            MusicManager.Instance.StopMusic();
            PerformAbduction();
        }

        
    }

    public void SetPaused(bool paused)
    {
        _paused = paused;

        if (paused && _playerRb != null)
        {
            _playerRb.velocity = Vector2.zero;
        }
    }

    private void MoveUFO()
    {
        float step = Time.deltaTime / secondsToTravel;

        if (_movingToB)
            _movementProgress += step;
        else
            _movementProgress -= step;

        if (_movementProgress >= 1f) { _movementProgress = 1f; _movingToB = false; }
        else if (_movementProgress <= 0f) { _movementProgress = 0f; _movingToB = true; }

        //float smoothedT = Mathf.SmoothStep(0f, 1f, _movementProgress);
        transform.position = Vector3.Lerp(pointA.position, pointB.position, _movementProgress);
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

            float xDirection = Mathf.Sign(transform.position.x - _playerTransform.position.x);
            float xMovement = xDirection * pullPower * Time.deltaTime;

            float yMovement = 0f;
            if (_playerTransform.position.y < transform.position.y)
            {
                yMovement = pullPower * 0.5f * Time.deltaTime; 
            }
            
            _playerTransform.position += new Vector3(xMovement, yMovement, 0);
        }
    }

    private void PerformAbduction()
    {
        SoundManager.Instance.PlaySound2D("Hit UFO");
        Vector3 targetPos = transform.position;
        _playerTransform.position = Vector3.MoveTowards(_playerTransform.position, targetPos, liftSpeed * Time.deltaTime);

        
        if (Vector2.Distance(_playerTransform.position, targetPos) < 0.2f)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            if(playerHealth != null)
            {
                playerHealth.TakeDamage(3, transform.position); 
                //playerHealth.health -=3;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            _isAbducting = true;

            CinemachineImpulseManager.Instance.Clear();

            _playerRb = other.GetComponent<Rigidbody2D>();
            _playerScript = other.GetComponent<RobotController.RobotController>();

            if (_playerScript != null) _playerScript.enabled = false;
            
            if (_playerRb != null)
            {
                _playerRb.velocity = Vector2.zero;
                _playerRb.isKinematic = true; 
            }

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