using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushController : MonoBehaviour
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

    void OnTriggerStay2D(Collider2D col)
    {
        // only push if walking towards object
        if (col.tag == "Movable")
        {
            if (p.movingLeft || p.movingRight)
            {
                p.anim.SetBool("pushing", true);
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
