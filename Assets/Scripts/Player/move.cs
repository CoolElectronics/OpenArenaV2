using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    [SerializeField]
    LayerMask lmWalls;

    [SerializeField]
    float fJumpVelocity = 5;

    Rigidbody2D rb;

    float timeSinceJumpPress = 0;

    [SerializeField]
    float fJumpPressedRememberTime = 0.2f;

    public float timeSinceGrounded = 0;

    [SerializeField]
    float fGroundedRememberTime = 0.25f;

    [SerializeField]
    float fHorizontalAcceleration = 1;

    [SerializeField]
    [Range(0, 1)]
    float damping = 0.5f;

    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenStopping = 0.5f;

    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenTurning = 0.5f;

    [SerializeField]
    [Range(0, 1)]
    float fCutJumpHeight = 0.5f;

    [SerializeField]
    float speed;

    [SerializeField]
    float climbspeed;

    public float gravity;

    [SerializeField]
    Binding binding;

    [SerializeField]
    float wallSlideSpeed;

    [SerializeField]
    float restickThreshold = 0.3f;

    float restickTimer;

    public int dir;

    public bool isWallStick = false;

    public LayerMask lmWallStick;

    [SerializeField]
    float wallJumpColliderLen = 6.4f;

    [SerializeField]
    float wallJumpColliderHeight = 6.4f;

    [SerializeField]
    Vector2 wallJumpColliderOffset;

    [SerializeField]
    float wallKickSpeed;

    [SerializeField]
    SpriteRenderer sprite;

    public float timeSinceNoXvJump = 0;

    float maxTimeSinceNoXvJump = 0.4f;

    public float timeSinceWallStuck = 0;

    float maxTimeSinceWallStuck = 0.4f;

    public bool lastWallStuckStatus = false;

    float stunnedTime = 0;

    bool impulse = true;

    public bool canDblJump = true;

    public bool useGravity = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = 0;
        if (Input.GetKey(binding.left))
        {
            horizontal--;
        }
        if (Input.GetKey(binding.right))
        {
            horizontal++;
        }
        Vector2 GBoxColliderPos =
            (Vector2) transform.position + new Vector2(0, -0.01f - 0.25f);
        Vector2 GBoxColliderScale =
            (Vector2) transform.localScale + new Vector2(-0.02f, -0.5f);
        bool isGrounded =
            Physics2D
                .OverlapBox(GBoxColliderPos, GBoxColliderScale, 0, lmWalls);
        isWallStick =
            Physics2D
                .OverlapBox(transform.position +
                (Vector3) wallJumpColliderOffset,
                new Vector2(wallJumpColliderLen, wallJumpColliderHeight),
                0,
                lmWallStick);
        timeSinceGrounded -= Time.deltaTime;
        restickTimer -= Time.deltaTime;
        stunnedTime -= Time.deltaTime;
        if (restickTimer > 0)
        {
            isWallStick = false;
        }
        if (isWallStick)
        {
            isGrounded = true;
            rb.gravityScale = 0;
            if (raycastInDir().normal.x > 0.1)
            {
                if (horizontal < -0.3)
                {
                    horizontal = 0;
                }
            }
            else if (raycastInDir().normal.x < -0.1)
            {
                if (horizontal > 0.3)
                {
                    horizontal = 0;
                }
            }
            rb.velocity = new Vector2(rb.velocity.x, wallSlideSpeed);
            if (impulse)
            {
                impulse = false;
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            impulse = true;
            if (useGravity)
            {
                rb.gravityScale = gravity;
            }
        }
        if (isGrounded)
        {
            timeSinceGrounded = fGroundedRememberTime;
            canDblJump = true;
        }
        timeSinceWallStuck -= Time.deltaTime;
        timeSinceNoXvJump -= Time.deltaTime;

        // if (timeSinceNoXvJump > 0 && timeSinceNoXvJump < maxTimeSinceNoXvJump - Time.deltaTime * 8)
        // {
        //     if (horizontal > 0.3f || horizontal < -0.3f && timeSinceWallStuck > 0)
        //     {
        //         rb.velocity = new Vector2(rb.velocity.x, fJumpVelocity);
        //         timeSinceNoXvJump = 0;
        //     }
        // }
        timeSinceJumpPress -= Time.deltaTime;
        if (Input.GetKeyDown(binding.jump))
        {
            timeSinceJumpPress = fJumpPressedRememberTime;
        }

        if (Input.GetKeyUp(binding.jump))
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity =
                    new Vector2(rb.velocity.x, rb.velocity.y * fCutJumpHeight);
            }
        }
        if (horizontal > 0.3f)
        {
            dir = 1;
            sprite.flipX = false;
        }
        if (horizontal < -0.3f)
        {
            dir = -1;
            sprite.flipX = true;
        }
        if (timeSinceJumpPress > 0)
        {
            if (timeSinceGrounded > 0)
            {
                timeSinceJumpPress = 0;
                timeSinceGrounded = 0;
                if (!isWallStick)
                {
                    rb.velocity = new Vector2(rb.velocity.x, fJumpVelocity);
                }
                else
                {
                    RaycastHit2D hit = raycastInDir();
                    if (hit.collider != null)
                    {
                        timeSinceNoXvJump = maxTimeSinceNoXvJump;
                        rb.velocity =
                            new Vector2(-hit.normal.x * wallKickSpeed,
                                fJumpVelocity);
                        horizontal = -hit.normal.x;
                    }
                    isWallStick = false;
                    restickTimer = restickThreshold;
                }
            }
            else if (canDblJump)
            {
                canDblJump = false;
                DoubleJumpDelegate();
                rb.velocity = new Vector2(rb.velocity.x, fJumpVelocity * 1.5f);
            }
        }

        //
        if (restickTimer < 0 && stunnedTime < 0)
        {
            float fHorizontalVelocity = rb.velocity.x / speed;
            fHorizontalVelocity += horizontal * Time.deltaTime * 40;
            if (Mathf.Abs(horizontal) < 0.01f)
                fHorizontalVelocity *=
                    Mathf
                        .Pow(1f - fHorizontalDampingWhenStopping,
                        Time.deltaTime * 10f);
            else if (Mathf.Sign(horizontal) != Mathf.Sign(fHorizontalVelocity))
            {
                fHorizontalVelocity *=
                    Mathf
                        .Pow(1f - fHorizontalDampingWhenTurning,
                        Time.deltaTime * 10f);
            }
            else
                fHorizontalVelocity *=
                    Mathf.Pow(1f - damping, Time.deltaTime * 10f);
            rb.velocity =
                new Vector2(fHorizontalVelocity * speed, rb.velocity.y);
        }
        if (isWallStick != lastWallStuckStatus)
        {
            lastWallStuckStatus = isWallStick;
            if (isWallStick)
            {
                timeSinceWallStuck = maxTimeSinceWallStuck;
            }
        }
    }

    public void SetForce(Vector2 force, float time)
    {
        stunnedTime = time;
        rb.velocity = force;
    }

    public void SetForceX(float force, float time)
    {
        stunnedTime = time;
        rb.velocity = new Vector2(force, rb.velocity.y);
    }

    public void SetForceY(float force, float time)
    {
        stunnedTime = time;
        rb.velocity = new Vector2(rb.velocity.x, force);
    }

    RaycastHit2D raycastInDir()
    {
        RaycastHit2D hit1 =
            Physics2D
                .Raycast(transform.position,
                new Vector2(1, 0),
                (wallJumpColliderLen + 0.9f) / 2,
                lmWallStick);
        Debug
            .DrawRay(transform.position,
            new Vector3(1, 0, 0) * (wallJumpColliderLen + 0.9f) / 2,
            Color.red);
        RaycastHit2D hit2 =
            Physics2D
                .Raycast(transform.position,
                new Vector2(-1, 0),
                (wallJumpColliderLen + 0.9f) / 2,
                lmWallStick);
        Debug
            .DrawRay(transform.position,
            new Vector3(-1, 0, 0) * (wallJumpColliderLen + 0.9f) / 2,
            Color.red);
        if (hit1.collider != null)
        {
            return hit1;
        }
        else if (hit2.collider != null)
        {
            return hit2;
        }
        else
        {
            return hit1;
        }
    }

    private void OnDrawGizmos()
    {
        // Gizmos.matrix = Matrix4x4.TRS(transform.position + (Vector3)wallJumpColliderOffset, Quaternion.identity, new Vector3(wallJumpColliderLen, wallJumpColliderHeight, 0));
        // Gizmos.color = Color.red;
        // Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }

    public DoubleJump DoubleJumpDelegate;

    public delegate void DoubleJump();

    void DoubleJumpEvent()
    {
    }
}
