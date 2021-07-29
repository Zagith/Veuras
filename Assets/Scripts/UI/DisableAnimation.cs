using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAnimation : MonoBehaviour
{
    public void ChangePage()
    {
        UIHandler.instance.ShowTransitionDrag();
        if (PlayerPrefs.GetString("Email") != "")
        {
            AccountManager.instance.InitializeUserAutoLogin();
            UIHandler.instance.HomeScreen();
            UIHandler.instance.DockBarVisibility(true);
        }
        else
            UIHandler.instance.MethodLoginScreen();
    }
}
