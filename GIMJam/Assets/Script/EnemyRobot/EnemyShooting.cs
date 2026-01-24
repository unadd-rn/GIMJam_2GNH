using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPos;
    public float shootEvery;
    private float timer;

    void Start()
    {
        GetComponent<Animator>().Play("idle");
    }
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > shootEvery)
        {
            timer = 0;
            shoot();
        }
    }

    void shoot()
    {
        Instantiate(bullet, bulletPos.position, Quaternion.identity);
    }
}
