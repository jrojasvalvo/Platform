﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadHub()
    {
        SceneManager.LoadScene("StartingScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
