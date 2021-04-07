using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneMusic : MonoBehaviour
{
    public AudioSource music;

    // Start is called before the first frame update
    void Start()
    {
        music = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayMusic()
    {
        music.Play();
    }

    public void FadeVolume()
    {
        music.volume -= 0.005f;
    }

    public void IncreaseVolume()
    {
        if (music.volume < 0.4f)
        {
            music.volume += 0.005f;
        }
    }
}
