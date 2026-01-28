using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletScript : MonoBehaviour, IPausable
{
    private Rigidbody2D rb;
    public float force;
    public bool ToRight;

    private bool _paused;

    public void SetPaused(bool paused)
    {
        _paused = paused;

        if (paused)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            float direction = ToRight ? 1f : -1f;

            rb.velocity = Vector2.right * direction * force;
        }
    } 

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
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
