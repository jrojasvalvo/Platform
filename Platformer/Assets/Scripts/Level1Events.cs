using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Events : MonoBehaviour
{
    GameObject[] camera;
    Camera cam;
    bool scene1;
    GameObject[] player;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindGameObjectsWithTag("MainCamera");
        player = GameObject.FindGameObjectsWithTag("Player");
        cam = camera[0].GetComponent<Camera>();
        scene1 = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(cam.GetComponent<moveCamera>().room == 2) {
            scene1 = true;
        }

        if(scene1) {
            
        }
    }
}
