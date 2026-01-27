using System;
using UnityEngine;

public class RobotHealth : MonoBehaviour
{
    public GameObject DeathUI;
    public float health = 3;
    public GameObject healthbar;
    public GameObject bar1, bar2, bar3;
    public static event Action OnRobotHit;
    private float _lastDamageTime;
    [SerializeField] private float _damageCooldown = 1.0f; //stop jittering anjay

    [Header("Effects")]
    [SerializeField] private HitFlash _hitFlash;

    void Start()
    {
        if(DeathUI != null) DeathUI.SetActive(false);
        health = 3;
        UpdateUI();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (Time.time >= _lastDamageTime + _damageCooldown)
            {
                TakeDamage(1f, collision.transform.position);
                _lastDamageTime = Time.time;
            }
        }
    }

    public void TakeDamage(float amount, Vector2 hazardPosition)
    {
        health -= amount;

        OnRobotHit?.Invoke();

        if(HitStopManager.Instance != null) HitStopManager.Instance.Stop(0.15f, 5f);

        RobotController.RobotController controller = GetComponent<RobotController.RobotController>();
        if (controller != null)
        {
            float direction = transform.position.x > hazardPosition.x ? 1 : -1;
            Vector2 knockbackForce = new Vector2(direction * 5f, 3f); // 5 horizontal, 3 vertical
            controller.ApplyKnockback(knockbackForce);
        }

        UpdateUI();
        if (health <= 0) Die();
    }
    

    void UpdateUI()
    {
        Debug.Log($"Health:{health}");
        if(bar3 != null) bar3.SetActive(health >= 3);
        if(bar2 != null) bar2.SetActive(health >= 2);
        if(bar1 != null) bar1.SetActive(health >= 1);
    }

    private void Die()
    {
        Debug.Log("Robot Destroyed!");
        // ? like death stuff
        Time.timeScale = 0f;
        if(DeathUI != null) DeathUI.SetActive(true);
    }
}