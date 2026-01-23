using UnityEngine;
using RobotController;

public class ControlPlate : MonoBehaviour
{
    public enum PlateType { Left, Right, Jump, Attack }
    
    [Header("Settings")]
    public PlateType type;
    public RobotController.RobotController robot;
    
    private int _overlapCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Control"))
        {
            _overlapCount++;
            UpdateRobotStatus(true);
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
                UpdateRobotStatus(false);
            }
        }
    }

    private void UpdateRobotStatus(bool active)
    {
        if (robot == null) return;

        switch (type)
        {
            case PlateType.Left:
                robot.ExternalMoveX = active ? -1f : 0f;
                break;
            case PlateType.Right:
                robot.ExternalMoveX = active ? 1f : 0f;
                break;
            case PlateType.Jump:
                robot.ExternalJumpDown = active;
                robot.ExternalJumpHeld = active;
                break;
            case PlateType.Attack:
                if(active) robot.ExternalAttackDown = true;
                break;
        }
    }
}