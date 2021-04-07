using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossController : MonoBehaviour
{
    GameObject p;
    PlayerController pc;
    public float delay;
    public Rigidbody2D rb;
    public BoxCollider2D hitBox;
    public GameObject cam;
    bool facingRight;

    //side movement
    float deceleration;
    float acceleration;
    float groundDeceleration;
    float airDeceleration;
    float speed;
    public bool movingLeft;
    public bool movingRight;

    //jumping 
    float jumpStartTime;
    public bool firstJump = true;
    public bool secondJump;
    float firstJumpHeight;
    float secondJumpHeight;
    float yvel;
    public bool isTouchingPlat;
    float cooldown;
    bool canDoubleJump;

    //wall sliding
    bool wallSliding;
    bool touchingWallLeft;
    bool touchingWallRight;
    bool canWallJump;
    float moveVelocity;
    float xWallForce;
    float yWallForce;
    float wallSlidingSpeed;

    //dashing 
    bool startedInAir;
    bool dash = false;
    float dashTimer;
    float dashCooldown;
    float dashCooldownTimer = 0.0f;
    public bool dashReset;
    float dashSpeed;
    float dashDuration;

    Animator anim;

    void Start() {
        p = GameObject.Find("Player");
        pc = p.GetComponent<PlayerController>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        firstJumpHeight = pc.firstJumpHeight;
        secondJumpHeight = pc.secondJumpHeight;
        xWallForce = pc.xWallForce;
        yWallForce = pc.yWallForce;
        wallSlidingSpeed = pc.wallSlidingSpeed;
        groundDeceleration = pc.groundDeceleration;
        airDeceleration = pc.airDeceleration;
        acceleration = pc.acceleration;
        speed = pc.speed;
        canWallJump = pc.canWallJump;
        canDoubleJump = pc.canDoubleJump;
        facingRight = pc.facingRight;
        dashSpeed = pc.dashSpeed;
        dashCooldown = pc.dashCooldown;
        dashDuration = pc.dashDuration;
    }

    void Update() {
        anim.SetFloat("xvel", Mathf.Abs(rb.velocity.x));
        anim.SetBool("pressingLeftorRight", (movingLeft || movingRight));
        if (rb.velocity.y >= -0.15f && rb.velocity.y <= 0.15f)
        {
            anim.SetBool("movingUp", false);
            anim.SetBool("movingDown", false);
        }
        else if (rb.velocity.y < -1f && !firstJump)
        {
            anim.SetBool("movingDown", true);
            anim.SetBool("movingUp", false);
        }
        else if (rb.velocity.y > 1f)
        {
            anim.SetBool("movingUp", true);
            anim.SetBool("movingDown", false);
        }
        if (rb.velocity.x == 0.0f)
        {
            anim.SetBool("pushing", false);
        }
        dashing();
    }

    void FixedUpdate() {
        rb.velocity = new Vector2(moveVelocity, yvel);
    }

    public IEnumerator SideMovement(bool left, bool right) {
        yield return new WaitForSeconds(delay);
        movingLeft = left;
        movingRight = right;
        //Left Right Movement
        yvel = rb.velocity.y;
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

    void decelerate(bool onGround) {
        if (onGround) {
            deceleration = groundDeceleration;
        } else {
            deceleration = airDeceleration;
        }

        if(moveVelocity <= 0.2f && moveVelocity >= -0.2f) {
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

    public IEnumerator coyoteTime()
    {
        yield return new WaitForSeconds((float)0.07);
        firstJump = false;
    }

    public IEnumerator Jump() {
        yield return new WaitForSeconds(delay);
        
        if (firstJump)
        {
            jumpStartTime = Time.time;
            yvel = firstJumpHeight;
            if(dash && isTouchingPlat) {
                rb.AddForce(new Vector2(1000f, 0f));
            }
            firstJump = false;
        }
        else if (secondJump && Time.time - jumpStartTime >= cooldown && !wallSliding && canDoubleJump)
        {
            yvel =  secondJumpHeight;
            secondJump = false;
        }

        if (wallSliding && touchingWallLeft && canWallJump) {
            moveVelocity = xWallForce;
            yvel = yWallForce;
            wallSliding = false;
            touchingWallLeft = false;
        } else if (wallSliding && touchingWallRight && canWallJump) {
            moveVelocity = -xWallForce;
            yvel = yWallForce;
            wallSliding = false;
            touchingWallRight = false;
        }
    }

    public IEnumerator changeGravity(bool hold) {
        yield return new WaitForSeconds(delay);
        if (hold) {
            rb.gravityScale = pc.normalGravity;
        } else {
            rb.gravityScale = pc.fastFallGravity;
        }
    }

    public IEnumerator Dash(){
        yield return new WaitForSeconds(delay);

        if(isTouchingPlat) {
            startedInAir = false;
        }
        
        anim.SetBool("dash", true);
        dash = true;
        dashTimer = 0.0f;
        dashCooldownTimer = dashCooldown;
        dashReset = false;
    }

    void dashing() {
        //Do not fall during dash, end dash if past timer
        if (dash)
        {
            rb.gravityScale = 0;
            if(facingRight) {
                moveVelocity = dashSpeed;
            } else {
                moveVelocity = -dashSpeed;
            }

            if(startedInAir && secondJump) {
                yvel = 0;
            } else if (!secondJump) {
                if(yvel <= 0) {
                    yvel = 0;
                }
            }

            if (dashTimer > dashDuration)
            {
                dash = false;
                anim.SetBool("dash", false);
                moveVelocity /= dashSpeed;
                startedInAir = true;
                rb.gravityScale = pc.normalGravity;
            }
        }

        //Update dash timers. Cannot dash infinitely.
        if (dashCooldownTimer > 0.0f) dashCooldownTimer -= Time.deltaTime;
        dashTimer += Time.deltaTime;
    }

    public IEnumerator WallJump() {
        yield return new WaitForSeconds(delay);

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

    void OnCollisionStay2D(Collision2D col) 
    {
        if (col.collider.tag == "Wall") 
        {
            dashReset = true;
            if(!facingRight) {
                touchingWallLeft = true;
                if(moveVelocity < 0) {
                    moveVelocity = 0;
                }
            } else if(facingRight) {
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
}
