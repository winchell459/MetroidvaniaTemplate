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

        float j = Input.GetKey(KeyCode.Space) && rb.velocity.y < MaxJumpSpeed ? 1 : 0;

        rb.velocity = new Vector2(h * Speed, rb.velocity.y + j * Jump);

        anim.SetFloat("Velocity", Mathf.Abs(rb.velocity.y));
        anim.SetFloat("VelocityH", Mathf.Abs(rb.velocity.x));

        if (h > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (h < 0) transform.localScale = new Vector3(-1, 1, 1);

        //for Nathan
        //anim.SetFloat("Velocity", rb.velocity.magnitude);
    }
}
