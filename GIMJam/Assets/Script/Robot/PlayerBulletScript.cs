using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public float force;
    public bool ToRight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        float direction = ToRight ? 1f : -1f;

        rb.velocity = Vector2.right * direction * force;

        transform.localScale = new Vector3(direction, 1, 1);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Enemy"))
        {
            if (HitStopManager.Instance != null)
                HitStopManager.Instance.Stop(0.1f, 3f);

            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }

            Destroy(gameObject);
        }
    }
}
