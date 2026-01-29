using UnityEngine;
using System.Collections;
using Cinemachine; 

public class PermanentPressurePlate : MonoBehaviour
{
    [Header("References")]
    public GameObject door;
    public Sprite pressedSprite;
    public CinemachineVirtualCamera playerVCam;

    [Header("Settings")]
    public float doorLowerAmount = 2.0f;
    public float moveSpeed = 2.0f;
    public float colliderShrinkRatio = 0.6f;
    public float focusDuration = 2.0f;
    [Range(0.1f, 5.0f)] public float shakeIntensity = 1.0f; 

    private Vector3 doorOpenPos;
    private bool isActivated = false;
    private bool shouldMoveDoor = false;
    
    private SpriteRenderer sr;
    private BoxCollider2D col;
    private CinemachineImpulseSource impulseSource;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        impulseSource = door.GetComponent<CinemachineImpulseSource>();
        
        doorOpenPos = door.transform.position + (Vector3.down * doorLowerAmount);
    }

    void Update()
    {
        if (shouldMoveDoor)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, doorOpenPos, moveSpeed * Time.deltaTime);
            
            if (Vector3.Distance(door.transform.position, doorOpenPos) < 0.001f)
            {
                shouldMoveDoor = false;
            }
        }
    }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (!isActivated && collision.gameObject.CompareTag("Player"))
    //     {
    //         StartCoroutine(CameraPanSequence());
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivated && collision.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySound2D("Pressure Plate");
            StartCoroutine(CameraPanSequence());
        }
    }

    IEnumerator CameraPanSequence()
    {
        isActivated = true;

        if (pressedSprite != null) sr.sprite = pressedSprite;
        col.size = new Vector2(col.size.x, col.size.y * colliderShrinkRatio);

        PauseManager.ToggleEntities(false);

        Transform originalTarget = playerVCam.Follow;
        playerVCam.Follow = door.transform; 

        yield return new WaitForSeconds(1.2f); 

        if (impulseSource != null) impulseSource.GenerateImpulse(shakeIntensity);
        
        shouldMoveDoor = true;
        SoundManager.Instance.PlaySound2D("Door");
        yield return new WaitForSeconds(focusDuration);

        playerVCam.Follow = originalTarget;
        yield return new WaitForSeconds(1.0f);

        PauseManager.ToggleEntities(true);
    }

    // void ToggleEntities(bool state)
    // {
    //     GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    //     GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    //     GameObject[] finalboss = GameObject.FindGameObjectsWithTag("FinalBoss");

    //     foreach (GameObject p in players) ToggleMovement(p, state);
    //     foreach (GameObject e in enemies) ToggleMovement(e, state);
    //     foreach (GameObject n in finalboss) ToggleMovement(n, state);
    // }

    // void ToggleMovement(GameObject obj, bool state)
    // {
    //     Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
    //     if (rb != null)
    //     {
    //         if (!state) rb.velocity = Vector2.zero; 
    //         rb.simulated = state; 
    //     }

    //     Animator anim = obj.GetComponent<Animator>();
    //     if (anim != null) anim.enabled = state;

    //     MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
    //     foreach (MonoBehaviour script in scripts)
    //     {
    //         if (script != this && !(script is CinemachineVirtualCamera)) 
    //             script.enabled = state;
    //     }
    // }
}