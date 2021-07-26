using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class LiveManager : MonoBehaviour
{
    public static LiveManager instance;

    public List<LiveDTO> liveList = new List<LiveDTO>();

    [Header("Instantiate Lives")]
    [SerializeField] GameObject livePrefab;
    [SerializeField] Transform listGB;

    [Header("Settings")]
    [SerializeField] string link = "";

    public List<GameObject> liveListGB = new List<GameObject>();

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
            liveList1.name = $"live {live.Name}";
            liveListGB.Add(liveList1.gameObject);
            liveList1.SetActive(false);
        }
        CategoryManager.instance.InitializeCategory();
    }

    public void GoToLive(string name)
    {
        liveListGB.Where(n => n.name == $"live {name}").FirstOrDefault().SetActive(true);
    }
}
