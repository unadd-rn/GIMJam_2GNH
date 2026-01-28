using UnityEngine;
using RobotController;

public class ControlPlate : MonoBehaviour, IPausable
{
    public enum PlateType { Left, Right, Jump, Attack }

    [Header("References")]
    public RobotController.RobotController robot;
    public SpriteRenderer spriteRenderer;

    [Header("Settings")]
    public PlateType type;
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    private int _overlapCount;
    private bool _paused;

    private void Start()
    {
        _overlapCount = 0;

        if (spriteRenderer != null && inactiveSprite != null)
        {
            spriteRenderer.sprite = inactiveSprite;
        }
    }

    // =====================
    // IPausable
    // =====================
    public void SetPaused(bool paused)
    {
        _paused = paused;

        if (paused)
        {
            // Reset state
            _overlapCount = 0;

            // Reset visuals
            if (spriteRenderer != null && inactiveSprite != null)
            {
                spriteRenderer.sprite = inactiveSprite;
            }

            // Reset robot input
            if (robot != null)
            {
                robot.ExternalMoveX = 0f;
                robot.ExternalJumpHeld = false;
                robot.ExternalJumpDown = false;
                robot.ExternalAttackDown = false;
            }
        }
    }

    // =====================
    // Triggers
    // =====================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_paused) return;

        if (collision.CompareTag("Control"))
        {
            _overlapCount++;
            UpdatePlateState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_paused) return;

        if (collision.CompareTag("Control"))
        {
            _overlapCount--;

            if (_overlapCount <= 0)
            {
                _overlapCount = 0;
                UpdatePlateState(false);
            }
        }
    }

    // =====================
    // Logic
    // =====================
    private void UpdatePlateState(bool isActive)
    {
        if (_paused) return;

        // Update visuals
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;
        }

        if (robot == null) return;

        switch (type)
        {
            case PlateType.Jump:
                robot.ExternalJumpHeld = isActive;
                if (isActive)
                {
                    robot.ExternalJumpDown = true;
                }
                break;

            case PlateType.Attack:
                if (isActive)
                {
                    robot.ExternalAttackDown = true;
                }
                break;

            case PlateType.Left:
                robot.ExternalMoveX = isActive ? -1f : 0f;
                break;

            case PlateType.Right:
                robot.ExternalMoveX = isActive ? 1f : 0f;
                break;
        }
    }
}
