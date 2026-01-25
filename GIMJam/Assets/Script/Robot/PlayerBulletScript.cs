using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private float timer;
    public float force;
    public bool ToRight;
    public float DisappearTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(ToRight){
        rb.velocity = new Vector2(1,0)*force;
        }
        else
        {
            rb.velocity = new Vector2(-1,0)*force;
        }
    }

    void Update() 
    {
        timer += Time.deltaTime;
        if(timer>DisappearTime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Trigger Freeze and Shake (0.1s duration, 3.0 intensity)
            if(HitStopManager.Instance != null) 
                HitStopManager.Instance.Stop(0.1f, 3f);

            EnemyHealth enemy = other.gameObject.GetComponent<EnemyHealth>();
            if(enemy != null)
            {
                enemy.TakeDamage(1);
            }

            Destroy(gameObject);
        }       
    }
}
