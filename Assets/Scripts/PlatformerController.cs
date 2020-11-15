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

    public Abilities PlayerAbilities;

    //Dash Parameters
    public float DashGapTime = 0.25f;
    int dashingDir;
    float dashDownTime = 0.2f;
    float dashBeginTime = Mathf.NegativeInfinity;
    public float DashCoolDownTime = 1;
    float dashCoolDownBegin = Mathf.NegativeInfinity;
    public float DashDuration = 1;
    public float DashMultiplier = 3;
    bool isDashing;
    public float DashSpeed { get { return DashSpeedCalc(); } }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        Dash();
        Move();
        WallJump();
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
            isWallJumpGrounded = true;
        }
        
    }

    float DashSpeedCalc()
    {
        float multiplier = (1 - DashMultiplier) * (Time.time - dashBeginTime) / DashDuration + DashMultiplier;
        return speed * multiplier;
    }

    void Dash()
    {

    }

    public Vector2 WallJumpNormal = new Vector2(1, 1.25f);
    public float WallJumpForce = 8;
    public float WallJumpDuration = 0.5f;
    private float wallJumpStart;
    bool isWallJumping { get { return Time.time < wallJumpStart + WallJumpDuration; } }
    bool isWallJumpGrounded;
    void WallJump()
    {

        if (PlayerAbilities.Climb)
        {
            //walled on left
            if(isWalledLeft && !isWalledRight && !isGrounded && isWallJumpGrounded)
            {
                if (btnJump.down)
                {
                    Debug.Log("Wall Jumped Left");
                    rb.velocity = WallJumpForce * WallJumpNormal; //for gravity flipping we need to modify 
                    wallJumpStart = Time.time;
                    isWallJumpGrounded = false;
                }
            }
            //walled on right
            else if (isWalledRight && !isWalledLeft && !isGrounded && isWallJumpGrounded)
            {
                if (btnJump.down)
                {
                    rb.velocity = new Vector2(-WallJumpForce * WallJumpNormal.x, WallJumpForce * WallJumpNormal.y);
                    wallJumpStart = Time.time;
                    isWallJumpGrounded = false;
                }
            }
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

        if (isWallJumping)
        {
            //dV increases as time remaining decreases
            float dV = Time.deltaTime * (rb.velocity.x - moveX) / (wallJumpStart + WallJumpDuration - Time.time);
            moveX = rb.velocity.x - dV;
        }


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

[System.Serializable]
public class Abilities
{
    public bool DoubleJump;
    public bool Dash;
    public bool Hover;
    public bool Fly;
    public bool StopWatch;
    public bool Climb;
    public bool GravitySwitch;
}