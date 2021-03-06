using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushController : MonoBehaviour
{
    GameObject[] player;
    PlayerController p;
    public bool pulling = false;
    bool pullKeyDown = false;
    Vector3 offset;
    GameObject box;

    public AudioClip pushSound;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        p = player[0].GetComponent<PlayerController>();

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pulling) {
            p.firstJump = false;
            p.secondJump = false;
            box.transform.position = p.transform.position - offset;
        } 

        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            pullKeyDown = true;
        } if (Input.GetKeyUp(KeyCode.LeftControl)) {
            pullKeyDown = false;
            pulling = false;
            p.anim.SetBool("pulling", false);
        }            
    }


    void OnTriggerEnter2D(Collider2D col) {
        if(col.tag == "Movable") {
            if (pullKeyDown) {
                pulling = true;
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(pushSound);
                }
                offset = p.transform.position - col.gameObject.transform.position;
                box = col.gameObject;
                p.firstJump = false;
            }
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        // only push if walking towards object
        if (col.tag == "Movable")
        {   
            if (pullKeyDown) {
                p.anim.SetBool("pulling", true);
                pulling = true;
                offset = p.transform.position - col.gameObject.transform.position;
                box = col.gameObject;
                p.firstJump = false;
            } else if (!pullKeyDown) {
                p.anim.SetBool("pulling", false);
                pulling = false;
            }
            
            if (p.movingLeft || p.movingRight)
            {
                p.anim.SetBool("pushing", true);
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(pushSound);
                }
            } else {
                p.anim.SetBool("pushing", false);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Movable")
        {
            p.anim.SetBool("pushing", false);
        }
    }    
}
