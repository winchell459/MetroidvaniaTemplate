using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platformer : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 4;
    public float jumpForce = 6;

    bool isGrounded = false;
    public Transform[] isGroundedChecker;
    private Transform isGroundedCheckerLeft, isGroundedCheckerRight;
    private Vector2 groundCheckerLeftPos, groundCheckerRightPos;
    public float checkGroundRadius = 0.05f;
    public LayerMask groundLayer;

    //Wall checkers and parameters
    public Transform[] isWallCheckerLeft, isWallCheckerRight;
    bool isWalledLeft = false;
    bool isWalledRight = false;

    //late jump parameters
    public float rememberGroundedFor = 0.1f;
    float lastTimeGrounded;

    //in air gravity multipliers
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    //Multi Jump Counters
    public int defaultAdditionalJumps = 1;
    int additionalJumps;

    //Debugging Ghost Parameters
    public GameObject SquareGhostPrefab;
    public bool showJumpGhost;
    public bool newJumpGhost;
    GameObject squareJumpGhost;

    //Dash Parameters
    public float DashGapTime = 0.5f;
    //float dashDirReleaseTime = Mathf.NegativeInfinity;
    int dashingDir;
    float dashDownTime = 0.2f;
    float dashBeginTime = Mathf.NegativeInfinity;
    public float DashCooldownTime = 2f;
    float dashCoolDownBegin = Mathf.NegativeInfinity;
    public float DashDuration = 2f;
    public float DashMultiplier = 2;
    public float DashSpeed {
        get
        {

            return speed * DashMultiplier;
        }
    }
    bool isDashing;
    

    //Standard Input variables
    float moveX;
    bool XButton;
    bool XButtonDown;
    bool XButtonUp;
    bool jumpButton;
    bool jumpButtonDown;

    public Abilities PlayerAbilities;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        SetupGroundCheckers();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    void FixedUpdate()
    {
        HandleInput();
        AdjustGrounded(isGrounded);
        Dash();
        Move();
        WallJump();
        Jump();
        BetterJump();
        CheckIfGrounded();
        CheckForWall();
    }

    void SetupGroundCheckers()
    {
        isGroundedCheckerLeft = isGroundedChecker[0];
        isGroundedCheckerRight = isGroundedChecker[isGroundedChecker.Length - 1];
        groundCheckerLeftPos = isGroundedCheckerLeft.localPosition;
        groundCheckerRightPos = isGroundedCheckerRight.localPosition;
    }

    float lastXDownTime;
    float lastXUpTime;
    float lastXDownDuration;
    float lastXUpDuration;

    void HandleInput()
    {

        moveX = Input.GetAxisRaw("Horizontal");
        if(Mathf.Abs(moveX) > 0.1f)
        {
            if (!XButton)
            {
                XButtonDown = true;
                lastXDownTime = Time.time;
                lastXUpDuration = lastXDownTime - lastXUpTime;
            }
            else XButtonDown = false;
            XButton = true;
            XButtonUp = false;
        }
        else
        {
            if (XButton)
            {
                XButtonUp = true;
                lastXUpTime = Time.time;
                lastXDownDuration = lastXUpTime - lastXDownTime;
            }
            else XButtonUp = false;
            XButton = false;
            XButtonDown = false;
            
        }
        //jumpButton = Input.GetKey(KeyCode.Space);
        //jumpButtonDown = Input.GetKeyDown(KeyCode.Space);
        if (Input.GetKey(KeyCode.Space) || Input.GetAxisRaw("Jump") > 0)
        {
            if (!jumpButton) jumpButtonDown = true;
            else jumpButtonDown = false;
            jumpButton = true;
        }
        else
        {
            jumpButton = jumpButtonDown = false;
        }
    }
    void Move()
    {
        float x = moveX;//Input.GetAxisRaw("Horizontal");
        //float moveBy = x * speed;
        float moveBy = isDashing && PlayerAbilities.Dash ? x * DashSpeed : x * speed;

        //breaks horizontal motion if touching a wall in direction of horizontal motion
        if (moveBy < 0 && isWalledLeft) moveBy = 0;
        else if (moveBy > 0 && isWalledRight) moveBy = 0;


        if (!isWallJumping) rb.velocity = new Vector2(moveBy, rb.velocity.y);
        else
        {
            float movePercent = (Time.time - wallJumpStart) / wallJumpDuration;
            float movingWeight = Mathf.Abs(moveX) > 0? 1: 0;
            movingWeight += Mathf.Sign(moveX) != Mathf.Sign(rb.velocity.x) ? 1 : 0;
            Debug.Log(Mathf.Sign(rb.velocity.x));

            //zero out velocity if direction held for entire jump
            //rb.velocity = new Vector2(rb.velocity.x - movingWeight * wallJumpDirection * Time.deltaTime * speed / wallJumpDuration, rb.velocity.y);

            //rb.velocity = new Vector2(rb.velocity.x - movingWeight * wallJumpDirection * Time.deltaTime * /*wallJumpForce*/ moveBy/wallJumpDuration, rb.velocity.y);
            float dV = Time.deltaTime * (rb.velocity.x - moveBy) / (wallJumpStart + wallJumpDuration - Time.time) ;
            rb.velocity = new Vector2(rb.velocity.x - dV, rb.velocity.y);
        }
    }

    void Dash()
    {
        if (isDashing)
        {
            if(!XButton  || dashBeginTime + DashDuration < Time.time)
            {
                isDashing = false;
                dashCoolDownBegin = Time.time;
            }
        }
        else if(dashCoolDownBegin + DashCooldownTime < Time.time)
        {
            int moveDir = 0;
            if (moveX > 0) moveDir = 1;
            else if (moveX < 0) moveDir = -1;
            if(XButtonDown && moveDir == dashingDir && DashGapTime + lastXUpTime /*dashDirReleaseTime*/ > Time.time && lastXDownDuration <= dashDownTime)
            {
                isDashing = true;
                dashBeginTime = Time.time;

            }else if(XButton || XButtonDown)
            {
                dashingDir = moveDir;
            }
            //else if(XButtonUp)
            //{
            //    dashDirReleaseTime = Time.time;
            //}
        }
    }
    Vector2 WallJumpNormal = new Vector2(1,1);
    int wallJumpDirection = 0;
    public float wallJumpForce = 60;
    public float wallJumpDuration = 1;
    float wallJumpStart;
    bool isWallJumping { get { return wallJumpStart + wallJumpDuration > Time.time; } set { wallJumpStart = value ?  Time.time: Mathf.NegativeInfinity; } }
    void WallJump()
    {
        if(isWalledLeft && !isWalledRight && !isGrounded)
        {
            if (jumpButtonDown)
            {
                rb.velocity = /*new Vector2(rb.velocity.x, jumpForce); */ wallJumpForce * WallJumpNormal;
                wallJumpStart = Time.time;
                wallJumpDirection = 1;
            }
        }else if(!isWalledLeft && isWalledRight && !isGrounded)
        {
            if (jumpButtonDown)
            {
                rb.velocity = new Vector2(-wallJumpForce * WallJumpNormal.x, wallJumpForce * WallJumpNormal.y);
                wallJumpStart = Time.time;
                wallJumpDirection = -1;
            }
        }
    }
    void Jump()
    {
        if (showJumpGhost && /*Input.GetKeyDown(KeyCode.Space)*/ jumpButtonDown)
        {
            if (!squareJumpGhost || newJumpGhost)
            {
                squareJumpGhost = Instantiate(SquareGhostPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                squareJumpGhost.transform.position = transform.position;
            }
            AdjustGrounded(false);
        }

        if (/*Input.GetKeyDown(KeyCode.Space)*/ jumpButtonDown && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor || (PlayerAbilities.DoubleJump && additionalJumps > 0)) )
        {
            
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            if(!isGrounded) additionalJumps -= 1;
        }
    }
    void BetterJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !/*Input.GetKey(KeyCode.Space)*/jumpButton)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    //allows for extended jumps on ledges and prevents isGrounded from being triggered by a wall
    void AdjustGrounded(bool grounded)
    {
        if (grounded)
        {
            isGroundedCheckerLeft.localPosition = groundCheckerLeftPos;
            isGroundedCheckerRight.localPosition = groundCheckerRightPos;
        }
        else
        {
            Vector2 offset = new Vector2(0.05f, 0);
            isGroundedCheckerLeft.localPosition = groundCheckerLeftPos + offset;
            isGroundedCheckerRight.localPosition = groundCheckerRightPos - offset;
        }
    }

    void CheckIfGrounded()
    { 
        isGrounded = CheckForCollision(isGroundedChecker);
        if (isGrounded) additionalJumps = defaultAdditionalJumps;
    }

    void CheckForWall()
    {
        isWalledLeft = CheckForCollision(isWallCheckerLeft);
        isWalledRight = CheckForCollision(isWallCheckerRight);
    }
    bool CheckForCollision(Vector2 checkerPos)
    {
        Collider2D collider = Physics2D.OverlapCircle(checkerPos, checkGroundRadius, groundLayer);
        if (collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckForCollision(Transform checker)
    {
        //Collider2D collider = Physics2D.OverlapCircle(checker.position, checkGroundRadius, groundLayer);
        if (CheckForCollision(checker.position))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckForCollision(Transform[] checkers)
    {
        foreach(Transform checker in checkers)
        {
            if (CheckForCollision(checker))
            {
                return true;
            }
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
