using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movableObjectController : MonoBehaviour
{
    // Start is called before the first frame update
    float x;
    float y;


    void Start()
    {
        x = transform.position.x;
        y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -5f) {
            reset();
        }
    }

    public void reset() {
        transform.position = new Vector3(x, y, 0f);
    }
}
