using UnityEngine;

namespace TarodevController1
{
    [CreateAssetMenu]
    public class ScriptableStats1 : ScriptableObject
    {
        [Header("LAYERS")] [Tooltip("Set this to the layer your player is on")]
        public LayerMask PlayerLayer;

        [Header("INPUT")] [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
        public bool SnapInput = true;

        [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
        public float VerticalDeadZoneThreshold = 0.3f;

        [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
        public float HorizontalDeadZoneThreshold = 0.1f;

        [Header("MOVEMENT")] [Tooltip("The top horizontal movement speed")]
        public float MaxSpeed = 14;

        [Tooltip("The player's capacity to gain horizontal speed")]
        public float Acceleration = 120;

        [Tooltip("The pace at which the player comes to a stop")]
        public float GroundDeceleration = 60;

        [Tooltip("Deceleration in air only after stopping input mid-air")]
        public float AirDeceleration = 30;

        [Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
        public float GroundingForce = -1.5f;

        [Tooltip("The detection distance for grounding and roof detection"), Range(0f, 0.5f)]
        public float GrounderDistance = 0.05f;

        [Header("JUMP")] [Tooltip("The immediate velocity applied when jumping")]
        public float JumpPower = 36;

        [Tooltip("The maximum vertical movement speed")]
        public float MaxFallSpeed = 40;

        [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
        public float FallAcceleration = 110;

        [Tooltip("The gravity multiplier added when jump is released early")]
        public float JumpEndEarlyGravityModifier = 3;

        [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
        public float CoyoteTime = .15f;

        [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
        public float JumpBuffer = .2f;

        [Tooltip("The velocity threshold for the apex. When Y velocity is between -threshold and +threshold, apex gravity is applied")]
        public float ApexThreshold = 5f;

        [Tooltip("The gravity multiplier applied at the apex of a jump. 0.5f means half gravity")]
        public float ApexGravityModifier = 0.5f;

        [Header("DASH")]
        [Tooltip("Key used to dash")]
        public KeyCode DashKey = KeyCode.LeftShift;

        [Tooltip("Dash speed")]
        public float DashSpeed = 30f;

        [Tooltip("How long the dash lasts")]
        public float DashDuration = 0.15f;

        [Tooltip("How many dashes are allowed before landing")]
        public int MaxDashes = 1;

        [Tooltip("Time after dash where gravity is temporarily disabled (Celeste-style)")]
        public float PostDashVerticalLockTime = 0.05f;
        
        [Tooltip("The time between dashes. Prevents spamming")]
        public float DashCooldown = 0.5f;

        [Tooltip("The speed multiplier applied to diagonal dashes. 1 is neutral, 1.15 is snappy")]
        public float DashDiagonalMultiplier = 1.15f;

        [Header("WALLS")]
        [Tooltip("Maximum speed the player slides down a wall")]
        public float WallSlideSpeed = 5f;

        [Tooltip("The force applied when jumping off a wall")]
        public Vector2 WallJumpForce = new Vector2(25, 35);

        [Tooltip("The detection distance for walls"), Range(0f, 0.5f)]
        public float WallDetectorDistance = 0.1f;

        [Header("WALL CLIMB")]
        public float WallClimbVerticalForce = 42f; // Higher than your normal JumpPower (36)
        public float WallClimbHorizontalForce = 4f; // Very small push away

    }
}