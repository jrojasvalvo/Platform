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
    AudioSource deathSound;
    AudioSource music;

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

    bool touchingDoor = false;

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

    bool dashReset;
    
    public bool canDash;
    public bool canDoubleJump;
    public bool canWallJump;

    public BoxCollider2D pushCollider;

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
        deathSound = sounds[3];
        music = sounds[4];
        dashReset = false;
        movables = GameObject.FindGameObjectsWithTag("MovablesParent")[0].GetComponentsInChildren<Transform>();

        music.Play();
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
            StartCoroutine(MusicCoroutine());
            deathSound.Play();
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
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) && !touchingDoor)
            {
                if (firstJump)
                {
                    jump1Sound.Play();
                    jumpStartTime = Time.time;
                    yvel = firstJumpHeight;
                    firstJump = false;
                }
                else if (secondJump && Time.time - jumpStartTime >= cooldown && !wallSliding && canDoubleJump)
                {
                    jump2Sound.Play();
                    yvel =  secondJumpHeight;
                    secondJump = false;
                }

                if (wallSliding && touchingWallLeft && canWallJump) {
                    jump2Sound.Play();
                    moveVelocity = xWallForce;
                    yvel = yWallForce;
                    wallSliding = false;
                    touchingWallLeft = false;
                } else if (wallSliding && touchingWallRight && canWallJump) {
                    jump2Sound.Play();
                    moveVelocity = -xWallForce;
                    yvel = yWallForce;
                    wallSliding = false;
                    touchingWallRight = false;
                }
            }

            //Dash
            if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0.0f && dashReset && canDash)
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
                } else {
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
            if ((touchingWallLeft || touchingWallRight) && !isTouchingPlat) {
                wallSliding = true;
            } else {
                wallSliding = false;
            }

            if (wallSliding) {
                yvel = Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue);
            }

            rb.velocity = new Vector2(moveVelocity, yvel);
            anim.SetFloat("xvel", Mathf.Abs(rb.velocity.x));
            if(rb.velocity.y >= -0.1f && rb.velocity.y <= 0.1f) {
                anim.SetBool("movingUp", false);
                anim.SetBool("movingDown", false);
            } else if (rb.velocity.y < -0.1f) {
                anim.SetBool("movingDown", true);
                anim.SetBool("movingUp", false);
            } else if (rb.velocity.y > 0.1f) {
                anim.SetBool("movingUp", true);
                anim.SetBool("movingDown", false);
            }
            if(rb.velocity.x == 0.0f) {
                anim.SetBool("pushing", false);
            }
        }
    }

    void FixedUpdate()
    {
        anim.SetFloat("xvel", Mathf.Abs(rb.velocity.x));
        if(rb.velocity.y == 0.0f) {
            anim.SetBool("movingUp", false);
            anim.SetBool("movingDown", false);
        } else if (rb.velocity.y < 0.0f) {
            anim.SetBool("movingDown", true);
            anim.SetBool("movingUp", false);
        } else {
            anim.SetBool("movingUp", true);
            anim.SetBool("movingDown", false);
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
        if (col.collider.tag == "Door") {
            touchingDoor = true;
        }
        if ((col.collider.tag == "Platform" || col.collider.tag == "Movable") && 
            col.transform.position.y <= transform.position.y - (playerHeight / 2))
        {
            firstJump = true;
            secondJump = true;
            isTouchingPlat = true;
        }
    }

    void OnCollisionStay2D(Collision2D col) 
    {
        if (col.collider.tag == "Wall" && canWallJump) 
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
        if ((col.collider.tag == "Platform" || col.collider.tag == "Movable") && 
            col.transform.position.y <= transform.position.y - (playerHeight / 2)) {
            dashReset = true;
            isTouchingPlat = true;
            firstJump = true;
        } 
        //else if(col.collider.tag == "Movable") {
            //anim.SetBool("pushing", true);
        //}
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Movable")
        {
            anim.SetBool("pushing", true);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Movable")
        {
            anim.SetBool("pushing", false);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.tag == "Wall") 
        {
            touchingWallLeft = false;
            touchingWallRight = false;
        }
        if ((col.collider.tag == "Platform" || col.collider.tag == "Movable") && 
            col.transform.position.y <= transform.position.y - (playerHeight / 2)) {
            isTouchingPlat = false;
            firstJump = false;
        } 
        //else if(col.collider.tag == "Movable") {
           // anim.SetBool("pushing", false);
        //}
    }

    public void resetRoom() {
        float resetX = cam.GetComponent<moveCamera>().room * cam.GetComponent<moveCamera>().roomWidth - 7.5f;
        transform.position = new Vector3(resetX, -3f, 0f);
        yvel = 0f;
        moveVelocity = 0f;
        for(int i = 1; i < movables.Length; i++) {
            movables[i].gameObject.GetComponent<movableObjectController>().reset();
        }
    }

    IEnumerator MusicCoroutine()
    {
        music.volume = 0.1f;
        yield return new WaitForSeconds(1.8f);
        music.volume += 0.05f;
        yield return new WaitForSeconds(0.05f);
        music.volume += 0.05f;
        yield return new WaitForSeconds(0.05f);
        music.volume += 0.05f;
        yield return new WaitForSeconds(0.05f);
        music.volume += 0.05f;
        yield return new WaitForSeconds(0.05f);
        music.volume += 0.05f;
        yield return new WaitForSeconds(0.05f);
        music.volume += 0.05f;
        yield return new WaitForSeconds(0.05f);
        music.volume += 0.05f;
        yield return new WaitForSeconds(0.05f);
        music.volume += 0.05f;
    }
}