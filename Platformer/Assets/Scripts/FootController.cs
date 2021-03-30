using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FootController : MonoBehaviour
{
    GameObject[] player;
    PlayerController p;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        p = player[0].GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col) {
        if((col.tag == "Platform" || col.tag == "Movable") && this.tag == "Foot") {
            p.firstJump = true;
            p.secondJump = true;
            p.isTouchingPlat = true;
            p.dashReset = true;
        }
    }

    void OnTriggerStay2D(Collider2D col) {
        if((col.tag == "Platform" || col.tag == "Movable") && this.tag == "Foot") {
            p.firstJump = true;
            p.secondJump = true;
            p.isTouchingPlat = true;
            p.dashReset = true;
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if(col.tag == "Platform" || col.tag == "Movable") {
            p.isTouchingPlat = false;
            //p.firstJump = false;
            StartCoroutine(p.coyoteTime());
        }
    }
}
