    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJump : MonoBehaviour
{
    public float cooldown;
    public float firstJumpHeight;
    public float secondJumpHeight;
    Transform t;
    Rigidbody2D rb;
    bool firstJump;
    bool secondJump;
    float jumpStartTime;
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            if(firstJump) {
                jumpStartTime = Time.time;
                rb.velocity = new Vector2(rb.velocity.x, (Vector2.up * firstJumpHeight).y);
                firstJump = false;
            } else if(secondJump && Time.time - jumpStartTime >= cooldown) {
                rb.velocity = new Vector2(rb.velocity.x, (Vector2.up * secondJumpHeight).y);
                secondJump = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if(col.tag == "Platform") {
            firstJump = true;
            secondJump = true;
        }
    }
}
