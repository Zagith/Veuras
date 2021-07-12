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
    protected string _webHost = "http://lozioconsiglia.it/vseum/";

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

    public void GetUserData(string userId)
    {
        StartCoroutine(getUserData(userId));
    }

    private IEnumerator getUserData(string userId)
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{WebHost}getuserdata.php?uid={userId}"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                Debug.Log(www.error);
            else
            {
                //Or retrieve results as binary data
                string results = www.downloadHandler.text;

                JSONArray jsonArray = JSON.Parse(Regex.Replace(results, @"\s+", "")) as JSONArray;
                AccountManager.instance.UpdateUserData(jsonArray[0].AsObject["Name"], jsonArray[0].AsObject["Surname"]);
            }
        }
    }
}
