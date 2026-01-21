using UnityEngine;
using Pathfinding;

public class EnemyAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private AIPath aiPath;

    [SerializeField] private Transform target;
    [SerializeField] private float attackRange = 1.2f;

    private Vector2 lastMoveDir = Vector2.down;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        aiPath = GetComponent<AIPath>();
    }

    void Update()
    {
        UpdateMovementAnimation();
        UpdateAttackState();
    }

    void UpdateMovementAnimation()
    {
        Vector2 velocity = Vector2.zero;

        if (aiPath != null && aiPath.enabled)
            velocity = aiPath.velocity;
        else if (rb != null)
            velocity = rb.velocity;

        float speed = velocity.magnitude;
        anim.SetFloat("Speed", speed);

        if (speed > 0.01f)
        {
            Vector2 dir = velocity.normalized;
            lastMoveDir = GetCardinalDirection(dir);

            anim.SetFloat("MoveX", lastMoveDir.x);
            anim.SetFloat("MoveY", lastMoveDir.y);

            HandleFlip(lastMoveDir.x);
        }
        else
        {
            anim.SetFloat("MoveX", lastMoveDir.x);
            anim.SetFloat("MoveY", lastMoveDir.y);
        }
    }

    void UpdateAttackState()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);
        bool isAttacking = distance <= attackRange;

        anim.SetBool("IsAttacking", isAttacking);
    }

    Vector2 GetCardinalDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return new Vector2(Mathf.Sign(dir.x), 0);
        else
            return new Vector2(0, Mathf.Sign(dir.y));
    }

    void HandleFlip(float x)
    {
        if (x > 0.01f)
            sprite.flipX = true;
        else if (x < -0.01f)
            sprite.flipX = false;
    }
}
