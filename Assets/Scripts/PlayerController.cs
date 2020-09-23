using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5;
    public float Jump = 10;
    public float MaxJumpSpeed = 5;

    private Rigidbody2D rb;
    private Animator anim;

    public float raycastLength = 0.5f;
    public LayerMask groundMask;
    public bool GroundCheck;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        GroundCheck = groundCheck();
        float j = handleJumping();
        
        
        rb.velocity = new Vector2(h * Speed, rb.velocity.y + j * Jump);

        anim.SetFloat("Velocity", Mathf.Abs(rb.velocity.y));
        anim.SetFloat("VelocityH", Mathf.Abs(rb.velocity.x));

        if (h > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (h < 0) transform.localScale = new Vector3(-1, 1, 1);

        //for Nathan
        //anim.SetFloat("Velocity", rb.velocity.magnitude);
    }
    bool jumping;
    private float handleJumping()
    {
        int j = Input.GetKey(KeyCode.Space)? 1:0;
        if (rb.velocity.y > MaxJumpSpeed) {
            j = 0;
            jumping = false;
        }
        if (GroundCheck && j > 0) {
            jumping = true;
            return j;
        }
        else if (j > 0 && jumping)
        {
            return j;
        }
        else
        {
            jumping = false;
            return 0;
        }
    }

    private bool groundCheck()
    {
        
        bool grounded = false;
        //RaycastHit2D hit;
        //Ray2D ray = new Ray2D(transform.position, -transform.up * raycastLength);
        if(Physics2D.Raycast(transform.position, -transform.up, raycastLength, groundMask))
        {
            grounded = true;
        }
        return grounded;
    }
}
