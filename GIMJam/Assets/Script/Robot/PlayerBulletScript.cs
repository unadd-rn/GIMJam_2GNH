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

        if (ToRight)
            rb.velocity = Vector2.right * force;
        else
            rb.velocity = Vector2.left * force;
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
