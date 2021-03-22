using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textDisplay1;
    public TextMeshProUGUI textDisplay2;
    TextMeshProUGUI textDisplay;
    [TextArea(3, 10)]
    public string[] sentences;
    private int index;
    public float typingSpeed;
    public bool sentenceDone;
    public bool sethActive;
    public bool noahActive;
    public Image Noah;
    public Image Seth;

    void Start()
    {
        textDisplay = textDisplay1;
        StartCoroutine(Type(textDisplay));
        index = 0;
        
        sethActive = true;
        noahActive = false;
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
        if (sethActive) {
            Seth.gameObject.SetActive(true);
            textDisplay = textDisplay1;
            Noah.gameObject.SetActive(false);
        } else if(noahActive) {
            Noah.gameObject.SetActive(true);
            textDisplay = textDisplay2;
            Seth.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            Progress();
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
            textDisplay.text = "";
            StartCoroutine(Type(textDisplay));
        }
        else
        {
            textDisplay.text = "";
            //add some way to deactivate all dialogue box elements 
        }
    }
}
