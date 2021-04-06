using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class marker : MonoBehaviour
{
    public float rotationSpeed;
    public float radius;

    float angle;

    GameObject[] player;
    PlayerController p;
    SpriteRenderer s;
    public string type;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        p = player[0].GetComponent<PlayerController>();
        s = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(type == "dash") {
            //had p.canDash, but doesn't look like its being used anywhere
            if(p.canDash && p.dashReset) {
                s.enabled = true;
            } else {
                s.enabled = false;
            }
        } else if(type == "doubleJump") {
            if(p.canDoubleJump && (p.secondJump || p.firstJump)) {
                s.enabled = true;
            } else {
                s.enabled = false;
            }
        }

        angle += rotationSpeed * Time.deltaTime;

        Vector3 move = new Vector3(Mathf.Sin(angle) * radius, Mathf.Cos(angle) * radius, 0);
        Vector3 relative = new Vector3(player[0].transform.position.x, player[0].transform.position.y + 1f, 0);
        transform.position = relative + move;
        
    }
}
