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

    public Transform[] isWallCheckerLeft, isWallCheckerRight;
    bool isWalledLeft = false;
    bool isWalledRight = false;

    public float rememberGroundedFor = 0.1f;
    float lastTimeGrounded;

    public GameObject SquareGhostPrefab;
    public bool showJumpGhost;
    public bool newJumpGhost;
    GameObject squareJumpGhost;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public int defaultAdditionalJumps = 1;
    int additionalJumps;

    float moveX;
    bool jumpButton;
    bool jumpButtonDown;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isGroundedCheckerLeft = isGroundedChecker[0];
        isGroundedCheckerRight = isGroundedChecker[isGroundedChecker.Length - 1];
        groundCheckerLeftPos = isGroundedCheckerLeft.localPosition;
        groundCheckerRightPos = isGroundedCheckerRight.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    void FixedUpdate()
    {
        HandleInput();
        AdjustGrounded(isGrounded);
        Move();
        Jump();
        BetterJump();
        CheckIfGrounded();
        CheckForWall();
    }
    void HandleInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        //jumpButton = Input.GetKey(KeyCode.Space);
        //jumpButtonDown = Input.GetKeyDown(KeyCode.Space);
        if (Input.GetKey(KeyCode.Space))
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
        float moveBy = x * speed;

        //breaks horizontal motion if touching a wall in direction of horizontal motion
        if (moveBy < 0 && isWalledLeft) moveBy = 0;
        else if (moveBy > 0 && isWalledRight) moveBy = 0;


        rb.velocity = new Vector2(moveBy, rb.velocity.y);
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

        if (/*Input.GetKeyDown(KeyCode.Space)*/ jumpButtonDown && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor || additionalJumps > 0) )
        {
            
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            additionalJumps -= 1;
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
            Vector2 offset = new Vector2(0.1f, 0);
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
