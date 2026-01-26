using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
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
        if (other.gameObject.CompareTag("Player"))
        {
            RobotHealth playerHealth = other.gameObject.GetComponent<RobotHealth>();
            if(playerHealth != null)
            {
                playerHealth.TakeDamage(1, transform.position); 
            }
            Destroy(gameObject);
        }
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
