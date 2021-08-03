using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoryManager : MonoBehaviour
{
    public static CategoryManager instance;

    public GameObject incomingCategory;
    public GameObject categoryParent;

    [Header("Header Components")]
    public TMP_Text headerTitle;
    public TMP_Text headerDescription;
    public Image headerImage;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeCategory()
    {
        WebService.instance.GetLiveCategory();
    }

    public void InitializeHeaderLive()
    {
        List<LiveDTO> lives = LiveManager.instance.liveList;
        int rnd = Random.Range(0, lives.Count);

        headerTitle.text = lives[rnd].Name;
        headerDescription.text = lives[rnd].Description;
        headerImage.sprite = lives[rnd].Sprite;
        headerTitle.gameObject.GetComponent<Button>().onClick.AddListener(delegate { UIHandler.instance.LiveScreen(
            LiveManager.instance.liveListGB.Where(n => n.name == $"live_{headerTitle.text}").FirstOrDefault());});
    }

    public void UpdateContinueToWatch()
    {
        List<LiveInstanceDTO> liveList = LiveManager.instance.LiveInstance;
        CategoryManager.instance.categoryParent.transform.GetChild(0).gameObject.SetActive(true);
        if (liveList.Count > 0)
        {
            int pageCount = 1;
            int singlePageCount = 0;
            int itemPage = 0;
            CategoryAttributes categoryAttributes = CategoryManager.instance.categoryParent.transform.GetChild(0).GetComponent<CategoryAttributes>();
            GameObject livePrefab = null;
            GameObject listGB = categoryAttributes.viewPort;
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
                    liveListUI.liveImage[itemPage].GetComponent<Image>().sprite = LiveManager.instance.liveList.Where(s => s.LiveId == liveList[i].LiveId).Select(s => s.Sprite).First();
                    liveListUI.liveImage[itemPage].GetComponent<Image>().preserveAspect = true;
                    liveListUI.livesList[itemPage].name = $"{LiveManager.instance.liveList.Where(s => s.LiveId == liveList[i].LiveId).Select(s => s.Name).FirstOrDefault()}";
                    liveListUI.liveNameText[itemPage].text = $"{LiveManager.instance.liveList.Where(s => s.LiveId == liveList[i].LiveId).Select(s => s.Name).FirstOrDefault()}";

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
        }
    }
}
