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

    bool isGrounded, isWalledLeft, isWalledRight;


    ButtonInput<float> btnX = new ButtonInput<float>(0.1f);
    ButtonInput<bool> btnJump = new ButtonInput<bool>(0);
    ButtonInput<bool> btnGravityFlip = new ButtonInput<bool>(0);

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
        CheckForWalls();
    }

    void CheckForWalls()
    {
        isWalledLeft = CheckForCollision(isWallCheckerLeft, wallMask);
        isWalledRight = CheckForCollision(isWallCheckerRight, wallMask);
    }
    private void CheckIfGrounded()
    {
        isGrounded = CheckForCollision(isGroundedCheckers, groundMask);
        if (isGrounded)
        {
            lastTimeGrounded = Time.time;
            
        }
        
    }

    void Jump()
    {
        if (btnJump.down && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }

    void BetterJump()
    {
        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }else if(rb.velocity.y > 0 && !btnJump.hold)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void Move()
    {
        float moveX = btnX.value * speed;
        float moveY = rb.velocity.y;

        if (moveX > 0 && isWalledRight) moveX = 0;
        else if (moveX < 0 && isWalledLeft) moveX = 0;


        rb.velocity = new Vector2(moveX, moveY);
    }


    void HandleInput()
    {
        btnX.UpdateButton(Input.GetAxisRaw("Horizontal"));
        btnJump.UpdateButton(Input.GetKey(KeyCode.Space) || Input.GetAxisRaw("Jump") > 0.1f);
        btnGravityFlip.UpdateButton(Input.GetKey(KeyCode.G));
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
