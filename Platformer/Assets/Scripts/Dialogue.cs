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
    //public PlayableDirector playableDirector;
    public GameObject dialogbox;
    public GameObject noahsprite;
    public bool hasCutscene;
    public GameObject player;

    public GameObject cutsceneManager;
    AudioSource textSound;

    void OnEnable()
    {
        cutsceneManager = GameObject.Find("CutsceneManager"); 
        player.GetComponent<PlayerController>().enabled = false;
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
        textSound = GetComponent<AudioSource>();
        textDisplay.text = "";
        StartCoroutine(Type(textDisplay));
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
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            Progress();
        }

        if (!sentenceDone)
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
            if (hasCutscene)
            {
                //playableDirector.Stop();
                //noahsprite.SetActive(false);
                player.GetComponent<PlayerController>().facingRight = true;
                //to play next cutscene upon event do cutsceneManager.GetComponent<CutsceneManager>().PlayNext();
            }
            player.GetComponent<PlayerController>().enabled = true;
            dialogbox.SetActive(false);
        }
    }
}
