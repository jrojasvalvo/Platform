using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    GameObject boss;
    GameObject player;

    void Start() {
        boss = GameObject.Find("Boss");
        player = GameObject.Find("Player");
    }
}
