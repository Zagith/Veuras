using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAnimation : MonoBehaviour
{
    public static DisableAnimation instance;

    public GameObject loader;

    void Awake()
    {
        instance = this;
    }
    public void ChangePage()
    {
        if (AccountManager.instance.CanAutoLogin())
        {
            UIHandler.instance.HomeScreen();
        }
        else
            UIHandler.instance.MethodLoginScreen();
    }

    public void VisibleLoading()
    {
        loader.SetActive(true);
    }
}
