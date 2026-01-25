using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPos;
    public float shootEvery;
    private float timer;
    private Vector3 startPos;
    public float floatAmplitude = 0.2f; //bob height
    public float floatFrequency = 1.5f;

    void Start()
    {
        GetComponent<Animator>().Play("idle");
        startPos=transform.position;
    }
    void Update()
    {
        timer += Time.deltaTime;

        float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + new Vector3(0, newY, 0);

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
