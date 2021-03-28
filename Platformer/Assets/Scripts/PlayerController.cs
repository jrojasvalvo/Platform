using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    public AudioSource music;

    private Rigidbody2D rb;
    public Animator anim;
    public GameObject cam;

    public bool isTouchingPlat;

    public double playerHeight;

    public float deceleration;
    public float groundDeceleration;
    public float airDeceleration;
    public float acceleration;

    //Movement
    public float speed;
    float moveVelocity;
    public bool facingRight;

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
    public bool firstJump;
    public bool secondJump;
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

    public bool dashReset;
    
    public bool canDash;
    public bool canDoubleJump;
    public bool canWallJump;

    public BoxCollider2D hitBox;

    public bool movingLeft;
    public bool movingRight;
    bool jumpPressed;
    bool dashPressed;
    public float normalGravity;
    public float fastFallGravity;
    private List<float> inputBuffer = new List<float>();

    public GameObject cutsceneManager;

    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        facingRight = true;
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
        cutsceneManager = GameObject.Find("CutsceneManager"); //to play next cutscene upon event do cutsceneManager.GetComponent<CutsceneManager>().PlayNext();
        //music.Play();
    }

    void Update()
    {
        if (canMove)
        {
            //Movement Stuff

            //Get Inputs
            movingLeft = false;
            movingRight = false;
            jumpPressed = false;
            dashPressed = false;
            // To stop player from entering idle animation when switching directions
            anim.SetBool("pressingLeftorRight", false);

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
                movingLeft = true;
                movingRight = false;
                anim.SetBool("pressingLeftorRight", true);
            } else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
                movingLeft = false;
                movingRight = true;
                anim.SetBool("pressingLeftorRight", true);
            }
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) && !touchingDoor) {
                jumpPressed = true;
                inputBuffer.Add(Time.time);
                rb.gravityScale = normalGravity;
            }
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow) && !touchingDoor) {
                rb.gravityScale = fastFallGravity;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0.0f && dashReset && canDash) {
                dashPressed = true;
            }

            SideMovement();
            tryBufferedJump();
            Jump();
            Dash();
            if (canWallJump) {
                WallJump();
            }
            
            if(Input.GetKeyDown(KeyCode.R)) {
                resetRoom();
            }

            rb.velocity = new Vector2(moveVelocity, yvel);
            anim.SetFloat("xvel", Mathf.Abs(rb.velocity.x));
            if(rb.velocity.y >= -0.15f && rb.velocity.y <= 0.15f) {
                anim.SetBool("movingUp", false);
                anim.SetBool("movingDown", false);
            } else if (rb.velocity.y < -0.1f && !firstJump) {
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

        if(transform.position.y <= -5f) {
            StartCoroutine(MusicCoroutine());
            deathSound.Play();
            resetRoom();
        }

        /* old code that deactivates music during cutscene
        if (noah.activeSelf == true)
        {
            music.volume -= 0.5f;
        } else if (noah.activeSelf == false && deathSound.isPlaying == false)
        {
            music.volume = 0.5f;
        }
        */

    }

    void FixedUpdate()
    {
        //If we need to we can put the movement stuff here but it works in update for now
    }

    void decelerate(bool onGround) {
        if (onGround) {
            deceleration = groundDeceleration;
        } else {
            deceleration = airDeceleration;
        }

        if(moveVelocity <= 0.15f && moveVelocity >= -0.15f) {
            moveVelocity = 0;
        } else {
            // moveVelocity -= moveVelocity / deceleration;
            // moveVelocity -= deceleration * Time.fixedDeltaTime;

            if (moveVelocity <= 0.5f && moveVelocity >= -0.5f) {
                //Sometimes deceleration is too much to stay within stopping threshold and player keeps sliding
                deceleration /= 2f;
            }

            if(moveVelocity < 0) {
                moveVelocity += deceleration * Time.fixedDeltaTime;
            } else {
                moveVelocity -= deceleration * Time.fixedDeltaTime;
            }
        }
    }

    void accelerate(float s) {
        float a = acceleration;
        if ((moveVelocity < 0 && movingRight) || (moveVelocity > 0 && movingLeft)) {
                //So the player doesn't moonwalk as much when switching directions
                a *= 2;
        }
        if(s < 0) {
            if(moveVelocity <= s + 0.15f) {
                moveVelocity = s;
            } else {
                moveVelocity -= a * Time.fixedDeltaTime;
            }
        } else {
            if(moveVelocity >= s - 0.15f) {
                moveVelocity = s;
            } else {
                moveVelocity += a * Time.fixedDeltaTime;
            }
        }
    }

    void SideMovement() {
        
        yvel = rb.velocity.y;

        //Left Right Movement
        if (movingLeft)
        {
            accelerate(-speed);
            if (facingRight)
            {
                reverseImage();
            }
        }
        else if (movingRight)
        {
            accelerate(speed);
            if (!facingRight)
            {
                reverseImage();
            }
        } else {
            if(isTouchingPlat) {
                decelerate(true);
            } else {
                decelerate(false);
            }
        }
    }

    // Enables player to jump if they presed the key slightly early
    // Keeps game from 'eating' player input
    private void tryBufferedJump() {
        if (inputBuffer.Count > 0 && firstJump) {
            foreach (float t in inputBuffer.ToArray()) {
                if (Time.time - t < 0.07f) {
                    jumpPressed = true;
                    inputBuffer.Clear();
                    break;
                }
            }
        }
    }

    //Player can jump slightly after leaving platform so game doesn't feel like its eating inputs
    public IEnumerator coyoteTime()
    {
        yield return new WaitForSeconds((float)0.05);
        firstJump = false;
    }

    void Jump() {
        if (jumpPressed)
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
    }

    void Dash(){
        //Dash
        if (dashPressed)
        {
            dashSound.Play();
            dash = true;
            dashTimer = 0.0f;
            dashCooldownTimer = dashCooldown;
            dashReset = false;
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
    }

    void WallJump() {
        if ((touchingWallLeft || touchingWallRight) && !isTouchingPlat) {
            wallSliding = true;
        } else {
            wallSliding = false;
        }

        if (wallSliding) {
            yvel = Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue);
        }
    }

    void reverseImage()
    {   
        facingRight = !facingRight;
        
        Vector2 scale = rb.transform.localScale;
        scale.x *= -1;
        rb.transform.localScale = scale;
        float offset = hitBox.offset.x * scale.x;
        rb.transform.position = new Vector3(rb.transform.position.x - offset, 
                                                rb.transform.position.y, rb.transform.position.z);
        //do not flip camera
        Vector2 cscale = cam.transform.localScale;
        cscale.x *= -1;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == "Door") {
            touchingDoor = true;
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
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.tag == "Wall") 
        {
            touchingWallLeft = false;
            touchingWallRight = false;
        }
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