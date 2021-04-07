using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;

public class CutsceneManager : MonoBehaviour
{

    private int index;
    public GameObject[] cutscenes;
    public GameObject player;
    public bool cutsceneActive;
    public GameObject[] dialogboxes;
    public GameObject cutsceneMusic;
    public GameObject cutsceneActiveObj;
    public bool dialogueActive;

    public GameObject music;
    public CutsceneMusic cutsceneAudioManager;

    void Start()
    {
        index = 0;
        player = GameObject.Find("Player");
        
        foreach (GameObject g in cutscenes)
        {
            g.SetActive(false);
        }
        PlayNext();

        cutsceneAudioManager = GameObject.FindWithTag("Music2").GetComponent<CutsceneMusic>();
    }

    void FixedUpdate()
    {
        //check if any active
        cutsceneActive = cutsceneActiveObj.activeSelf;
        /*
        foreach (GameObject g in cutscenes)
        {
            cutsceneActive = cutsceneActive || g.activeSelf;
        }*/
        //disable player controller during cutscenes and dialogue
        dialogueActive = false;
        foreach (GameObject g in dialogboxes)
        {
            cutsceneActive = cutsceneActive || g.activeSelf;
        }
        // if (dialogueActive)
        // {
        //     player.GetComponent<PlayerController>().enabled = false;
        // } else if (cutsceneActive)
        // {
        //     player.GetComponent<PlayerController>().enabled = false;
        // } else
        if (dialogueActive || cutsceneActive){
            player.GetComponent<PlayerController>().enabled = false;
        } else {
            player.GetComponent<PlayerController>().enabled = true;
            //cutsceneMusic.SetActive(false);
        }

        if (!cutsceneActive)
        {
            cutsceneAudioManager.FadeVolume();
        } else if (cutsceneActive)
        {
            cutsceneAudioManager.IncreaseVolume();
        }
    }

    // Plays the next cutscene
    public void PlayNext()
    {
        //canDash is never really used
        if (index > 1) {
            player.GetComponent<PlayerController>().canDash = true;
        }
        cutsceneMusic.SetActive(true);
        if (index < cutscenes.Length)
        {
            cutscenes[index].SetActive(true);
            index++;
        }
        // } else {
        //     foreach (GameObject c in cutscenes) {
        //         c.SetActive(false);
        //     }
        // }
        
    }
}
