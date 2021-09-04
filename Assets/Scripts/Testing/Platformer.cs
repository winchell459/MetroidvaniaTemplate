using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platformer : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 4;
    public float jumpForce = 6;
    public float maxGravityForce = 10;

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
    public LayerMask wallLayer;

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

            return DashSpeedCalc();
        }
    }
    bool isDashing;
    

    //Standard Input variables
    //float moveX;
    //bool XButton;
    //bool XButtonDown;
    //bool XButtonUp;
    //bool jumpButton;
    //bool jumpButtonDown;
    //bool gravityFlipButtonDown;
    //bool gravityFlipButton;

    public Abilities PlayerAbilities;

    GravityObjectTheCube gravityObject;
    int gravityFlipped
    {
        get { return gravityObject.gravityFlipped; }
    }
    bool canFlipGravity { get { return isGrounded; } }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravityObject = GetComponent<GravityObjectTheCube>();
        SetupGroundCheckers();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    void FixedUpdate()
    {
        HandleInput();
        //Adjust the isGroundedChecker position if not grounded
        AdjustGrounded(isGrounded);

        Dash();
        Move();
        WallJump();
        Jump();
        BetterJump();
        if (gravityFlipped > 0 && rb.velocity.y < -maxGravityForce ) rb.velocity = new Vector2(rb.velocity.x, -maxGravityForce);
        else if (gravityFlipped < 0 && rb.velocity.y > maxGravityForce) rb.velocity = new Vector2(rb.velocity.x, maxGravityForce);
        CheckIfGrounded();
        CheckForWall();

        GravityFlip();
    }

    void SetupGroundCheckers()
    {
        isGroundedCheckerLeft = isGroundedChecker[0];
        isGroundedCheckerRight = isGroundedChecker[isGroundedChecker.Length - 1];
        groundCheckerLeftPos = isGroundedCheckerLeft.localPosition;
        groundCheckerRightPos = isGroundedCheckerRight.localPosition;
    }

    //Player Input Stats
    //float lastXDownTime;
    //float lastXUpTime;
    //float lastXDownDuration;
    //float lastXUpDuration;
    //float lastJumpDownTime;
    //float lastJumpUpTime;
    //float lastJumpDownDuration;
    //float lastJumpUpDuration;

    ButtonInput<float> btnX = new ButtonInput<float>(0.1f);
    ButtonInput<bool> btnJump = new ButtonInput<bool>(0.1f);
    ButtonInput<bool> btnGravityFlip = new ButtonInput<bool>(0.1f);

    void SetupInput()
    {

    }

    void HandleInput()
    {
        btnX.Update(Input.GetAxisRaw("Horizontal"));
        btnJump.Update(Input.GetKey(KeyCode.Space) || Input.GetAxisRaw("Jump") > 0);
        btnGravityFlip.Update(Input.GetKey(KeyCode.G));
        //moveX = Input.GetAxisRaw("Horizontal");
        //if(Mathf.Abs(moveX) > 0.1f)
        //{
        //    if (!XButton)
        //    {
        //        XButtonDown = true;
        //        lastXDownTime = Time.time;
        //        lastXUpDuration = lastXDownTime - lastXUpTime;
        //    }
        //    else XButtonDown = false;
        //    XButton = true;
        //    XButtonUp = false;
        //}
        //else
        //{
        //    if (XButton)
        //    {
        //        XButtonUp = true;
        //        lastXUpTime = Time.time;
        //        lastXDownDuration = lastXUpTime - lastXDownTime;
        //    }
        //    else XButtonUp = false;
        //    XButton = false;
        //    XButtonDown = false;
            
        //}
        ////jumpButton = Input.GetKey(KeyCode.Space);
        ////jumpButtonDown = Input.GetKeyDown(KeyCode.Space);
        //if (Input.GetKey(KeyCode.Space) || Input.GetAxisRaw("Jump") > 0)
        //{
        //    if (!jumpButton)
        //    {
        //        jumpButtonDown = true;
        //        lastJumpDownTime = Time.time;
        //        lastJumpUpDuration = Time.time - lastJumpUpTime;
        //    }
        //    else jumpButtonDown = false;
        //    jumpButton = true;
        //}
        //else
        //{
        //    if (jumpButton)
        //    {
        //        lastJumpUpTime = Time.time;
        //        lastJumpDownDuration = Time.time - lastJumpDownTime;
        //    }
        //    jumpButton = jumpButtonDown = false;
        //}

        //if (Input.GetKey(KeyCode.G))
        //{
        //    if (!gravityFlipButton) gravityFlipButtonDown = true;
        //    else gravityFlipButtonDown = false;
        //    gravityFlipButton = true;
        //}
        //else
        //{
        //    gravityFlipButton = false;
        //    gravityFlipButtonDown = false;
        //}
    }
    void Move()
    {
        float x = btnX.value;//moveX;//Input.GetAxisRaw("Horizontal");
        //float moveBy = x * speed;
        float moveBy = isDashing && PlayerAbilities.Dash ? x * DashSpeed : x * speed;
        float moveY = rb.velocity.y;

        //breaks horizontal motion if touching a wall in direction of horizontal motion
        if (moveBy < 0 && isWalledLeft) moveBy = 0;
        else if (moveBy > 0 && isWalledRight) moveBy = 0;


        //if (!isWallJumping && isGrounded && Mathf.Abs(x) < 0.1f) moveY = 0;// rb.velocity = new Vector2(moveBy, moveY);
        //else
        if (isWallJumping)
        {

            //Debug.Log(Mathf.Sign(rb.velocity.x));

            //zero out velocity if direction held for entire jump
            //float movingWeight = Mathf.Abs(moveX) > 0 ? 1 : 0;
            //movingWeight += Mathf.Sign(moveX) != Mathf.Sign(rb.velocity.x) ? 1 : 0;
            //rb.velocity = new Vector2(rb.velocity.x - movingWeight * wallJumpDirection * Time.deltaTime * speed / wallJumpDuration, rb.velocity.y);

            //rb.velocity = new Vector2(rb.velocity.x - movingWeight * wallJumpDirection * Time.deltaTime * /*wallJumpForce*/ moveBy/wallJumpDuration, rb.velocity.y);
            float dV = Time.deltaTime * (rb.velocity.x - moveBy) / (wallJumpStart + wallJumpDuration - Time.time);
            //rb.velocity = new Vector2(rb.velocity.x - dV, moveY);
            moveBy = rb.velocity.x - dV;
        }
        rb.velocity = new Vector2(moveBy, moveY);
    }
    float DashSpeedCalc()
    {
        //m = At + DashMultiplier
        //1 = A(DashDuration) + DashMultiplier
        //1 - DashMultiplier = A*DashDuration
        //A = (1 - DashMultiplier)/DashDuration
        //m = t*(1 - DashMultiplier)/DashDuration + DashMultiplier
      
        float multiplier = (Time.time - dashBeginTime) * (1 - DashMultiplier) / DashDuration + DashMultiplier;
        Debug.Log(multiplier);
        return speed * multiplier;
    }

    void Dash()
    {
        if (isDashing)
        {
            if(!btnX.hold  || dashBeginTime + DashDuration < Time.time)
            {
                isDashing = false;
                dashCoolDownBegin = Time.time;
            }
        }
        else if(dashCoolDownBegin + DashCooldownTime < Time.time && isGrounded)
        {
            int moveDir = 0;
            if (btnX.value > 0) moveDir = 1;
            else if (btnX.value < 0) moveDir = -1;
            if (btnX.down && moveDir == dashingDir && DashGapTime + btnX.upTime/*lastXUpTime /*dashDirReleaseTime*/ > Time.time && btnX.downDuration/*lastXDownDuration*/ <= dashDownTime)
            {
                isDashing = true;
                dashBeginTime = Time.time;

            }else if(btnX.hold || btnX.down)
            {
                dashingDir = moveDir;
            }
            //else if(XButtonUp)
            //{
            //    dashDirReleaseTime = Time.time;
            //}
        }
    }
    public Vector2 WallJumpNormal = new Vector2(1,1);
    int wallJumpDirection = 0;
    public float wallJumpForce = 60;
    public float wallJumpDuration = 1;
    float wallJumpStart;
    bool isWallJumping { get { return wallJumpStart + wallJumpDuration > Time.time; } set { wallJumpStart = value ?  Time.time: Mathf.NegativeInfinity; } }
    void WallJump()
    {
        if (PlayerAbilities.Climb)
        {
            if (isWalledLeft && !isWalledRight && !isGrounded)
            {
                if (btnJump.down)
                {
                    rb.velocity =  wallJumpForce * WallJumpNormal * new Vector2(1,gravityFlipped);
                    wallJumpStart = Time.time;
                    wallJumpDirection = 1;
                }
            }
            else if (!isWalledLeft && isWalledRight && !isGrounded)
            {
                if (btnJump.down)
                {
                    Debug.Log("WallJumped");
                    rb.velocity = new Vector2(-wallJumpForce * WallJumpNormal.x, wallJumpForce * WallJumpNormal.y) * new Vector2(1, gravityFlipped);
                    wallJumpStart = Time.time;
                    wallJumpDirection = -1;
                }
            }
        }
    }
    void Jump()
    {
        if (showJumpGhost && /*Input.GetKeyDown(KeyCode.Space)*/ btnJump.down)
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

        if (/*Input.GetKeyDown(KeyCode.Space)*/ btnJump.down && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor || (PlayerAbilities.DoubleJump && additionalJumps > 0)) )
        {
            
            rb.velocity = new Vector2(rb.velocity.x, gravityFlipped*jumpForce);
            if(!isGrounded) additionalJumps -= 1;
        }
    }
    void BetterJump()
    {
        if (gravityFlipped * rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (gravityFlipped * rb.velocity.y > 0 && !btnJump.hold)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    //allows for extended jumps on ledges and prevents isGrounded from being triggered by a wall
    void AdjustGrounded(bool grounded)
    {
        //Debug.Log("Adjusted(" + grounded + ")");
        Vector2 leftPos = groundCheckerLeftPos * new Vector2(1, gravityFlipped);
        Vector2 rightPos = groundCheckerRightPos * new Vector2(1, gravityFlipped);
        if (grounded)
        {
            isGroundedCheckerLeft.localPosition = leftPos; // groundCheckerLeftPos;
            isGroundedCheckerRight.localPosition = rightPos;// groundCheckerRightPos;
        }
        else
        {
            Vector2 offset = new Vector2(0.05f, 0);
            isGroundedCheckerLeft.localPosition = /*groundCheckerLeftPos*/ leftPos + offset;
            isGroundedCheckerRight.localPosition = /*groundCheckerRightPos*/ rightPos - offset;
        }
    }

    void GravityFlip()
    {
        if (PlayerAbilities.GravitySwitch && btnGravityFlip.down && canFlipGravity)
        {
            
            FindObjectOfType<WorldHandler>().FlipGravity();
        }
    }

    void CheckIfGrounded()
    { 
        isGrounded = CheckForCollision(isGroundedChecker, groundLayer);
        if(isGrounded) lastTimeGrounded = Time.time;
        if (isGrounded) additionalJumps = defaultAdditionalJumps;
    }

    void CheckForWall()
    {
        isWalledLeft = CheckForCollision(isWallCheckerLeft, wallLayer);
        isWalledRight = CheckForCollision(isWallCheckerRight, wallLayer);
    }
    bool CheckForCollision(Vector2 checkerPos, LayerMask layer)
    {
        Collider2D collider = Physics2D.OverlapCircle(checkerPos, checkGroundRadius, layer);
        
        if (collider != null)
        {
            
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckForCollision(Transform checker, LayerMask layer)
    {
        //Collider2D collider = Physics2D.OverlapCircle(checker.position, checkGroundRadius, groundLayer);
        if (CheckForCollision(checker.position, layer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckForCollision(Transform[] checkers, LayerMask layer)
    {
        foreach(Transform checker in checkers)
        {
            if (CheckForCollision(checker, layer))
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

