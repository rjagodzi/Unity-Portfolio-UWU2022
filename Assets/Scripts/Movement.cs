using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Collision collision;
    public Collision Collision => collision;
    private Rigidbody2D rb;
    [SerializeField] private Animator playerAnimator;
    public Animator Animator => playerAnimator;

    public string HorizontalAxis;
    public string VerticalAxis;
    public GameObject shadow;
    public float normalSpeed = 10;
    public float slideSpeed = 5;
    public float jumpForce = 10;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20f;
    public float dashCooldown = 1f;
    public float dashLength = 0.5f;
    public float currentMovementSpeed;

    //private bool wallGrab;
    public bool wallSlide;
    public bool canMove = true;
    public bool wallJumped;
    public bool canDash;

    // Start is called before the first frame update
    void Start()
    {
        currentMovementSpeed = normalSpeed;
        canDash = true;
        collision = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
     //   playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis(HorizontalAxis);
        float y = Input.GetAxis(VerticalAxis);
        float xRaw = Input.GetAxisRaw(HorizontalAxis);
        float yRaw = Input.GetAxisRaw(VerticalAxis);
        Vector2 direction = new Vector2(x, y);
            
        Walk(direction);
        Dash();

        if (collision.onGround)
        {
            wallJumped = false;
            GetComponent<Jumping>().enabled = true;
            shadow.gameObject.SetActive(true);
        }

        if (!collision.onGround)
        {
            shadow.gameObject.SetActive(false);
        }

        if (collision.onWall && !collision.onGround && !wallJumped)
        {
            //StopCoroutine(DisableMovement(0));
            StartCoroutine(DisableMovement(0.2f));
            wallJumped = true;
            WallSlide();
        }
        
        //if(!collision.onWall && collision.onGround)
        //{
        //    wallSlide = false;
        //}
    }

    public void DoJump()
    {
      //  if (collision.onGround)
      //  {
            Jump(Vector2.up, false);
        playerAnimator.SetTrigger("jump");
      //  }


        if (collision.onWall && !collision.onGround)
        {
            WallJump();
        }
    }

    private void Walk (Vector2 direction)
    {
        jumpForce = 10;

        if (!canMove)
        {
            return;
        }

        if (!wallJumped)
        {
            rb.velocity = (new Vector2(direction.x * currentMovementSpeed, rb.velocity.y));
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(direction.x * currentMovementSpeed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }

        if(collision.onGround && rb.velocity.x > 0)
        {
            playerAnimator.SetTrigger("isWalkingRight");
        }
        else if(collision.onGround && rb.velocity.x < 0)
        {
            playerAnimator.SetTrigger("isWalkingLeft");
        }
        else
        {
            playerAnimator.SetTrigger("isIdle");
        }
    }

    private void WallSlide()
    {
        rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
    }

    private void WallJump()
    {
        jumpForce = 15;

        wallJumped = true;

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDirection = collision.onRightWall ? Vector2.left : Vector2.right;
        Jump((Vector2.up / 2f + wallDirection / 0.5f), true);
        
    }

    private void Jump(Vector2 direction, bool wall)
    {
        rb.velocity = new Vector2(wall?0:rb.velocity.x, 0);
        rb.velocity += direction * jumpForce;
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            currentMovementSpeed = dashSpeed;
            canDash = false;

            if (!collision.onGround && rb.velocity.x > 0)
            {
                playerAnimator.SetTrigger("dashRight");
            }
            else if (!collision.onGround && rb.velocity.x < 0)
            {
                playerAnimator.SetTrigger("dashLeft");
            }

            StartCoroutine(DashCooldownCounter());
            StartCoroutine(DashLengthCounter());

        }
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
        wallJumped = false;
    }

    IEnumerator DashCooldownCounter()
    {
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    IEnumerator DashLengthCounter()
    {
        yield return new WaitForSeconds(dashLength);

        currentMovementSpeed = normalSpeed;
    }
}
