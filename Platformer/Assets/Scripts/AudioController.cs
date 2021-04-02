using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    AudioSource[] sounds;
    AudioSource jump1Sound;
    AudioSource jump2Sound;
    AudioSource dashSound;
    AudioSource deathSound;
    //AudioSource music;
    public GameObject cutsceneManager;

    public GameObject music;
    public AudioManager audioManager;


    // Start is called before the first frame update
    void Start()
    {
        sounds = GetComponents<AudioSource>();
        jump1Sound = sounds[0];
        jump2Sound = sounds[1];
        dashSound = sounds[2];
        deathSound = sounds[3];
       // music = sounds[4];
        cutsceneManager = GameObject.Find("CutsceneManager");
        audioManager = GameObject.FindWithTag("Music").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cutsceneManager.activeSelf)
        {
            // deactivates music during cutscene
            if (cutsceneManager.GetComponent<CutsceneManager>().cutsceneActive)
            {
                audioManager.music.volume = 0;
            }
            else if (deathSound.isPlaying == false)
            {
                if (audioManager.music.volume < 0.5f)
                {
                    audioManager.music.volume += 0.005f;
                }
            }
        }
    }
}
