using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Collision collision;
    private Rigidbody2D rb;

    public float speed = 10;
    public float slideSpeed = 5;
    public float jumpForce = 50;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;

    //private bool wallGrab;
    public bool wallSlide;
    public bool canMove = true;
    public bool wallJumped;
    public bool hasDashed = false;

    // Start is called before the first frame update
    void Start()
    {
        collision = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 direction = new Vector2(x, y);

        Walk(direction);

        if (collision.onGround)
        {
            wallJumped = false;
            GetComponent<Jumping>().enabled = true;
        }

        if (collision.onWall && !collision.onGround)
        {
            wallSlide = true;
            WallSlide();
        }

        if(!collision.onWall && collision.onGround)
        {
            wallSlide = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (collision.onGround)
            {
                Jump(Vector2.up, false);
            }
            

            if(collision.onWall && !collision.onGround)
            {
                WallJump();
            }
            
        }

        if(Input.GetButtonDown("Fire3"))
        {
            if(xRaw != 0 || yRaw != 0)
            {
                Dash(xRaw, yRaw);
            }
        }
    }

    private void Walk (Vector2 direction)
    {
        if (!canMove)
        {
            return;
        }

        if (!wallJumped)
        {
            rb.velocity = (new Vector2(direction.x * speed, rb.velocity.y));
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(direction.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void WallSlide()
    {
        rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
    }

    private void WallJump()
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

         Vector2 wallDirection = collision.onRightWall ? Vector2.left : Vector2.right;
        Jump((Vector2.up * 5f + wallDirection), true);
        wallJumped = true;
    }

    private void Jump(Vector2 direction, bool wall)
    {
        rb.velocity = new Vector2(wall?0:rb.velocity.x, 0);
        rb.velocity += direction * jumpForce;
    }

    private void Dash(float x, float y)
    {
        rb.velocity = Vector2.zero;
        Vector2 direction = new Vector2(x, y);

        rb.velocity += direction.normalized * dashSpeed;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
}
