using SimpleJSON;
using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
        PlayerPrefs.SetString("AvatarLink", "10.jpg");
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

    public void GetProfileAvatar(string link)
    {
        StartCoroutine(getProfileAvatar(link));
    }

    private IEnumerator getProfileAvatar(string link)
    {
        using (UnityWebRequest wwww = UnityWebRequest.Get($"{WebService.instance.WebHost}getavatar.php?path={link}"))
        {
            yield return wwww.SendWebRequest();

            if (wwww.isNetworkError || wwww.isHttpError)
                Debug.Log(wwww.error);
            else
            {
                string result = wwww.downloadHandler.text;
                byte[] bytes = wwww.downloadHandler.data;
                Debug.Log($"avatar link: {bytes.Length}");
                Texture2D texture = new Texture2D(2,2);
                texture.LoadImage(bytes);
                Sprite sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), new Vector2(.5f,.5f));   
                        
                AccountManager.instance.UpdateProfileAvatar(sprite);
            }
        }
    }

    public void GetAllProfileAvatar()
    {
        StartCoroutine(getAllProfileAvatar());
    }

    private IEnumerator getAllProfileAvatar()
    {
        using (UnityWebRequest wwww = UnityWebRequest.Get($"{WebService.instance.WebHost}getallavatar.php?"))
        {
            yield return wwww.SendWebRequest();

            if (wwww.isNetworkError || wwww.isHttpError)
                Debug.Log(wwww.error);
            else
            {
                string result = wwww.downloadHandler.text;
                byte[] bytes = wwww.downloadHandler.data;
                Debug.Log($"avatar link: {bytes.Length}");
                Texture2D texture = new Texture2D(2,2);
                texture.LoadImage(bytes);
                Sprite sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), new Vector2(.5f,.5f));   
                        
                AccountManager.instance.UpdateProfileAvatar(sprite);
            }
        }
    }

    // Get all lives
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

    // Get live category
    public void GetLiveCategory()
    {
        StartCoroutine(getLiveCategory());
    }

    private IEnumerator getLiveCategory()
    {
        int pageCount = 1;
        int singlePageCount = 0;
        int itemPage = 0;
        CategoryAttributes categoryAttributes = CategoryManager.instance.categoryParent.transform.GetChild(1).GetComponent<CategoryAttributes>();
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
        if (AccountManager.instance.CanAutoLogin())
        {
            AccountManager.instance.InitializeUserAutoLogin();
        }
        GetCategoryList();
        CategoryManager.instance.InitializeHeaderLive();
        DockManager.instance.UpdateDockBar();
        DisableAnimation.instance.ChangePage();
    }

    // Get whatched lives
    public void GetWatchedLives()
    {
        StartCoroutine(getWatchedLives());
    }
    private IEnumerator getWatchedLives()
    {
        Debug.Log(AccountManager.instance.UserToken);
        using (UnityWebRequest w = UnityWebRequest.Get($"{WebService.instance.WebHost}getliveinstance.php?uid={AccountManager.instance.UserToken}"))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError(w.error);
            }
            
            string results = w.downloadHandler.text;
            Debug.Log($"STATUS {results}");
            JSONArray jsonArray = JSON.Parse(Regex.Replace(results, @"\s+", " ")) as JSONArray;

            LiveManager.instance.UpdateLiveinstanceList(jsonArray);
        }
    }

    public void GetCategoryList()
    {
        StartCoroutine(getCategoryList());
    }
    private IEnumerator getCategoryList()
    {
        using (UnityWebRequest w = UnityWebRequest.Get($"{WebService.instance.WebHost}getcategorylist.php"))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError(w.error);
            }
            
            string results = w.downloadHandler.text;
            JSONArray jsonArray = JSON.Parse(Regex.Replace(results, @"\s+", " ")) as JSONArray;

            for (int i = 0; i < jsonArray.Count; i++)
            {
                CategoryDTO category = new CategoryDTO {
                    CategoryId = jsonArray[i].AsObject["CategoryId"],
                    Name = jsonArray[i].AsObject["Nome"]
                };
                CategoryManager.instance.categoryList.Add(category);
            }
            StartCoroutine(getCategoryLive());
        }
    }
    private IEnumerator getCategoryLive()
    {
        using (UnityWebRequest ww = UnityWebRequest.Get($"{WebService.instance.WebHost}getlivecategory.php"))
        {
            yield return ww.SendWebRequest();
            if (ww.isNetworkError || ww.isHttpError)
            {
               Debug.LogError(ww.error);
            }
                
            string results = ww.downloadHandler.text;
            JSONArray jsonArray = JSON.Parse(Regex.Replace(results, @"\s+", " ")) as JSONArray;

            for (int i = 0; i < jsonArray.Count; i++)
            {
                LiveCategoryDTO category = new LiveCategoryDTO {
                    CategoryId = jsonArray[i].AsObject["CategoryId"],
                    LiveName = jsonArray[i].AsObject["LiveName"]
                };
                CategoryManager.instance.liveCategoryList.Add(category);
            }
            CategoryManager.instance.UpdateCategory();
        }
    }

    // Add whatched live
    public void AddLiveInstance(int liveId)
    {
        StartCoroutine(addLiveInstance(liveId));
    }
    private IEnumerator addLiveInstance(int liveId)
    {
        WWWForm form = new WWWForm();
        form.AddField("LiveId", liveId);
        form.AddField("UserId", AccountManager.instance.UserToken);
            
        using (UnityWebRequest w = UnityWebRequest.Post($"{WebService.instance.WebHost}addliveinstance.php", form))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError(w.error);
            }
        }
    }

    public void UploadProfileImage(Texture2D texture)
    {
        StartCoroutine(uploadProfileImage(texture));
    }

    private IEnumerator uploadProfileImage(Texture2D texture)
    {
        WWWForm form = new WWWForm ();

        //You can then load it to a texturev
        Debug.Log($"encod:");
        // Texture2D tex = new Texture2D(texture.width,texture.height, texture.format, texture.mipmapCount);
        // Texture2D tex = 
        byte[] bytes = texture.EncodeToJPG();
        
        // Destroy ( texture );
        // foreach (byte bytel in bytes)
        // {
        //     Debug.Log($"com: {bytel}");
        // }
		//image file extension
        string fileName = $"{AccountManager.instance.UserToken}";
        form.AddField ( "action", "Upload Image" );
        form.AddField ("UserId", fileName);
		form.AddBinaryData ("fileUpload", bytes, fileName + ".jpg", "image/jpg");

        using (UnityWebRequest wwww = UnityWebRequest.Post($"{WebService.instance.WebHost}/test.php", form))
        {
            yield return wwww.SendWebRequest();

            if (wwww.isNetworkError || wwww.isHttpError)
                Debug.Log(wwww.error);
            else
            {
                string result = wwww.downloadHandler.text;
                Debug.Log(result);
                AccountManager.instance.UpdateProfileAvatar();
                PlayerPrefs.SetString("AvatarLink", $"{fileName}.jpg");
            }
        }
    }
}