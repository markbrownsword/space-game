using System.Collections.Generic;
using UnityEngine;

public abstract class PhysicsObject : MonoBehaviour
{
    public float minGroundNormalY = .65f;
    public float gravityModifier = 1.5f;
    public float jumpVelocityModifier = 0.5f;

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;
    protected Vector2 targetVelocity;
    protected bool grounded;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D> (16);
    protected enum Facing { Left, Right }
    protected abstract void ComputeVelocity();

    // Protected Methods

    protected bool CheckCameraBounds(Camera camera, Transform transform, SpriteRenderer spriteRenderer, Facing facing)
    {
        var cameraPosition = camera.transform.position;
        var xDist = camera.aspect * camera.orthographicSize;
        var xMax = cameraPosition.x + xDist;
        var xMin = cameraPosition.x - xDist;

        var position = transform.position;
        var offset = spriteRenderer.size.x / 2; // Make the offset half the width of the sprite

        if (facing == Facing.Left)
        {
            return position.x - offset < xMin;
        }

        // Facing Right
        return position.x + offset > xMax;
    }

    // Unity Methods

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask (Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }
    
    void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity(); 
    }

    void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;

        // Determine velocity x when on the ground or in the air
        velocity.x = grounded ? targetVelocity.x : targetVelocity.x * jumpVelocityModifier;

        grounded = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        // Move X
        Vector2 move = moveAlongGround * deltaPosition.x;
        Movement(move, false);

        // Move Y
        move = Vector2.up * deltaPosition.y;
        Movement(move, true);
    }

    // Private Methods

    private void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance) 
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i < count; i++) {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++) 
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY) 
                {
                    grounded = true;
                    if (yMovement) 
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0) 
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }
}
