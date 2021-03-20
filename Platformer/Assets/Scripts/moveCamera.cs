﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{
    GameObject[] player;
    // Start is called before the first frame update
    float x;
    bool canScroll;
    bool scrollingLeft = false;
    bool scrollingRight = false;
    public float cameraScrollSpeed;
    public int room;

    Camera cam;

    public float roomWidth;

    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        x = transform.position.x;
        canScroll = true;
        room = 0;
        cam = gameObject.GetComponent<Camera>();
        roomWidth = cam.aspect * cam.orthographicSize * 2;
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
            transform.position = new Vector3(currX - cameraScrollSpeed, 0f, -10f);
            if(transform.position.x <= x) {
                transform.position = new Vector3(x, 0f, -10f);
                scrollingLeft = false;
                canScroll = true;
            }
        }

        if(scrollingRight) {
            float currX = transform.position.x;
            transform.position = new Vector3(currX + cameraScrollSpeed, 0f, -10f);
            if(transform.position.x >= x) {
                transform.position = new Vector3(x, 0f, -10f);
                scrollingRight = false;
                canScroll = true;
            }
        }
    }
}