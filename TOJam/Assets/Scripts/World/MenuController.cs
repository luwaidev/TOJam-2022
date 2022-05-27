using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public bool menuMode;
    public void Play()
    {

        GameManager.instance.Load("Level 1");
    }

    public void Exit()
    {
        GameManager.instance.Exit();
    }

    public void ToggleMenu()
    {
        menuMode = !menuMode;
        GetComponent<Animator>().Play(!menuMode ? "Main Menu" : "Settings");
    }

}
