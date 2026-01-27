using System;
using UnityEngine;
using Cinemachine;

namespace RobotController
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class RobotController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStatsRobot _stats;
        [SerializeField] private Transform _visuals;

        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        // Special layers
        private bool _isStickyWall;
        private bool _isStickyGround;
        private bool _isJumpBoostGround;
        private Rigidbody2D _activePlatformRb;

        // Double jump
        private int _maxJumps = 1;
        private int _jumpsRemaining;
        private bool _isJumping;

        // Wall thingy
        private bool _onWall;
        private int _wallDir; // 1 for wall on right, -1 for wall on left

        #region Interface
        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;
        #endregion

        private int _facingDirection = 1;
        public int FacingDirection => _facingDirection;
        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();
            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            GatherInput();
        }

        #region Input

        [HideInInspector] public float ExternalMoveX;
        [HideInInspector] public bool ExternalJumpDown;
        [HideInInspector] public bool ExternalJumpHeld;
        [HideInInspector] public bool ExternalAttackDown;


        private void GatherInput()
        {
            _frameInput.JumpDown = ExternalJumpDown;
            _frameInput.JumpHeld = ExternalJumpHeld;
            _frameInput.Move = new Vector2(ExternalMoveX, 0);

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
                
                ExternalJumpDown = false; 
            }
        }

        #endregion

        private void FixedUpdate()
        {
            CheckCollisions();

            HandleJump();
            HandleDirection();
            HandleGravity();
            
            ApplyMovement();
        }

        #region Collisions
        
        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            // Ground Detection
            RaycastHit2D groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);

            bool isMovingPlatform = groundHit.collider != null && groundHit.collider.gameObject.layer == LayerMask.NameToLayer("MovingPlatform");

            if (isMovingPlatform) {
                _activePlatformRb = groundHit.collider.GetComponent<Rigidbody2D>();
            } else {
                _activePlatformRb = null;
            }

            _isJumpBoostGround = groundHit.collider != null && groundHit.collider.gameObject.layer == LayerMask.NameToLayer("JumpBoost");

            // Wall Detection 
            RaycastHit2D leftWallHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.left, _stats.WallDetectorDistance, ~_stats.PlayerLayer);
            RaycastHit2D rightWallHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.right, _stats.WallDetectorDistance, ~_stats.PlayerLayer);

            bool leftWall = leftWallHit.collider != null;
            bool rightWall = rightWallHit.collider != null;

            _isStickyWall = (leftWall && leftWallHit.collider.gameObject.layer == LayerMask.NameToLayer("Sticky")) ||
                            (rightWall && rightWallHit.collider.gameObject.layer == LayerMask.NameToLayer("Sticky"));

            _isStickyGround = groundHit.collider != null && groundHit.collider.gameObject.layer == LayerMask.NameToLayer("Sticky");

            _onWall = _isStickyWall && (leftWall || rightWall) && !_grounded && _rb.velocity.y < 0.1f;
            _wallDir = rightWall ? 1 : -1;

            if (!_grounded && groundHit)
            {
                _grounded = true;
                _jumpsRemaining = _maxJumps;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
           /* if (groundHit.collider != null) 
                Debug.Log("robot am touching: " + groundHit.collider.name + " on layer: " + LayerMask.LayerToName(groundHit.collider.gameObject.layer));
            else 
                Debug.Log("robot am touching NOTHING");*/
        }

        #endregion

        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump()
        {
            if (_grounded && _isJumpBoostGround)
            {
                _frameVelocity.y = _stats.JumpPower * 2.5f; 
                _jumpToConsume = false;
                _grounded = false; 
                Jumped?.Invoke();
                return;
            }

            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_onWall)
            {
                float horizontalInput = _frameInput.Move.x;
                bool pushingAgainstWall = (horizontalInput > 0 && _wallDir == 1) || (horizontalInput < 0 && _wallDir == -1);

                if (pushingAgainstWall)
                {
                    _frameVelocity.y = _stats.WallClimbVerticalForce;
                    _frameVelocity.x = -_wallDir * _stats.WallClimbHorizontalForce;
                }
                else
                {
                    _frameVelocity = new Vector2(-_wallDir * _stats.WallJumpForce.x, _stats.WallJumpForce.y);
                }
                
                _endedJumpEarly = false;
                _bufferedJumpUsable = false;
                _coyoteUsable = false;
                Jumped?.Invoke();
            }
            else if (_grounded || CanUseCoyote) 
            {
                ExecuteJump();
            }
            else if (_jumpsRemaining > 0)
            {
                ExecuteJump();
                _jumpsRemaining--;
            }
            else
            {
                return;
            }

            _jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            if (_isJumping) return;
            
            _isJumping = true;
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;

            Jumped?.Invoke(); 

            StartCoroutine(JumpForceDelay());
        }

        private System.Collections.IEnumerator JumpForceDelay()
        {
            yield return new WaitForSeconds(0.16f); 
            
            float currentJumpPower = _isStickyGround ? _stats.JumpPower * 0.5f : _stats.JumpPower;
            _frameVelocity.y = currentJumpPower;
            _grounded = false;
            
            _isJumping = false;
        }

        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            if (_frameInput.Move.x != 0)
            {
                _facingDirection = (int)Mathf.Sign(_frameInput.Move.x);

                if (_visuals != null)
                {
                    float rotationY = _facingDirection == 1 ? 180 : 0;
                    _visuals.rotation = Quaternion.Euler(0, rotationY, 0);
                }
            }

            float targetSpeed = _frameInput.Move.x * _stats.MaxSpeed;

            if (_isStickyGround || _onWall)
            {
                targetSpeed *= 0.4f;
            }

            if (_frameInput.Move.x == 0)
            {
                float deceleration = _isStickyGround ? _stats.GroundDeceleration * 2 : (_grounded ? _stats.GroundDeceleration : _stats.AirDeceleration);
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, targetSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (_onWall && _frameVelocity.y < 0)
            {
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.WallSlideSpeed, _stats.FallAcceleration * Time.fixedDeltaTime);
            }
            else if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                bool isAtApex = Mathf.Abs(_frameVelocity.y) < _stats.ApexThreshold;
                
                if (isAtApex) 
                {
                    inAirGravity *= _stats.ApexGravityModifier;
                }

                if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;

                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        private void ApplyMovement()
        {
            if (_activePlatformRb != null)
            {
                _rb.velocity = _frameVelocity + _activePlatformRb.velocity;
            }
            else
            {
                _rb.velocity = _frameVelocity;
            }
        }

        public void ApplyKnockback(Vector2 force)
        {
            _rb.velocity = Vector2.zero; 
            _rb.AddForce(force, ForceMode2D.Impulse);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}