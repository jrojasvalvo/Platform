using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class doorController : MonoBehaviour
{
    public bool locked;
    public int doorNum;
    public GameObject lockedDialogue;

    bool canOpen;
    // Start is called before the first frame update
    void Start()
    {
        lockedDialogue.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            lockedDialogue.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.E) && canOpen) {
            if(!locked) {
                SceneManager.LoadScene("SampleScene");
            } else {
                lockedDialogue.SetActive(true);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if(col.tag == "Player") {
            canOpen = true;
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if(col.tag == "Player") {
            canOpen = false;
        }
    }
}
