using SimpleJSON;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    public void GetLiveCategory()
    {
        StartCoroutine(getLiveCategory());
    }

    private IEnumerator getLiveCategory()
    {
        int pageCount = 1;
        int singlePageCount = 0;
        int itemPage = 0;
        CategoryAttributes categoryAttributes = CategoryManager.instance.categoryParent.transform.GetChild(0).GetComponent<CategoryAttributes>();
        GameObject livePrefab = null;
        GameObject listGB = categoryAttributes.viewPort;
        List<LiveDTO> liveList = LiveManager.instance.liveList;
        CategoryPrefab liveListUI = null;
        if (listGB.transform.childCount > 0)
        {
            for (int p = 0; p < listGB.transform.childCount; p++)
            {
                Destroy(listGB.transform.GetChild(p).gameObject);
            }
        }
        for (int i = 0; i < liveList.Count; i++)
        {
            if (singlePageCount == 0)
            {
                livePrefab = Instantiate(categoryAttributes.prefabLive);

                livePrefab.transform.SetParent(listGB.transform, false);

                liveListUI = livePrefab.GetComponent<CategoryPrefab>();

                pageCount++;
            }

            if (liveListUI != null)
            {
                using (UnityWebRequest wwww = UnityWebRequest.Get($"{WebService.instance.WebHost}getlivecover.php?path={liveList[i].CoverImage}"))
                {
                    yield return wwww.SendWebRequest();

                    if (wwww.isNetworkError || wwww.isHttpError)
                        Debug.Log(wwww.error);
                    else
                    {
                        byte[] bytes = wwww.downloadHandler.data;
                        Texture2D texture = new Texture2D(2,2);
                        texture.LoadImage(bytes);
                        Sprite sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), new Vector2(.5f,.5f));   
                        
                        liveListUI.liveImage[itemPage].GetComponent<Image>().sprite = sprite;
                        liveList[i].Sprite = sprite;
                        liveListUI.liveImage[itemPage].GetComponent<Image>().preserveAspect = true;
                    }
                }
                liveListUI.livesList[itemPage].name = $"{liveList[i].Name}";
                liveListUI.liveNameText[itemPage].text = liveList[i].Name;

            }

            if (singlePageCount == 1)
            {
                singlePageCount = 0;
                itemPage = 0;
            }
            else if (i < liveList.Count)
            {
                itemPage++;
                singlePageCount++;
                Debug.Log($"aaaa {singlePageCount}"); 
            }
        }
        if (itemPage != 2)
        {
            switch (itemPage)
            {
                case 1:
                    Destroy(liveListUI.livesList[1]);
                    // categoryAttributes.blurGB.SetActive(false);
                break;
            }
        }
        categoryAttributes.scrollSnapRect.InitializeScroll(container: categoryAttributes.viewPort.GetComponent<RectTransform>());
        CategoryManager.instance.InitializeHeaderLive();
        DockManager.instance.UpdateDockBar();
    }
}
