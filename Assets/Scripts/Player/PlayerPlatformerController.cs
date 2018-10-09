using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        // Determine if the player is facing right or left
        var facing = spriteRenderer.flipX ? Facing.Left : Facing.Right;

        // Determine if the player is moving right or left
        if (move.x > 0.01f) // Moving Right
        {
            if (spriteRenderer.flipX == true)
            {
                spriteRenderer.flipX = false;
                facing = Facing.Right;
            }
        }
        else if (move.x < -0.01f) // Moving Left
        {
            if (spriteRenderer.flipX == false)
            {
                spriteRenderer.flipX = true;
                facing = Facing.Left;
            }
        }

        // Determine if the player has reached edge of screen
        if (CheckCameraBounds(Camera.main, transform, spriteRenderer, facing))
        {
            targetVelocity = Vector2.zero;
            animator.SetBool("IsIdle", true);
        }
        else
        {
            targetVelocity = move * maxSpeed;
            animator.SetBool("IsIdle", grounded && Mathf.Abs(velocity.x) == 0);
        }
    }
}
