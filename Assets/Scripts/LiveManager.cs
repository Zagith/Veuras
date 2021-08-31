using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class LiveManager : MonoBehaviour
{
    public static LiveManager instance;

    public List<LiveDTO> liveList = new List<LiveDTO>();

    public List<LiveInstanceDTO> LiveInstance = new List<LiveInstanceDTO>();

    public List<Image> LiveImages = new List<Image>();

    [Header("Instantiate Lives")]
    [SerializeField] GameObject livePrefab;
    [SerializeField] Transform listGB;

    [Header("Settings")]
    [SerializeField] string link = "";

    public List<GameObject> liveListGB = new List<GameObject>();

    private List<string> channelInitialize = new List<string>();

    // Start is called before the first frame update
    void Awake()
    {
        Instantiate();
    }

    void Start()
    {
        WebService.instance.GetLives();
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

    public void UpdateLiveList(JSONArray list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            LiveDTO live = new LiveDTO {
                LiveId = list[i].AsObject["LiveId"],
                Name = list[i].AsObject["Nome"],
                Description = list[i].AsObject["Descrizione"],
                Link = list[i].AsObject["Link"],
                LiveDate = DateTime.ParseExact(list[i].AsObject["Inizio"], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                CoverImage = list[i].AsObject["Copertina"]
            };
            LiveManager.instance.liveList.Add(live);
        }
        InitializeLives();
    }

    void InitializeLives()
    {
        foreach (LiveDTO live in liveList)
        {
            GameObject liveList1 = Instantiate(livePrefab);
            liveList1.transform.SetParent(listGB, false);

            UniWebView stream = liveList1.transform.GetChild(0).GetComponent<UniWebView>();
            stream.urlOnStart = $"{link}?link={live.Link}";
            liveList1.name = $"live_{live.Name}";
            liveListGB.Add(liveList1.gameObject);
            liveList1.SetActive(false);
            channelInitialize.Add(live.Name);
            LiveImages.Add(liveList1.GetComponent<CloseLive>().avatarLive);
        }
        ChatGui.instance.InitializeChannels(channelInitialize);
        CategoryManager.instance.InitializeCategory();
    }

    public void GoToLive(string name)
    {
        UIHandler.instance.LiveScreen(liveListGB.Where(n => n.name == $"live_{name}").FirstOrDefault());
        int liveId = LiveManager.instance.liveList.Where(s => s.Name == name).Select(n => n.LiveId).FirstOrDefault();
        if (!LiveManager.instance.LiveInstance.Where(s => s.LiveId == liveId).Any())
        {
            if (!CategoryManager.instance.isContinueToWatchActive)
                CategoryManager.instance.isContinueToWatchActiveInRun = true;
            UpdateLiveinstanceList(liveId);
        }
    }

    public void UpdateLiveinstanceList(JSONArray list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            LiveInstanceDTO liveinstance = new LiveInstanceDTO {
                LiveId = list[i].AsObject["LiveId"],
                UserId = list[i].AsObject["AccountId"]
            };
            LiveInstance.Add(liveinstance);
        }
        CategoryManager.instance.UpdateContinueToWatch();
    }

    public void UpdateLiveinstanceList(int liveId)
    {
        LiveInstanceDTO liveinstance = new LiveInstanceDTO {
            LiveId = liveId,
            UserId = AccountManager.instance.UserToken
        };
        LiveInstance.Add(liveinstance);
        CategoryManager.instance.UpdateContinueToWatch();
        WebService.instance.AddLiveInstance(liveId);
    }


}
