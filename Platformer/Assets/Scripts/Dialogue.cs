using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textDisplay1;
    public TextMeshProUGUI textDisplay2;
    TextMeshProUGUI textDisplay;
    [TextArea(3, 10)]
    public string[] sentences;
    public string[] characters; //who is saying what sentence
    private int index;
    public float typingSpeed;
    public bool sentenceDone;
    public bool sethActive;
    public bool noahActive;
    public Image Noah;
    public Image Seth;
    public TimelineAsset timeline;
    public PlayableDirector playableDirector;
    public GameObject dialogbox;
    public GameObject noahsprite;

    AudioSource textSound;

    void Start()
    {
        index = 0;
        if (characters[index] == "Seth")
        {
            sethActive = true;
            noahActive = false;
        }
        if (characters[index] == "Noah")
        {
            noahActive = true;
            sethActive = false;
        }
        if (sethActive)
        {
            Seth.gameObject.SetActive(true);
            textDisplay = textDisplay1;
            Noah.gameObject.SetActive(false);
        }
        else if (noahActive)
        {
            Noah.gameObject.SetActive(true);
            textDisplay = textDisplay2;
            Seth.gameObject.SetActive(false);
        }
        StartCoroutine(Type(textDisplay));

        textSound = GetComponent<AudioSource>();
    }

    IEnumerator Type(TextMeshProUGUI t)
    {
        sentenceDone = false;
        foreach (char letter in sentences[index].ToCharArray())
        {
            if (sentenceDone) break;
            t.text += letter;
            yield return new WaitForSeconds(typingSpeed); //default 0.02f
        }
        sentenceDone = true;
    }

    void Update()
    {
        /*
        if (sethActive) {
            Seth.gameObject.SetActive(true);
            textDisplay = textDisplay1;
            Noah.gameObject.SetActive(false);
        } else if(noahActive) {
            Noah.gameObject.SetActive(true);
            textDisplay = textDisplay2;
            Seth.gameObject.SetActive(false);
        }
        */
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            Progress();
        }

        if (sentenceDone == false)
        {
            textSound.Play();
        } else
        {
            textSound.Stop();
        }
    }
    public void Progress()
    {
        if (!sentenceDone && index < sentences.Length)
        {
            textDisplay.text = "";
            textDisplay.text = sentences[index];
            sentenceDone = true;
        }
        else if (index < sentences.Length - 1)
        {
            index++;
            if (characters[index] == "Seth")
            {
                textDisplay1.text = "";
                Seth.gameObject.SetActive(true);
                textDisplay = textDisplay1;
                Noah.gameObject.SetActive(false);
            }
            if (characters[index] == "Noah")
            {
                textDisplay2.text = "";
                Noah.gameObject.SetActive(true);
                textDisplay = textDisplay2;
                Seth.gameObject.SetActive(false);
            }
            textDisplay.text = "";
            StartCoroutine(Type(textDisplay));
        }
        else
        {
            textDisplay.text = "";
            playableDirector.Stop();
            noahsprite.SetActive(false);
            dialogbox.SetActive(false);

            //add some way to deactivate all dialogue box elements 
        }
    }
}
