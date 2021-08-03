using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAnimation : MonoBehaviour
{
    public static DisableAnimation instance;

    void Awake()
    {
        instance = this;
    }
    public void ChangePage()
    {
        if (AccountManager.instance.CanAutoLogin())
        {
            AccountManager.instance.InitializeUserAutoLogin();
            UIHandler.instance.HomeScreen();
        }
        else
            UIHandler.instance.MethodLoginScreen();
    }
}
