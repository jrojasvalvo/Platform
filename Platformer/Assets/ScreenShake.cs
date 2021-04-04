using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float duration = 0.1f;
    public float magnitude = 0.1f;
    public float dampingSpeed = 2f;
    float mag;
    float dur;
    private Vector3 initialPosition;
    // private Vector3 playerOffset;


    private void OnEnable()
    {
        // playerOffset = gameObject.GetComponent<FollowPlayer>().offset;
        initialPosition = transform.localPosition;
        mag = 0;
        dur = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // playerOffset = gameObject.GetComponent<FollowPlayer>().offset;

        if (dur > 0)
        {
            transform.localPosition = transform.position + Random.insideUnitSphere * mag;

            dur -= Time.deltaTime * dampingSpeed;
        }
        else
        { 
            dur = 0f;
            if (!GetComponent<moveCamera>().canScroll) {
                initialPosition = transform.localPosition;
            }
            transform.localPosition = initialPosition;
        }
    }
    
    public void CameraShake()
    {
        dur = duration;
        mag = magnitude;
    }
}
