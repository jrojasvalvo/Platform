using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{
    GameObject[] player;
    // Start is called before the first frame update
    float x;
    float y;
    public bool canScroll;
    bool scrollingLeft = false;
    bool scrollingRight = false;
    bool scrollingDown = false;
    bool scrollingUp = false;
    public float cameraScrollSpeed;
    public int room;

    Camera cam;

    GameObject barrier;

    public float roomWidth;
    public float roomHeight;

    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        barrier = GameObject.FindGameObjectsWithTag("Barrier")[0];
        x = transform.position.x;
        y = transform.position.y;
        canScroll = true;
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
            //if scrolling freeze world movement
            Time.timeScale = 0;
            player[0].GetComponent<PlayerController>().canMove = false;
            barrier.transform.position = new Vector3(barrier.transform.position.x + roomWidth, barrier.transform.position.y, barrier.transform.position.z);
        } else if(player[0].transform.position.x < transform.position.x - (roomWidth / 2f) && canScroll) {
            canScroll = false;
            x -= roomWidth;
            scrollingLeft = true;
            room -= 1;
            //if scrolling freeze world movement
            Time.timeScale = 0;
            player[0].GetComponent<PlayerController>().canMove = false;
        }

        if(scrollingLeft) {
            float currX = transform.position.x;
            transform.position = new Vector3(currX - cameraScrollSpeed * Time.fixedDeltaTime, y, -10f);
            if(transform.position.x <= x) {
                transform.position = new Vector3(x, y, -10f);
                scrollingLeft = false;
                canScroll = true;
                //resume time when finish scrolling
                Time.timeScale = 1;
                player[0].GetComponent<PlayerController>().canMove = true;
            }
        }

        if(scrollingRight) {
            float currX = transform.position.x;
            transform.position = new Vector3(currX + cameraScrollSpeed * Time.fixedDeltaTime, y, -10f);
            if(transform.position.x >= x) {
                transform.position = new Vector3(x, y, -10f);
                scrollingRight = false;
                canScroll = true;
                //resume time when finish scrolling
                Time.timeScale = 1;
                player[0].GetComponent<PlayerController>().canMove = true;
            }
        }

        if(player[0].transform.position.y < -5f) {
            player[0].GetComponent<PlayerController>().resetRoom();
        }

        if(player[0].transform.position.y > transform.position.y + (roomHeight / 2f + 1f) && canScroll) {
            canScroll = false;
            y += roomHeight;
            scrollingUp = true;
            Time.timeScale = 0;
            player[0].GetComponent<PlayerController>().canMove = false;
        } else if(player[0].transform.position.y < transform.position.y - (roomHeight / 2f + 1f) && canScroll) {
            canScroll = false;
            y -= roomHeight;
            scrollingDown = true;
            Time.timeScale = 0;
            player[0].GetComponent<PlayerController>().canMove = false;
        }

        if(scrollingDown) {
            float currY = transform.position.y;
            transform.position = new Vector3(x, currY - cameraScrollSpeed * Time.fixedDeltaTime, -10f);
            if(transform.position.y <= y) {
                transform.position = new Vector3(x, y, -10f);
                scrollingDown = false;
                canScroll = true;
                Time.timeScale = 1;
                player[0].GetComponent<PlayerController>().canMove = true;
            }
        }

        if(scrollingUp) {
            float currY = transform.position.y;
            transform.position = new Vector3(x, currY + cameraScrollSpeed * Time.fixedDeltaTime, -10f);
            if(transform.position.y >= y) {
                transform.position = new Vector3(x, y, -10f);
                scrollingUp = false;
                canScroll = true;
                Time.timeScale = 1;
                player[0].GetComponent<PlayerController>().canMove = true;
            }
        }
    }
}
