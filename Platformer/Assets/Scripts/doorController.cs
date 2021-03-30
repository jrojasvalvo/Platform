using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class doorController : MonoBehaviour
{
    public bool locked;
    public int doorNum;
    public GameObject lockedDialogue;
    public GameObject e;

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
            if(!locked && doorNum == 1) {
                SceneManager.LoadScene("Level1");
            } else if(!locked && doorNum == 11) {
                SceneManager.LoadScene("HubLevel");
            } else if(!locked && doorNum == 2) {
                SceneManager.LoadScene("Level2");
            }
            else {
                lockedDialogue.SetActive(true);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if(col.tag == "Player") {
            canOpen = true;
            e.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if(col.tag == "Player") {
            canOpen = false;
            e.SetActive(false);
        }
    }
}
