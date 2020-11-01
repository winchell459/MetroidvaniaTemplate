using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerController : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float speed = 6;
    [SerializeField] float jumpSpeed = 6;

    //coyote time jump variables
    [SerializeField] float rememberGroundedFor = 0.1f;
    float lastTimeGrounded;

    //in air gravity multipiers
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;

    [SerializeField] LayerMask groundMask, wallMask;
    [SerializeField] Transform[] isGroundedCheckers, isWallCheckerLeft, isWallCheckerRight;

    bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        Move();
        Jump();
        BetterJump();
        CheckIfGrounded();
    }

    private void CheckIfGrounded()
    {
        isGrounded = CheckForCollision(isGroundedCheckers, groundMask);
    }

    void Jump()
    {
        if (jumpButtonDown && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }

    void BetterJump()
    {
        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }else if(rb.velocity.y > 0 && !jumpButton)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void Move()
    {
        float moveBy = moveX * speed;
        rb.velocity = new Vector2(moveBy, rb.velocity.y);
    }

    float moveX;
    bool xButton;
    bool xButtonDown;
    bool xButtonUp;
    bool jumpButton;
    bool jumpButtonDown;
    bool jumpButtonUp;

    void HandleInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        if(Mathf.Abs(moveX) > 0.1f) // "x" button is pressed
        {
            if (!xButton)
            {
                xButtonDown = true;
            }
            else
            {
                xButtonDown = false;
            }
            xButton = true;
            xButtonUp = false;
        }
        else
        {
            if (xButton)
            {
                xButtonUp = true;
            }
            else
            {
                xButtonUp = false;
            }
            xButton = false;
            xButtonDown = false;
        }

        //check for jump buttons
        if (Input.GetKey(KeyCode.Space) || Input.GetAxisRaw("Jump") > 0.1f)
        {
            if (!jumpButton)
            {
                jumpButtonDown = true;
            }
            else
            {
                jumpButtonDown = false;
            }
            jumpButton = true;
            jumpButtonUp = false;
        }
        else
        {
            if (jumpButton)
            {
                jumpButtonUp = true;
            }
            else
            {
                jumpButtonUp = false;
            }
            jumpButton = false;
            jumpButtonDown = false;
        }
    }

    float checkGroundRadius = 0.05f;
    bool CheckForCollision(Vector2 checkPos, LayerMask layer)
    {
        if (Physics2D.OverlapCircle(checkPos, checkGroundRadius, layer)) return true;
        else return false;
    }
    bool CheckForCollision(Transform checker, LayerMask layer)
    {
        if (CheckForCollision(checker.position, layer)) return true;
        else return false;
    }
    bool CheckForCollision(Transform[] checkers, LayerMask layer)
    {
        foreach(Transform checker in checkers)
        {
            if (CheckForCollision(checker, layer)) return true;
        }
        return false;
    }
}
