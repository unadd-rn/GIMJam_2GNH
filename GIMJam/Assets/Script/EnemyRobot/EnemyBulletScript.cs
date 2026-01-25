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

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<RobotHealth>().Health -= 1;
            Destroy(gameObject);
        }       
    }
}
