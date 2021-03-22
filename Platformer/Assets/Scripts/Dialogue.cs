using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    [TextArea(3, 10)]
    public string[] sentences;
    private int index;
    public float typingSpeed;
    public bool sentenceDone;

    void Start()
    {
        StartCoroutine(Type());
        index = 0;
    }

    IEnumerator Type()
    {
        sentenceDone = false;
        foreach (char letter in sentences[index].ToCharArray())
        {
            if (sentenceDone) break;
            textDisplay.text += letter;
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
            StartCoroutine(Type());
        }
        else
        {
            textDisplay.text = "";
            //add some way to deactivate all dialogue box elements 
        }
    }
}
