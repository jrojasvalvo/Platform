using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Transform[] movables;

    AudioSource[] sounds;
    AudioSource jump1Sound;
    AudioSource jump2Sound;
    AudioSource dashSound;
    AudioSource deathSound;
   // AudioSource music;


    private Rigidbody2D rb;
    public Animator anim;
    public GameObject cam;

    public bool isTouchingPlat;

    public float deceleration;
    public float groundDeceleration;
    public float airDeceleration;
    public float acceleration;

    //Movement
    public float speed;
    public float moveVelocity;
    public bool facingRight;

    //Dash
    bool dash = false;
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

    public float bossInputDelay;
    public GameObject boss;

    public BoxCollider2D hitBox;

    public bool movingLeft;
    public bool movingRight;
    bool jumpPressed;
    bool dashPressed;
    public float normalGravity;
    public float fastFallGravity;
    float delay;
    private List<float> inputBuffer = new List<float>();

    public GameObject cutsceneManager;
    private bool midcutsceneComplete;
    private bool midcutscene2;

    public AudioSource cutsceneMusic;
    bool startedInAir = true;

    public GameObject noah;
    public PushController pushController;

    public GameObject music;
    public AudioManager audioManager;

    public GameObject blackscreen;
    public Image blackscreenImg;

    float invincible;
    GameObject abilities;
    AbilityTracker a;

    void OnAwake()
    {
        cutsceneMusic.Stop();
    }

    void Start()
    {
        abilities = GameObject.FindGameObjectsWithTag("Abilities")[0];
        a = abilities.GetComponent<AbilityTracker>();
        if(a.dash) {
            canDash = true;
        }
        if(a.doubleJump) {
            canDoubleJump = true;
        }
        if(a.wallJump) {
            canWallJump = true;
        }
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        facingRight = true;
        dead = false;
        camera_init = cam.transform.position;
        sounds = GetComponents<AudioSource>();
        jump1Sound = sounds[0];
        jump2Sound = sounds[1];
        dashSound = sounds[2];
        deathSound = sounds[3];
        //music = sounds[4];
        dashReset = false;
        movables = GameObject.FindGameObjectsWithTag("MovablesParent")[0].GetComponentsInChildren<Transform>();
        cutsceneManager = GameObject.Find("CutsceneManager"); //to play next cutscene upon event do cutsceneManager.GetComponent<CutsceneManager>().PlayNext();
        audioManager = GameObject.FindWithTag("Music").GetComponent<AudioManager>();
        audioManager.PlayMusic();
        midcutsceneComplete = false;
        midcutscene2 = false;
        boss = GameObject.FindGameObjectsWithTag("Boss")[0];
        blackscreen = GameObject.Find("Blackscreen").transform.GetChild(0).gameObject;
        blackscreenImg = blackscreen.GetComponent<Image>();
        invincible = 0f;
        delay = boss.GetComponent<bossController>().delay;
    }

    void Update()
    {
        if (canMove)
        {
            // StartCoroutine(getInputs());
            getInputs();

            Dash();

            anim.SetFloat("xvel", Mathf.Abs(rb.velocity.x));
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
        }

        if (transform.position.y <= -5f)
        {
            //music.volume = 0.5f;
            audioManager.LowerVolume();
            resetRoom();
        }


        //Level 1 Cutscene Trigger
        if (!midcutsceneComplete && SceneManager.GetActiveScene().name == "Level1" && rb.transform.position.x > 27)
        {
            cutsceneManager.GetComponent<CutsceneManager>().PlayNext();
            midcutsceneComplete = true;
        }

        //Level 3 Cutscene Trigger
        if (!midcutsceneComplete && SceneManager.GetActiveScene().name == "Level3" && rb.transform.position.x > 46)
        {
            cutsceneManager.GetComponent<CutsceneManager>().PlayNext();
            midcutsceneComplete = true;
        }
        //Level 3 Cutscene Trigger
        if (!midcutscene2 && SceneManager.GetActiveScene().name == "Level3" && rb.transform.position.x > 62)
        {
            cutsceneManager.GetComponent<CutsceneManager>().PlayNext();
            midcutscene2 = true;
        }
        invincible += 0.1f;

    }


    // public IEnumerator getInputs() {
    void getInputs() {
        movingLeft = false;
        movingRight = false;
        //jumpPressed = false;
        dashPressed = false;
        
        if (canDash && Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0.0f && dashReset)
        {
            dashPressed = true;
        }
        // To stop player from entering idle animation when switching directions
        anim.SetBool("pressingLeftorRight", false);
        // yield return new WaitForSeconds(delay);
        // boss.GetComponent<bossController>().movingLeft = false;
        // boss.GetComponent<bossController>().movingRight = false;
        // boss.GetComponent<Animator>().SetBool("pressingLeftorRight", false);
        

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            jumpPressed = true;
            inputBuffer.Add(Time.time);
            rb.gravityScale = normalGravity;
            StartCoroutine(boss.GetComponent<bossController>().changeGravity(true));
        }
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            rb.gravityScale = fastFallGravity;
            StartCoroutine(boss.GetComponent<bossController>().changeGravity(false));
        }
        
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            movingLeft = true;
            movingRight = false;
            anim.SetBool("pressingLeftorRight", true);
            // yield return new WaitForSeconds(delay);
            // boss.GetComponent<bossController>().movingLeft = true;
            // boss.GetComponent<bossController>().movingRight = false;
            
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            movingLeft = false;
            movingRight = true;
            anim.SetBool("pressingLeftorRight", true);
            // yield return new WaitForSeconds(delay);
            // boss.GetComponent<bossController>().movingLeft = false;
            // boss.GetComponent<bossController>().movingRight = true;
        }
    
    }

    void FixedUpdate()
    {
        //If we need to we can put the movement stuff here but it works in update for now
        if (canMove) {
            //Movement Stuff

            SideMovement();
            
            tryBufferedJump();
            Jump();
            
            
            if (canWallJump)
            {
                WallJump();
            }

            /*if (Input.GetKeyDown(KeyCode.R))
            {
                resetRoom();
            }*/

            rb.velocity = new Vector2(moveVelocity, yvel);
        }
    }

    public void decelerate(bool onGround) {
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

    void SideMovement() {
        //Left Right Movement
        if (!dash) {
            StartCoroutine(boss.GetComponent<bossController>().SideMovement(movingLeft, movingRight));
            yvel = rb.velocity.y;
            if (movingLeft)
            {
                accelerate(-speed);
                if (facingRight && !pushController.pulling)
                {
                    reverseImage();
                }
            }
            else if (movingRight)
            {
                accelerate(speed);
                if (!facingRight && !pushController.pulling)
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
        yield return new WaitForSeconds((float)0.07);
        firstJump = false;
    }

    void Jump() {
        if (jumpPressed)
        {
            jumpPressed = false;
            StartCoroutine(boss.GetComponent<bossController>().Jump());
            if (firstJump)
            {
                jump1Sound.Play();
                jumpStartTime = Time.time;
                yvel = firstJumpHeight;
                if(dash && isTouchingPlat) {
                    rb.AddForce(new Vector2(1000f, 0f));
                }
                firstJump = false;
            }
            else if (secondJump && !wallSliding && canDoubleJump && Time.time - jumpStartTime >= cooldown)
            {
                jump2Sound.Play();
                yvel = secondJumpHeight;
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
            boss.GetComponent<bossController>().StartCoroutine("Dash");
            if(isTouchingPlat) {
                startedInAir = false;
            }
            
            anim.SetBool("dash", true);
            dashSound.Play();
            dash = true;
            dashTimer = 0.0f;
            dashCooldownTimer = dashCooldown;
            dashReset = false;
            dashPressed = false;
        }

        //Do not fall during dash, end dash if past timer
        if (dash)
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
            if(facingRight) {
                moveVelocity = dashSpeed;
            } else {
                moveVelocity = -dashSpeed;
            }

            if(startedInAir) {
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
                GetComponent<Rigidbody2D>().gravityScale = normalGravity;
            }
        }

        //Update dash timers. Cannot dash infinitely.
        if (dashCooldownTimer > 0.0f) dashCooldownTimer -= Time.deltaTime;
        dashTimer += Time.deltaTime;
    }

    void WallJump() {
        boss.GetComponent<bossController>().StartCoroutine("WallJump");
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
        // boss.GetComponent<bossController>().StartCoroutine("bossReverseImage");
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

    void OnTriggerEnter2D(Collider2D col) {
        if(col.tag == "Hazard") {
            resetRoom();
        }
        if (col.tag == "Boss" && invincible > 5 && SceneManager.GetActiveScene().name == "Level3")
        {
            resetRoom();
        }
    }

    void OnCollisionStay2D(Collision2D col) 
    {
        if (col.collider.tag == "Wall") 
        {
            dashReset = true;
            if (!facingRight)
            {
                touchingWallLeft = true;
                if (moveVelocity < 0)
                {
                    moveVelocity = 0;
                }
            } else if (facingRight)     {
                touchingWallRight = true;
                if (moveVelocity > 0)
                {
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
        StartCoroutine(DeathScreenFade());
        movingLeft = false;
        movingRight = false;
        jumpPressed = false;
        dashPressed = false;
        dash = false;
        yvel = 0;
        moveVelocity = 0;
        deathSound.Play();
        audioManager.LowerVolume();
        canMove = false;
        anim.SetBool("dash", false);
        rb.gravityScale = normalGravity;
       // StartCoroutine(MusicCoroutine());
       // cam.GetComponent<ScreenShake>().CameraShake();
        if (audioManager.music.volume < 0.5f)
        {
            audioManager.music.volume += 0.005f;
        }
        float resetX = cam.GetComponent<moveCamera>().room * cam.GetComponent<moveCamera>().roomWidth + initial_x;
        transform.position = new Vector3(resetX, -3f, 0f);
        yvel = 0f;
        moveVelocity = 0f;

        if(SceneManager.GetActiveScene().name == "Level3") {
            boss.transform.position = new Vector3(resetX, -3f, 0f);
            boss.GetComponent<bossController>().yvel = 0f;
            boss.GetComponent<bossController>().moveVelocity = 0f;
        }
        for(int i = 1; i < movables.Length; i++) {
            movables[i].gameObject.GetComponent<movableObjectController>().reset();
        }
        canMove = true;
        invincible = 0f;
    }

    IEnumerator DeathScreenFade()
    {
        blackscreenImg.color = new Color(0f, 0f, 0f, 1f);
        while (blackscreenImg.color.a > 0)
        {
            yield return new WaitForSeconds(0.1f);
            blackscreenImg.color = new Color(0f, 0f, 0f, blackscreenImg.color.a-0.1f);
        }
    }

   /* IEnumerator MusicCoroutine()
    {
        music.volume = 0.1f;
        if (music.volume < 0.5f)
        {
            yield return new WaitForSeconds(1.8f);
            music.volume += 0.05f;
            yield return new WaitForSeconds(1);
            music.volume += 0.05f;
            yield return new WaitForSeconds(1);
            music.volume += 0.05f;
            yield return new WaitForSeconds(1);
            music.volume += 0.05f;
            yield return new WaitForSeconds(1);
            music.volume += 0.05f;
            yield return new WaitForSeconds(1);
            music.volume += 0.05f;
            yield return new WaitForSeconds(1);
            music.volume += 0.05f;
            yield return new WaitForSeconds(1);
            music.volume += 0.05f;
            yield return new WaitForSeconds(1);
        }
    }*/
}