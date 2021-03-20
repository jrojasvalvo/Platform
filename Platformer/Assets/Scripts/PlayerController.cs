using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class PlayerController : MonoBehaviour
{
    Transform[] movables;

    AudioSource[] sounds;
    AudioSource jump1Sound;
    AudioSource jump2Sound;
    AudioSource dashSound;

    private Rigidbody2D rb;
    private Animator anim;
    public GameObject cam;

    bool isTouchingPlat;

    public double playerHeight;

    public float deceleration;
    public float acceleration;

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

    //Wall Jump
    bool touchingWallLeft;
    bool touchingWallRight;
    bool wallSliding;
    public float wallSlidingSpeed;
    public float xWallForce;
    public float yWallForce;

    public bool canMove = true;

    public float initial_x;
    public float initial_y;

    public Vector3 camera_init;

    public bool dead;

    public bool dashReset;

    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        facingRight = false;
        dead = false;
        camera_init = cam.transform.position;
        initial_x = transform.position.x;
        initial_y = transform.position.y;
        sounds = GetComponents<AudioSource>();
        jump1Sound = sounds[0];
        jump2Sound = sounds[1];
        dashSound = sounds[2];
        dashReset = false;
        movables = GameObject.FindGameObjectsWithTag("MovablesParent")[0].GetComponentsInChildren<Transform>();
    }

    void decelerate() {
        if(moveVelocity <= 0.1f && moveVelocity >= -0.1f) {
            moveVelocity = 0;
        } else {
            moveVelocity -= moveVelocity / deceleration;
        }
    }

    void accelerate(float s) {
        if(s < 0) {
            if(moveVelocity <= s + 0.1f) {
                moveVelocity = s;
            } else {
                moveVelocity -= acceleration;
            }
        } else {
            if(moveVelocity >= s - 0.1f) {
                moveVelocity = s;
            } else {
                moveVelocity += acceleration;
            }
        }
    }

    void Update()
    {   
        if(transform.position.y <= -5f) {
            resetRoom();
        }
        if (canMove)
        {
            if(isTouchingPlat) {
                decelerate();
            }
            yvel = rb.velocity.y;

            //Left Right Movement
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                accelerate(-speed);
                if (facingRight)
                {
                    reverseImage();
                }
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                accelerate(speed);
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
                    jump1Sound.Play();
                    jumpStartTime = Time.time;
                    yvel = firstJumpHeight;
                    firstJump = false;
                }
                else if (secondJump && Time.time - jumpStartTime >= cooldown && !wallSliding)
                {
                    jump2Sound.Play();
                    yvel =  secondJumpHeight;
                    secondJump = false;
                }

                if (wallSliding && touchingWallLeft) {
                    jump2Sound.Play();
                    moveVelocity = xWallForce;
                    yvel = yWallForce;
                    wallSliding = false;
                    touchingWallLeft = false;
                } else if (wallSliding && touchingWallRight) {
                    jump2Sound.Play();
                    moveVelocity = -xWallForce;
                    yvel = yWallForce;
                    wallSliding = false;
                    touchingWallRight = false;
                }
            }

            //Dash
            if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0.0f && dashReset)
            {
                dashSound.Play();
                dash = true;
                dashTimer = 0.0f;
                dashCooldownTimer = dashCooldown;
                dashReset = false;
            }

            if(Input.GetKeyDown(KeyCode.R)) {
                resetRoom();
            }
            
            //Update dash timers. Cannot dash infinitely.
            if (dashCooldownTimer > 0.0f) dashCooldownTimer -= Time.deltaTime;
            dashTimer += Time.deltaTime;

            //Do not fall during dash, end dash if past timer
            if (dash)
            {
                if(facingRight) {
                    moveVelocity = dashSpeed;
                } else{
                    moveVelocity = -dashSpeed;
                }
                //rb.AddForce(Physics.gravity * (rb.mass * rb.mass)); //idk why this doesnt work
                yvel = 0;
                if (dashTimer > dashDuration)
                {
                    dash = false;
                    moveVelocity /= dashSpeed;
                }
            }

            //Wall Jump
            if ((touchingWallLeft || touchingWallRight) && !firstJump) {
                wallSliding = true;
            } else {
                wallSliding = false;
            }

            if (wallSliding) {
                yvel = Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue);
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

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == "Platform" && col.transform.position.y <= transform.position.y - (playerHeight / 2))
        {
            firstJump = true;
            secondJump = true;
            isTouchingPlat = true;
        }
    }

    void OnCollisionStay2D(Collision2D col) 
    {
        if (col.collider.tag == "Wall") 
        {
            dashReset = true;
            if(col.transform.position.x < transform.position.x) {
                touchingWallLeft = true;
                if(moveVelocity < 0) {
                    moveVelocity = 0;
                }
            } else if(col.transform.position.x >= transform.position.x) {
                touchingWallRight = true;
                if(moveVelocity > 0) {
                    moveVelocity = 0;
                }
            }
        }
        if (col.collider.tag == "Platform" && col.transform.position.y <= transform.position.y - (playerHeight / 2)) {
            dashReset = true;
            isTouchingPlat = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.tag == "Wall") 
        {
            touchingWallLeft = false;
            touchingWallRight = false;
        }
        if (col.collider.tag == "Platform") {
            isTouchingPlat = false;
            firstJump = false;
        }
    }

    void resetRoom() {
        float resetX = cam.GetComponent<moveCamera>().room * cam.GetComponent<moveCamera>().roomWidth - 10.5f;
        transform.position = new Vector3(resetX, -3f, 0f);
        yvel = 0f;
        moveVelocity = 0f;
        for(int i = 1; i < movables.Length; i++) {
            movables[i].gameObject.GetComponent<movableObjectController>().reset();
        }
    }
}