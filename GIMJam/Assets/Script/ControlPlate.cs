using UnityEngine;
using RobotController;

public class ControlPlate : MonoBehaviour
{
    public enum PlateType { Left, Right, Jump, Attack }
    
    [Header("References")]
    public RobotController.RobotController robot;
    public SpriteRenderer spriteRenderer;

    [Header("Settings")]
    public PlateType type;
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    
    private int _overlapCount = 0;

    private void Start()
    {
        if (spriteRenderer != null && inactiveSprite != null)
        {
            spriteRenderer.sprite = inactiveSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Control"))
        {
            _overlapCount++;
            UpdatePlateState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
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

    private void UpdatePlateState(bool isActive)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;

        if (robot == null) return;

        switch (type)
        {
            case PlateType.Jump:
                // This tells the robot the button is CURRENTLY being held
                robot.ExternalJumpHeld = isActive;
                
                // This sends the "Initial Press" signal only when stepping ON
                if (isActive) 
                {
                    robot.ExternalJumpDown = true;
                }
                break;

            case PlateType.Attack:
                if (isActive) robot.ExternalAttackDown = true;
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