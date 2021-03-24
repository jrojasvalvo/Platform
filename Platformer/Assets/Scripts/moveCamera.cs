using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{
    GameObject[] player;
    // Start is called before the first frame update
    float x;
    float y;
    bool canScroll;
    bool scrollingLeft = false;
    bool scrollingRight = false;
    bool scrollingDown = false;
    bool scrollingUp = false;
    public float cameraScrollSpeed;
    public int room;

    Camera cam;

    public float roomWidth;
    public float roomHeight;

    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        x = transform.position.x;
        y = transform.position.y;
        canScroll = true;
        room = -1;
        cam = gameObject.GetComponent<Camera>();
        roomWidth = cam.aspect * cam.orthographicSize * 2;
        roomHeight = roomWidth / cam.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        if(player[0].transform.position.x > transform.position.x + (roomWidth / 2f) && canScroll) {
            canScroll = false;
            x += roomWidth;
            scrollingRight = true;
            room += 1;
        } else if(player[0].transform.position.x < transform.position.x - (roomWidth / 2f) && canScroll) {
            canScroll = false;
            x -= roomWidth;
            scrollingLeft = true;
            room -= 1;
        }

        if(scrollingLeft) {
            float currX = transform.position.x;
            transform.position = new Vector3(currX - cameraScrollSpeed, y, -10f);
            if(transform.position.x <= x) {
                transform.position = new Vector3(x, y, -10f);
                scrollingLeft = false;
                canScroll = true;
            }
        }

        if(scrollingRight) {
            float currX = transform.position.x;
            transform.position = new Vector3(currX + cameraScrollSpeed, y, -10f);
            if(transform.position.x >= x) {
                transform.position = new Vector3(x, y, -10f);
                scrollingRight = false;
                canScroll = true;
            }
        }

        if(player[0].transform.position.y < -5f) {
            player[0].GetComponent<PlayerController>().resetRoom();
        }

        /*if(player[0].transform.position.y > transform.position.y + (roomHeight / 2f) && canScroll) {
            canScroll = false;
            y += roomHeight;
            scrollingUp = true;
            room += 1;
        } else if(player[0].transform.position.y < transform.position.y - (roomHeight / 2f) && canScroll) {
            canScroll = false;
            y -= roomHeight;
            scrollingDown = true;
            room -= 1;
        }

        if(scrollingDown) {
            float currY = transform.position.y;
            transform.position = new Vector3(x, currY - cameraScrollSpeed, -10f);
            if(transform.position.y <= y) {
                transform.position = new Vector3(x, y, -10f);
                scrollingLeft = false;
                canScroll = true;
            }
        }

        if(scrollingUp) {
            float currY = transform.position.y;
            transform.position = new Vector3(x, currY + cameraScrollSpeed, -10f);
            if(transform.position.y >= y) {
                transform.position = new Vector3(x, y, -10f);
                scrollingRight = false;
                canScroll = true;
            }
        }*/
    }
}
