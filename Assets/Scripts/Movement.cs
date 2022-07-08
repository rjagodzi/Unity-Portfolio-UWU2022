using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(x, y);

        Walk(direction);
    }

    private void Walk (Vector2 direction)
    {
        rb.velocity = (new Vector2(direction.x * speed, rb.velocity.y));
    }
}
