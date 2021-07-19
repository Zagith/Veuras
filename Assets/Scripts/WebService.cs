using SimpleJSON;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebService : MonoBehaviour
{
    public static WebService instance;
    protected string _webHost = "https://veruas.enjinia.it/webapi/";

    public string WebHost 
    {
        get {
            return _webHost;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        Instantiate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Instantiate()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }
}
