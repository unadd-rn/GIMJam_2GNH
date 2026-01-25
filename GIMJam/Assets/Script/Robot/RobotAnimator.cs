using UnityEngine;

namespace RobotController
{
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Animator _anim;
        [SerializeField] private SpriteRenderer _sprite;

        [Header("Settings")] 
        [SerializeField, Range(1f, 3f)] private float _maxIdleSpeed = 2;
        [SerializeField] private float _maxTilt = 5;
        [SerializeField] private float _tiltSpeed = 20;

        [Header("Particles")] 
        [SerializeField] private ParticleSystem _jumpParticles;
        [SerializeField] private ParticleSystem _launchParticles;
        [SerializeField] private ParticleSystem _moveParticles;
        [SerializeField] private ParticleSystem _landParticles;

        [Header("Audio Clips")] 
        [SerializeField] private AudioClip[] _footsteps;

        private AudioSource _source;
        private IPlayerController _player;
        private bool _grounded;
        private ParticleSystem.MinMaxGradient _currentGradient;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _player = GetComponentInParent<IPlayerController>();
        }

        private void OnEnable()
        {
            _player.Jumped += OnJumped;
            _player.GroundedChanged += OnGroundedChanged;
            _moveParticles.Play();
        }

        private void OnDisable()
        {
            _player.Jumped -= OnJumped;
            _player.GroundedChanged -= OnGroundedChanged;
            _moveParticles.Stop();
        }

        private void Update()
        {
            if (_player == null) return;

            DetectGroundColor();
            HandleWalkingState(); // New walking logic
            HandleIdleSpeed();
            HandleCharacterTilt();
        }

        private void HandleWalkingState()
        {
            // Check if there is any horizontal input
            bool isWalking = Mathf.Abs(_player.FrameInput.x) > 0.01f;
            
            // Update the animator boolean
            _anim.SetBool(WalkingKey, isWalking);
        }

        private void HandleIdleSpeed()
        {
            var inputStrength = Mathf.Abs(_player.FrameInput.x);
            _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, inputStrength));
            
            _moveParticles.transform.localScale = Vector3.MoveTowards(
                _moveParticles.transform.localScale, 
                Vector3.one * inputStrength, 
                2 * Time.deltaTime
            );
        }

        private void HandleCharacterTilt()
        {
            var targetZ = _grounded ? -_player.FrameInput.x * _maxTilt : 0;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetZ);

            _anim.transform.localRotation = Quaternion.RotateTowards(
                _anim.transform.localRotation, 
                targetRotation, 
                _tiltSpeed * 10 * Time.deltaTime
            );
        }

        private void OnJumped()
        {
            _anim.SetTrigger(JumpKey);
            _anim.ResetTrigger(GroundedKey);

            if (_grounded)
            {
                SetColor(_jumpParticles);
                SetColor(_launchParticles);
                _jumpParticles.Play();
            }
        }

        private void OnGroundedChanged(bool grounded, float impact)
        {
            _grounded = grounded;
            
            if (grounded)
            {
                DetectGroundColor();
                SetColor(_landParticles);

                _anim.SetTrigger(GroundedKey);
                _source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
                _moveParticles.Play();

                _landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, 40, impact);
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
        }

        private void SetColor(ParticleSystem ps)
        {
            var main = ps.main;
            main.startColor = _currentGradient;
        }

        public void PlayAttack() => _anim.SetTrigger(AttackKey);

        // Animator Hashes
        private static readonly int GroundedKey = Animator.StringToHash("Grounded");
        private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
        private static readonly int JumpKey = Animator.StringToHash("Jump");
        private static readonly int WalkingKey = Animator.StringToHash("IsWalking");
        private static readonly int AttackKey = Animator.StringToHash("Attack");
    }
}