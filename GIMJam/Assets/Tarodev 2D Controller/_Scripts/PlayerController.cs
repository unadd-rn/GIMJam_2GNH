using System;
using UnityEngine;
using Cinemachine;

namespace TarodevController
{
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// I have a premium version on Patreon, which has every feature you'd expect from a polished controller. Link: https://www.patreon.com/tarodev
    /// You can play and compete for best times here: https://tarodev.itch.io/extended-ultimate-2d-controller
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/tarodev
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats _stats;

        // idk what this is
        private Rigidbody2D _rb;
        private BoxCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        //Special layers
        private bool _isStickyWall;
        private bool _isStickyGround;
        private bool _isJumpBoostGround;
        private Rigidbody2D _activePlatformRb;

        // Double jump
        private int _maxJumps = 1;
        private int _jumpsRemaining;

        // dashing
        private int _dashesRemaining;
        private bool _isDashing;
        private float _dashTime;
        private float _postDashLockTimer;
        private Vector2 _lastDashDirection;
        private float _dashCooldownTimer;

        //wall thingy
        private bool _onWall;
        private int _wallDir; // 1 for wall on right, -1 for wall on left

        #region Interface

        // idk this either
        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        #endregion

        // dash camera shake
        private CinemachineImpulseSource _impulseSource;

        private int _facingDirection = 1; // 1 = right, -1 = left

        private float _time;

        private void Awake()
        {
            _impulseSource = GetComponent<CinemachineImpulseSource>();

            _dashesRemaining = _stats.MaxDashes;
            
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<BoxCollider2D>();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            _dashCooldownTimer -= Time.deltaTime; // Count down every frame
            GatherInput();
        }

        private void GatherInput()
        {
            if (DialogueManager.GetInstance().dialogueIsPlaying)
            {
                _frameInput = new FrameInput
                {
                    JumpDown = false,
                    JumpHeld = false,
                    Move = Vector2.zero
                };
                return;
            }

            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }

            if (Input.GetKeyDown(_stats.DashKey))
            {
                TryDash();
            }

        }

        private void FixedUpdate()
        {
            if (_isDashing)
            {
                _dashTime -= Time.fixedDeltaTime;
                _rb.velocity = _frameVelocity;

                if (_dashTime <= 0)
                {
                    _isDashing = false;
                    
                    // Hard stop in the air
                    _frameVelocity = Vector2.zero; 

                    // Hang Time
                    _postDashLockTimer = _stats.PostDashVerticalLockTime; 
                }

                return;
            }
            
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

            Vector2 boxSize = new Vector2(_col.size.x * 0.9f, 0.05f);
            
            float castDistance = _stats.GrounderDistance + 0.05f;
            RaycastHit2D groundHit = Physics2D.BoxCast(_col.bounds.center, boxSize, 0, Vector2.down, (_col.size.y / 2) + _stats.GrounderDistance, ~_stats.PlayerLayer);

            bool isMovingPlatform = groundHit.collider != null && groundHit.collider.gameObject.layer == LayerMask.NameToLayer("MovingPlatform");
            _activePlatformRb = isMovingPlatform ? groundHit.collider.GetComponent<Rigidbody2D>() : null;

            _isJumpBoostGround = groundHit.collider != null && groundHit.collider.gameObject.layer == LayerMask.NameToLayer("JumpBoost");

            Vector2 wallBoxSize = new Vector2(0.05f, _col.size.y * 0.8f);
            RaycastHit2D leftWallHit = Physics2D.BoxCast(_col.bounds.center, wallBoxSize, 0, Vector2.left, (_col.size.x / 2) + _stats.WallDetectorDistance, ~_stats.PlayerLayer);
            RaycastHit2D rightWallHit = Physics2D.BoxCast(_col.bounds.center, wallBoxSize, 0, Vector2.right, (_col.size.x / 2) + _stats.WallDetectorDistance, ~_stats.PlayerLayer);

            bool leftWall = leftWallHit.collider != null;
            bool rightWall = rightWallHit.collider != null;

            // Sticky Logic
            _isStickyWall = (leftWall && leftWallHit.collider.gameObject.layer == LayerMask.NameToLayer("Sticky")) ||
                            (rightWall && rightWallHit.collider.gameObject.layer == LayerMask.NameToLayer("Sticky"));
            _isStickyGround = groundHit.collider != null && groundHit.collider.gameObject.layer == LayerMask.NameToLayer("Sticky");

            _onWall = _isStickyWall && (leftWall || rightWall) && !_grounded && _rb.velocity.y < 0.1f;
            _wallDir = rightWall ? 1 : -1;

            // Ground Logic
            if (!_grounded && groundHit)
            {
                _grounded = true;
                _jumpsRemaining = _maxJumps;
                _dashesRemaining = _stats.MaxDashes;
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
                _frameVelocity.y = _stats.JumpPower * 2.5f; // 2.5x height, adjust as needed
                _jumpToConsume = false;
                _grounded = false; // Force leave ground
                Jumped?.Invoke();
                return;
            }

            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_onWall)
            {
                float horizontalInput = _frameInput.Move.x;
                // Detect if we are holding the stick TOWARDS the wall
                bool pushingAgainstWall = (horizontalInput > 0 && _wallDir == 1) || (horizontalInput < 0 && _wallDir == -1);

                if (pushingAgainstWall)
                {
                    //  WALL CLIMB JUMP 
                    _frameVelocity.y = _stats.WallClimbVerticalForce; // Strong upward boost
                    _frameVelocity.x = -_wallDir * _stats.WallClimbHorizontalForce; // Tiny push away
                }
                else
                {
                    // WALL KICK
                    _frameVelocity = new Vector2(-_wallDir * _stats.WallJumpForce.x, _stats.WallJumpForce.y);
                }
                
                _endedJumpEarly = false;
                _bufferedJumpUsable = false;
                _coyoteUsable = false;
                Jumped?.Invoke();
            }
            else if (_grounded || CanUseCoyote) // Normal Ground Jump
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
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;

            float currentJumpPower = _isStickyGround ? _stats.JumpPower * 0.5f : _stats.JumpPower;
            
            _frameVelocity.y = currentJumpPower;
            Jumped?.Invoke();
        }

        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            if (_frameInput.Move.x != 0)
            {
                _facingDirection = (int)Mathf.Sign(_frameInput.Move.x);
                transform.localScale = new Vector3(-_facingDirection, 1, 1);
            }

            // Determine target speed and acceleration
            float targetSpeed = _frameInput.Move.x * _stats.MaxSpeed;
            float currentAccel = _stats.Acceleration;

            // SLOW DOWN if on a sticky wall
            if (_isStickyGround || _onWall)
            {
                targetSpeed *= 0.4f; // 40% of normal speed
            }

            if (_frameInput.Move.x == 0)
            {
                // When you stop moving on sticky ground, you stop almost instantly
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
            if (_postDashLockTimer > 0)
            {
                _postDashLockTimer -= Time.fixedDeltaTime;
                // While the lock is active, gravity is 0 and we stay frozen.
                return; 
            }

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

                // --- THE FIX ---
                // We only apply Apex Threshold if we AREN'T coming out of a dash hang
                // This preserves your jump feel but lets the dash drop fast.
                bool isAtApex = Mathf.Abs(_frameVelocity.y) < _stats.ApexThreshold;
                
                // If you want to be extra precise, only float if velocity is barely moving
                // and you haven't just finished a dash.
                if (isAtApex) 
                {
                    inAirGravity *= _stats.ApexGravityModifier;
                }

                if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;

                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }


        #endregion

        #region Dashing

        private void TryDash()
        {
            // Check if cooldown is still active OR if already dashing
            if (_dashCooldownTimer > 0 || _isDashing) return;

            if (!_grounded && _dashesRemaining <= 0) return;

            Vector2 dashInput = new Vector2(_frameInput.Move.x, _frameInput.Move.y);
            if (dashInput == Vector2.zero) dashInput = Vector2.right * _facingDirection;

            bool isDiagonal = dashInput.x != 0 && dashInput.y != 0;
            Vector2 dashDirection = dashInput.normalized;
            
            _lastDashDirection = dashDirection;
            _isDashing = true;
            _dashTime = _stats.DashDuration;
            
            // cooldown
            _dashCooldownTimer = _stats.DashCooldown;

            if (!_grounded) _dashesRemaining--;

            _frameVelocity = Vector2.zero;

            float finalDashSpeed = isDiagonal 
                ? _stats.DashSpeed * _stats.DashDiagonalMultiplier 
                : _stats.DashSpeed;

            _frameVelocity = dashDirection * finalDashSpeed;
            if (_impulseSource != null) _impulseSource.GenerateImpulse();
        }

        // private void EndDashFreeze()
        // {
        //     Time.timeScale = 1f;
        // }

        #endregion

        private void ApplyMovement()
        {
            if (_activePlatformRb != null)
            {
                // Add the platform's velocity to the player's calculated frame velocity
                _rb.velocity = _frameVelocity + _activePlatformRb.velocity;
            }
            else
            {
                _rb.velocity = _frameVelocity;
            }
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