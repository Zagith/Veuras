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

    public void GetLives()
    {
        StartCoroutine(getLives());
    }
    private IEnumerator getLives()
    {
        using (UnityWebRequest w = UnityWebRequest.Get($"{WebService.instance.WebHost}getlives.php"))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError(w.error);
            }
            
            string results = w.downloadHandler.text;
            JSONArray jsonArray = JSON.Parse(Regex.Replace(results, @"\s+", " ")) as JSONArray;
            LiveManager.instance.UpdateLiveList(jsonArray);
        }
    }
}
