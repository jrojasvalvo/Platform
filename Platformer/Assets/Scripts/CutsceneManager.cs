using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{

    private int index;
    public GameObject[] cutscenes;
    public GameObject player;
    private bool cutsceneActive;
    public GameObject dialogbox;

    void Start()
    {
        index = 0;
        player = GameObject.Find("Player");
        
        foreach (GameObject g in cutscenes)
        {
            g.SetActive(false);
        }
        PlayNext();
    }

    void Update()
    {
        //check if any active
        foreach (GameObject g in cutscenes)
        {
            cutsceneActive = cutsceneActive || g.activeSelf;
        }
        if (cutsceneActive || dialogbox.activeSelf)
        {
            player.GetComponent<PlayerController>().enabled = false;
        }
        else
        {
            player.GetComponent<PlayerController>().enabled = true;
        }

    }

    // Plays the next cutscene
    void PlayNext()
    {
        if (index < cutscenes.Length)
        {
            cutscenes[index].SetActive(true);
            index++;
        }
        
    }
}
