using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //allows this to be edited in inspector
public class Dialogue
{
    public string character;
    [TextArea(3,10)]
    public string[] sentences;
    

}
