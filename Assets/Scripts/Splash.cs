

using System;
using System.Collections;
using MyGame;
using UnityEngine;

public class Splash : MonoBehaviour
{
    private void Start()
    {     
        GameManager.LoadScene("MainMenu");
    }
}