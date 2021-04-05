using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FootController : MonoBehaviour
{
    GameObject[] player;
    GameObject boss;
    PlayerController p;
    bossController b;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        p = player[0].GetComponent<PlayerController>();
        boss = GameObject.FindGameObjectWithTag("Boss");
        b = boss.GetComponent<bossController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col) {
        if(col.tag == "Platform" || col.tag == "Movable") {
            if (this.tag == "Foot") {
                p.firstJump = true;
                p.secondJump = true;
                p.isTouchingPlat = true;
                p.dashReset = true;
            } else if (this.tag == "BossFoot") {
                b.firstJump = true;
                b.secondJump = true;
                b.isTouchingPlat = true;
                b.dashReset = true;
            }
        }
    }

    void OnTriggerStay2D(Collider2D col) {
        if (this.tag == "Foot") {
                p.firstJump = true;
                p.secondJump = true;
                p.isTouchingPlat = true;
                p.dashReset = true;
            } else if (this.tag == "BossFoot") {
                b.firstJump = true;
                b.secondJump = true;
                b.isTouchingPlat = true;
                b.dashReset = true;
            }
    }

    void OnTriggerExit2D(Collider2D col) {
        if(col.tag == "Platform" || col.tag == "Movable") {
            if (this.tag == "Foot") {
                p.isTouchingPlat = false;
                StartCoroutine(p.coyoteTime());
            } else if (this.tag == "BossFoot") {
                b.isTouchingPlat = false;
                StartCoroutine(b.coyoteTime());
            }
        }
        
    }
}
