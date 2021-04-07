using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTracker : MonoBehaviour
{
    public bool dash;
    public bool doubleJump;
    public bool wallJump;
    // Start is called before the first frame update
    void Start()
    {
        dash = false;
        doubleJump = false;
        wallJump = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }
}
