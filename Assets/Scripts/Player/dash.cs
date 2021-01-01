using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dash : ability
{
    [SerializeField]
    Binding binding;

    move controller;

    public float dashCooldownTimer = 0;

    [SerializeField]
    float dashCooldownTimeMax = 1;

    [SerializeField]
    float dashSpeed;

    float dashTimer;

    [SerializeField]
    float dashDuration;

    Rigidbody2D rb;

    Vector2 dashDir = Vector2.zero;

    public bool dashed = true;

    public bool CanDash = false;

    void Start()
    {
        controller = GetComponent<move>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        dashTimer -= Time.deltaTime;
        if (controller.timeSinceGrounded > 0)
        {
            CanDash = true;
        }
        if (Input.GetKeyDown(binding.dash) && CanDash)
        {
            if (dashed)
            {
                CanDash = false;
                dashTimer = dashDuration;
                dashDir = Vector2.zero;
                dashDir.x = controller.dir;
                if (Input.GetKey(binding.up))
                {
                    dashDir.y = 0.5f;
                    dashDir.x *= 0.7f;
                }
                if (Input.GetKey(binding.down))
                {
                    dashDir.x *= 0.7f;
                    dashDir.y = -0.5f;
                }
                dashed = true;
            }
        }
        if (dashTimer > 0)
        {
            controller.useGravity = false;
            rb.velocity = dashDir * dashSpeed;
        }
        else
        {
            controller.useGravity = true;
        }
    }
}
