using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossController : MonoBehaviour
{
    // GameObject p;
    // public float delay;
    // Rigidbody2D rb;
    // float jumpStartTime;
    // bool firstJump;
    // bool secondJump;
    // float firstJumpHeight;
    // float secondJumpHeight;
    // float yvel;
    // bool dash;
    // bool isTouchingPlat;
    // Animator anim;

    // void Start() {
    //     p = GameObject.Find("Player");
    //     rb = gameObject.GetComponent<Rigidbody2D>();
    //     anim = GetComponent<Animator>();
    //     firstJumpHeight = p.GetComponent<PlayerController>().firstJumpHeight;
    //     secondJumpHeight = p.GetComponent<PlayerController>().secondJumpHeight;
    // }

    // void Update() {
    //     anim.SetFloat("xvel", Mathf.Abs(rb.velocity.x));
    //     if (rb.velocity.y >= -0.15f && rb.velocity.y <= 0.15f)
    //     {
    //         anim.SetBool("movingUp", false);
    //         anim.SetBool("movingDown", false);
    //     }
    //     else if (rb.velocity.y < -1f && !firstJump)
    //     {
    //         anim.SetBool("movingDown", true);
    //         anim.SetBool("movingUp", false);
    //     }
    //     else if (rb.velocity.y > 1f)
    //     {
    //         anim.SetBool("movingUp", true);
    //         anim.SetBool("movingDown", false);
    //     }
    //     if (rb.velocity.x == 0.0f)
    //     {
    //         anim.SetBool("pushing", false);
    //     }
    // }

    // public IEnumerator bossJump() {
    //     yield return new WaitForSeconds(delay);
    //     if (firstJump)
    //     {
    //         jumpStartTime = Time.time;
    //         yvel = firstJumpHeight;
    //         if(dash && isTouchingPlat) {
    //             rb.AddForce(new Vector2(1000f, 0f));
    //         }
    //         firstJump = false;
    //     }
    //     else if (secondJump && Time.time - jumpStartTime >= cooldown && !wallSliding && canDoubleJump)
    //     {
    //         yvel =  secondJumpHeight;
    //         secondJump = false;
    //     }

    //     if (wallSliding && touchingWallLeft && canWallJump) {
    //         moveVelocity = xWallForce;
    //         yvel = yWallForce;
    //         wallSliding = false;
    //         touchingWallLeft = false;
    //     } else if (wallSliding && touchingWallRight && canWallJump) {
    //         moveVelocity = -xWallForce;
    //         yvel = yWallForce;
    //         wallSliding = false;
    //         touchingWallRight = false;
    //     }
    // }

    // void bossDash(){
    //     if(isTouchingPlat) {
    //         startedInAir = false;
    //     }
        
    //     anim.SetBool("dash", true);
    //     dash = true;
    //     dashTimer = 0.0f;
    //     dashCooldownTimer = dashCooldown;
    //     dashReset = false;
    //     dashPressed = false;

    //     //Do not fall during dash, end dash if past timer
    //     if (dash)
    //     {
    //         if(facingRight) {
    //             moveVelocity = dashSpeed;
    //         } else {
    //             moveVelocity = -dashSpeed;
    //         }

    //         if(startedInAir && secondJump) {
    //             yvel = 0;
    //         } else if (!secondJump) {
    //             if(yvel <= 0) {
    //                 yvel = 0;
    //             }
    //         }

    //         if (dashTimer > dashDuration)
    //         {
    //             dash = false;
    //             anim.SetBool("dash", false);
    //             moveVelocity /= dashSpeed;
    //             startedInAir = true;
    //         }
    //     }

    //     //Update dash timers. Cannot dash infinitely.
    //     if (dashCooldownTimer > 0.0f) dashCooldownTimer -= Time.deltaTime;
    //     dashTimer += Time.deltaTime;
    // }

    // void bossWallJump() {
    //     if ((touchingWallLeft || touchingWallRight) && !isTouchingPlat) {
    //         wallSliding = true;
    //     } else {
    //         wallSliding = false;
    //     }

    //     if (wallSliding) {
    //         yvel = Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue);
    //     }
    // }

    // void bossReverseImage()
    // {   
    //     facingRight = !facingRight;
        
    //     Vector2 scale = rb.transform.localScale;
    //     scale.x *= -1;
    //     rb.transform.localScale = scale;
    //     float offset = hitBox.offset.x * scale.x;
    //     rb.transform.position = new Vector3(rb.transform.position.x - offset, 
    //                                             rb.transform.position.y, rb.transform.position.z);
    //     //do not flip camera
    //     Vector2 cscale = cam.transform.localScale;
    //     cscale.x *= -1;
    // }

    // void OnCollisionStay2D(Collision2D col) 
    // {
    //     if (col.collider.tag == "Wall") 
    //     {
    //         dashReset = true;
    //         if(col.transform.position.x < transform.position.x) {
    //             touchingWallLeft = true;
    //             if(moveVelocity < 0) {
    //                 moveVelocity = 0;
    //             }
    //         } else if(col.transform.position.x >= transform.position.x) {
    //             touchingWallRight = true;
    //             if(moveVelocity > 0) {
    //                 moveVelocity = 0;
    //             }
    //         }
    //     }
    // }

    // void OnCollisionExit2D(Collision2D col)
    // {
    //     if (col.collider.tag == "Wall") 
    //     {
    //         touchingWallLeft = false;
    //         touchingWallRight = false;
    //     }
    // }
}
