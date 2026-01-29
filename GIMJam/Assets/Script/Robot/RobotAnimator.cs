using UnityEngine;
using Cinemachine;

namespace RobotController
{
    public class PlayerAnimator : MonoBehaviour, IPausable
    {
        [Header("References")]
        [SerializeField] private Animator _anim;
        [SerializeField] private SpriteRenderer _sprite;

        [Header("Settings")]
        // [SerializeField, Range(1f, 3f)] private float _maxIdleSpeed = 2;
        [SerializeField] private float _maxTilt = 5;
        [SerializeField] private float _tiltSpeed = 20;

        [Header("Particles")]
        [SerializeField] private ParticleSystem _moveParticles;
        [SerializeField] private ParticleSystem _landParticles;

        [Header("Audio Clips")]
        [SerializeField] private string _footstepSfxName = "Footstep Robot";
        [Header("Footstep Timer Settings")]
        [SerializeField] private float _stepInterval = 0.25f; // Time between steps
        private float _stepTimer;

        private AudioSource _source;
        private IPlayerController _player;
        private bool _grounded;
        private ParticleSystem.MinMaxGradient _currentGradient;
        private CinemachineImpulseSource _impulseSource;

        private bool _paused;

        public void SetPaused(bool paused)
        {
            _paused = paused;
            if (paused)
            {
                _anim.SetBool(WalkingKey, false);
                _anim.SetBool(GroundedKey, true);
                _anim.speed = 0f;
            }
            else
            {
                _anim.speed = 1f;
            }
        }

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _impulseSource = GetComponent<CinemachineImpulseSource>();
            _player = GetComponentInParent<IPlayerController>();
        }

        private void OnEnable()
        {
            _player.Jumped += OnJumped;
            _player.GroundedChanged += OnGroundedChanged;
            
            RobotHealth.OnRobotHit += PlayHitAnimation; 
        }

        private void OnDisable()
        {
            _player.Jumped -= OnJumped;
            _player.GroundedChanged -= OnGroundedChanged;
            
            RobotHealth.OnRobotHit -= PlayHitAnimation;
        }

        private void PlayHitAnimation()
        {
            _anim.Play(HitKey, 0, 0.25f); 
        }

        private void Update()
        {
            if (_paused || _player == null) return;

            _anim.transform.localScale = Vector3.Lerp(_anim.transform.localScale, Vector3.one, Time.deltaTime * 10f);

            HandleWalkingState();

            // --- SCRIPT-BASED FOOTSTEP LOOP ---
            bool isWalking = Mathf.Abs(_player.FrameInput.x) > 0.01f;
            
            if (isWalking && _grounded)
            {
                _stepTimer -= Time.deltaTime;
                if (_stepTimer <= 0)
                {
                    PlayFootstep();
                    _stepTimer = _stepInterval; // Reset the clock
                }
            }
            else
            {
                _stepTimer = 0; // Reset timer when you stop so the first step always plays instantly
            }
        }

        private void HandleWalkingState()
        {
            bool isWalking = Mathf.Abs(_player.FrameInput.x) > 0.01f;
            _anim.SetBool(WalkingKey, isWalking);
        }

        public void PlayFootstep()
        {
            if (!_grounded || _paused) return;

            // Grabs a random clip from the "Footsteps" group in your SoundLibrary
            SoundManager.Instance.PlaySound2D(_footstepSfxName);
        }

        private void OnJumped()
        {
            _anim.SetTrigger(JumpKey); 

            StartCoroutine(JumpEffectsSequence());
        }

        private System.Collections.IEnumerator JumpEffectsSequence()
        {
            yield return new WaitForSeconds(0.16f);

            if(_impulseSource != null) _impulseSource.GenerateImpulse();
        }

        private void OnGroundedChanged(bool grounded, float impact)
        {
            _grounded = grounded;
            
            _anim.SetBool(GroundedKey, grounded);
            
            if (grounded)
            {
                DetectGroundColor();
                // _source.PlayOneShot(_footsteps[UnityEngine.Random.Range(0, _footsteps.Length)]);
                SoundManager.Instance.PlaySound2D(_footstepSfxName);
                _moveParticles.Play();
                _landParticles.Play();
            }
            else
            {
                _moveParticles.Stop();
            }
        }

        private void DetectGroundColor()
        {
            var hit = Physics2D.Raycast(transform.position, Vector3.down, 2);

            if (!hit || hit.collider.isTrigger || !hit.transform.TryGetComponent(out SpriteRenderer r)) return;
            
            var color = r.color;
            _currentGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
            
            SetColor(_moveParticles);
            SetColor(_landParticles);
        }

        private void SetColor(ParticleSystem ps)
        {
            var main = ps.main;
            main.startColor = _currentGradient;
        }

        private static readonly int GroundedKey = Animator.StringToHash("Grounded");
        private static readonly int JumpKey = Animator.StringToHash("Jump");
        private static readonly int WalkingKey = Animator.StringToHash("IsWalking");
        private static readonly int HitKey = Animator.StringToHash("Hit");
    }
}