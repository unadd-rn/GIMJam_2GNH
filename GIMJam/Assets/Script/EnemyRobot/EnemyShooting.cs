using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour, IPausable
{
    public GameObject bullet;
    public Transform bulletPos;
    public float shootEvery;

    public float floatAmplitude = 0.2f;
    public float floatFrequency = 1.5f;

    private float timer;
    private Vector3 startPos;
    private bool _paused;
    private Animator _anim;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _anim.Play("idle");
        startPos = transform.position;
    }

    void Update()
    {
        if (_paused) return; 

        timer += Time.deltaTime;

        float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + new Vector3(0, newY, 0);

        if (timer > shootEvery)
        {
            timer = 0;
            Shoot();
        }
    }

    void Shoot()
    {
        if (_paused) return; 
        SoundManager.Instance.PlaySound2D("Enemy Shoot");
        Instantiate(bullet, bulletPos.position, Quaternion.identity);
    }

    public void SetPaused(bool paused)
    {
        _paused = paused;

        if (_anim != null)
            _anim.speed = paused ? 0f : 1f;   //  freeze animation
    }
}
