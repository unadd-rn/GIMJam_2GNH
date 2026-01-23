using UnityEngine;

public class PermanentPressurePlate : MonoBehaviour
{
    [Header("References")]
    public GameObject door;
    public Sprite pressedSprite;  

    [Header("Settings")]
    public float doorLowerAmount = 2.0f;
    public float moveSpeed = 5.0f;
    public float colliderShrinkRatio = 0.6f;

    private Vector2 doorClosedPos, doorOpenPos;
    private bool isActivated = false;
    
    private SpriteRenderer sr;
    private BoxCollider2D col;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        
        doorClosedPos = door.transform.position;
        doorOpenPos = doorClosedPos + (Vector2.down * doorLowerAmount);
    }

    void Update()
    {
        if (isActivated)
        {
            door.transform.position = Vector2.Lerp(door.transform.position, doorOpenPos, Time.deltaTime * moveSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActivated && collision.gameObject.CompareTag("Player"))
        {
            ActivatePlate();
        }
    }

    void ActivatePlate()
    {
        isActivated = true;
        
        // image
        if (pressedSprite != null) sr.sprite = pressedSprite;
        
        // shrink
        col.size = new Vector2(col.size.x, col.size.y * colliderShrinkRatio);
        col.offset = new Vector2(col.offset.x, col.offset.y - (col.size.y * 0.1f));
    }
}