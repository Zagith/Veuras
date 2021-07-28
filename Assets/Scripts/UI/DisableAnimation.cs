using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAnimation : MonoBehaviour
{
    public void ChangePage()
    {
        if (PlayerPrefs.GetString("Email") != null)
        {
            AccountManager.instance.InitializeUserAutoLogin();
            UIHandler.instance.HomeScreen();
        }
        else
            UIHandler.instance.MethodLoginScreen();
    }
}
