using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public GameObject cam;

    //Movement
    public float speed;
    float moveVelocity;
    private bool facingRight;

    //Dash
    public bool dash = false;
    public float dashSpeed;
    public float dashDuration;
    float dashTimer = 0.0f;
    public float dashCooldown;
    float dashCooldownTimer = 0.0f;

    //Jump
    public float cooldown;
    public float firstJumpHeight;
    public float secondJumpHeight;
    bool firstJump;
    bool secondJump;
    float jumpStartTime;
    float yvel = 0f;

    public bool canMove = true;

    public float initial_x;
    public float initial_y;

    public Vector3 camera_init;

    public bool dead;

    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        facingRight = true;
        dead = false;
        camera_init = cam.transform.position;
        initial_x = transform.position.x;
        initial_y = transform.position.y;
    }

    void Update()
    {
        if (canMove)
        {
            moveVelocity = 0;
            yvel = rb.velocity.y;

            //Left Right Movement
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                moveVelocity = -speed;
                if (facingRight)
                {
                    reverseImage();
                }
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                moveVelocity = speed;
                if (!facingRight)
                {
                    reverseImage();
                }
            }

            //Jump
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (firstJump)
                {
                    jumpStartTime = Time.time;
                    yvel = firstJumpHeight;
                    firstJump = false;
                }
                else if (secondJump && Time.time - jumpStartTime >= cooldown)
                {
                    yvel =  secondJumpHeight;
                    secondJump = false;
                }
            }

            //Dash
            if (Input.GetKey(KeyCode.LeftShift))
            if (Input.GetKey(KeyCode.LeftShift) && dashCooldownTimer <= 0.0f)
            {
                dash = true;
                dashTimer = 0.0f;
                dashCooldownTimer = dashCooldown;
            }
            
            //Update dash timers. Cannot dash infinitely.
            if (dashCooldownTimer > 0.0f) dashCooldownTimer -= Time.deltaTime;
            dashTimer += Time.deltaTime;

            //Do not fall during dash, end dash if past timer
            if (dash)
            {
                moveVelocity *= dashSpeed;
                //rb.AddForce(Physics.gravity * (rb.mass * rb.mass)); //idk why this doesnt work
                yvel = 0;
                if (dashTimer > dashDuration)
                {
                    dash = false;
                    moveVelocity /= dashSpeed;
                }
            }

            rb.velocity = new Vector2(moveVelocity, yvel);
        }
    }

    void reverseImage()
    {
        facingRight = !facingRight;
        Vector2 scale = rb.transform.localScale;
        scale.x *= -1;
        rb.transform.localScale = scale;
        //do not flip camera
        Vector2 cscale = cam.transform.localScale;
        cscale.x *= -1;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Platform")
        {
            firstJump = true;
            secondJump = true;
        }
    }
}