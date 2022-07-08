using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Collision collision;
    private Rigidbody2D rb;

    private float speed = 10;
    private float slideSpeed = 5;

    private bool wallGrab;

    // Start is called before the first frame update
    void Start()
    {
        collision = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        Debug.DrawLine(Vector3.zero, new Vector3(5, 0, 0), Color.white, 2.5f);
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(x, y);

        Walk(direction);

        if (collision.onWall && !collision.onGround)
        {
            WallSlide();         
        }
    }

    private void Walk (Vector2 direction)
    {
        rb.velocity = (new Vector2(direction.x * speed, rb.velocity.y));
    }

    private void WallSlide()
    {
        rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
    }
}
