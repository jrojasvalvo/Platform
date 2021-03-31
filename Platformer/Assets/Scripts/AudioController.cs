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
    AudioSource music;
    public GameObject cutsceneManager;


    // Start is called before the first frame update
    void Start()
    {
        sounds = GetComponents<AudioSource>();
        jump1Sound = sounds[0];
        jump2Sound = sounds[1];
        dashSound = sounds[2];
        deathSound = sounds[3];
        music = sounds[4];
        cutsceneManager = GameObject.Find("CutsceneManager");
    }

    // Update is called once per frame
    void Update()
    {
        // deactivates music during cutscene
        if (cutsceneManager.GetComponent<CutsceneManager>().cutsceneActive)
        {
            music.volume -= 0.5f;
        }
        else if (deathSound.isPlaying == false)
        {
            if (music.volume < 0.5f)
            {
                music.volume += 0.005f;
            }
        }
    }
}
