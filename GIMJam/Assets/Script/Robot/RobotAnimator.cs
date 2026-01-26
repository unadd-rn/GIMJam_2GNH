using UnityEngine;
using Cinemachine;

namespace RobotController
{
    public class PlayerAnimator : MonoBehaviour
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
        [SerializeField] private AudioClip[] _footsteps;

        private AudioSource _source;
        private IPlayerController _player;
        private bool _grounded;
        private ParticleSystem.MinMaxGradient _currentGradient;
        private CinemachineImpulseSource _impulseSource;

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
        }

        private void Update()
        {
            if (_player == null) return;

            _anim.transform.localScale = Vector3.Lerp(_anim.transform.localScale, Vector3.one, Time.deltaTime * 10f);

            HandleWalkingState();
            HandleCharacterTilt();
        }

        private void HandleWalkingState()
        {
            bool isWalking = Mathf.Abs(_player.FrameInput.x) > 0.01f;
            _anim.SetBool(WalkingKey, isWalking);
        }

        private void HandleCharacterTilt()
        {
            var targetZ = _grounded ? -_player.FrameInput.x * _maxTilt : 0;
            _anim.transform.localRotation = Quaternion.RotateTowards(_anim.transform.localRotation, Quaternion.Euler(0, 0, targetZ), _tiltSpeed * 10 * Time.deltaTime);
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
                _source.PlayOneShot(_footsteps[UnityEngine.Random.Range(0, _footsteps.Length)]);
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
    }
}