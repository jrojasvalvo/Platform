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
    public float jump;
    float moveVelocity;
    private bool facingRight;
    //Dash
    bool dash = false;
    public float dashSpeed;
    public float dashDuration;
    float dashTimer = 0.0f;
    public float dashCooldown;
    float dashCooldownTimer = 0.0f;

    //Grounded Vars
    bool grounded = false;
    float fastFall = 0f;
    public float fastFallSpeed;
    public bool canMove = true;

    public float initial_x = -7.13f;
    public float initial_y = -2.5f;

    public Vector3 camera_init;

    public bool dead;
    public bool canJump = true;

    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        grounded = false;
        facingRight = true;
        dead = false;
        camera_init = cam.transform.position;
        initial_x = transform.position.x;
        initial_y = transform.position.y;
        dash = false;
        dashTimer = 0.0f;
    }

    void Update()
    {
        if (canMove)
        {
            /*
            //Jumping
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W))
            {
                if (grounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jump);
                    fastFall = 0f;
                    anim.SetTrigger("jump_start");
                }
            }
            else
            {
                if (!grounded)
                {
                    fastFall = fastFallSpeed;
                }
            }
            
            //can't just hold down the jump button and keep jumping
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow)
                || Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.W))
            {
                canJump = true;
            }
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)
                || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W))
            {
                canJump = false;
            }
            */

            moveVelocity = 0;

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

            //Dash
            if (Input.GetKey(KeyCode.LeftShift) && dashCooldownTimer <= 0f)
            {
                dash = true;
                dashTimer = 0.0f;
                dashCooldownTimer = dashCooldown;
                moveVelocity *= dashSpeed;
            }
            
            //Update dash timers. Cannot dash infinitely.
            dashCooldownTimer -= Time.deltaTime;
            dashTimer += Time.deltaTime;

            //Do not fall during dash, end dash if past timer
            if (dash)
            {
                rb.AddForce(Physics.gravity * (rb.mass * rb.mass));
                if (dashTimer > dashDuration)
                {
                    dash = false;
                    moveVelocity /= dashSpeed;
                }
            }

            rb.velocity = new Vector2(moveVelocity, rb.velocity.y - fastFall);
        }
        if (Time.timeScale == 0) canMove = false;
    }

    void FixedUpdate()
    {
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
        if (grounded)
        {
            anim.SetTrigger("landing");
        }
        else
        {
            anim.ResetTrigger("landing");
        }
        if (transform.position.y < -8)
        {
            dead = true;
        }
    }

    //Check if Grounded
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            grounded = false;
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

    public void LoadNext()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}