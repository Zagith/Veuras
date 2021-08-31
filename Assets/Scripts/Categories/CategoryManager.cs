using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using TMPro;

public class CategoryManager : MonoBehaviour
{
    public static CategoryManager instance;

    public GameObject categoryPrefab;
    public GameObject categoryParent;

    public RectTransform ContentCategory;

    [Header("Header Components")]
    public TMP_Text headerTitle;
    public TMP_Text headerDescription;
    public Image headerImage;

    public List<CategoryDTO> categoryList = new List<CategoryDTO>();
    public List<LiveCategoryDTO> liveCategoryList = new List<LiveCategoryDTO>();

    private bool isContinueToWatchActive = false;

    public bool isContinueToWatchActiveInRun = false;

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

    public void UpdateCategory()
    {
        if (categoryList.Count > 0 && liveCategoryList.Count > 0)
        {
            CategoryAttributes categoryAttributes;
            float offset = 0f;
            for (int c = 0; c < categoryList.Count; c++)
            {
                List<LiveCategoryDTO> categoryLive = liveCategoryList.Where(l => l.CategoryId == categoryList[c].CategoryId).ToList();
                if (categoryLive.Any()){
                    if (!isContinueToWatchActive)
                    {
                        if (c > 0)
                        {
                            offset -= 600f;
                        }
                    }
                    else
                    {
                        offset -= 600f;
                    }
                    Debug.Log($"entro in c {c}");
                    // categoryParent.GetComponent<VerticalLayoutGroup>().spacing += 400;
                    // Initialize Category
                    GameObject categoryPrefabs = Instantiate(categoryPrefab);
                    categoryPrefabs.transform.SetParent(categoryParent.transform, false);
                    categoryAttributes = categoryPrefabs.GetComponent<CategoryAttributes>();
                    Debug.Log($"pageCount {categoryAttributes.viewPort.transform.parent.parent.name}");
                    categoryAttributes.categoryName.text = categoryList[c].Name;

                    // Initialize Live Category
                    int pageCount = 1;
                    int singlePageCount = 0;
                    int itemPage = 0;
                    GameObject livePrefab = null;
                    GameObject listGB = categoryAttributes.viewPort;
                    CategoryPrefab liveListUI = null;
                    for (int i = 0; i < categoryLive.Count; i++)
                    {
                        Debug.Log($"count lives {pageCount} {itemPage}");
                        LiveDTO liveList = LiveManager.instance.liveList.Where(l => l.Name == categoryLive[i].LiveName).First();
                        if (singlePageCount == 0)
                        {
                            livePrefab = Instantiate(categoryAttributes.prefabLive);

                            livePrefab.transform.SetParent(listGB.transform, false);

                            liveListUI = livePrefab.GetComponent<CategoryPrefab>();

                            pageCount++;
                        }

                        if (liveListUI != null)
                        {
                            Debug.Log($"Sprite {liveList.Sprite.name}");
                            liveListUI.liveImage[itemPage].GetComponent<Image>().sprite = liveList.Sprite;
                            liveListUI.liveImage[itemPage].GetComponent<Image>().preserveAspect = true;
                            liveListUI.livesList[itemPage].name = $"{liveList.Name}";
                            liveListUI.liveNameText[itemPage].text = $"{liveList.Name}";

                        }

                        if (singlePageCount == 1)
                        {
                            singlePageCount = 0;
                            itemPage = 0;
                        }
                        else
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
                    DisableAnimation.instance.ChangePage();
                }
            }
            ContentCategory.offsetMin = new Vector2(ContentCategory.offsetMin.x, offset);
        }
    }
    public void UpdateContinueToWatch()
    {
        List<LiveInstanceDTO> liveList = LiveManager.instance.LiveInstance;
        if (liveList.Count > 0)
        {
            float offset = 0f;
            isContinueToWatchActive = true;
            if (isContinueToWatchActiveInRun)
            {
                offset -= 600f;
                ContentCategory.offsetMin = new Vector2(ContentCategory.offsetMin.x, offset);
                isContinueToWatchActiveInRun = false;
            }
            CategoryManager.instance.categoryParent.transform.GetChild(0).gameObject.SetActive(true);
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

    public void RemoveContinueToWhatch()
    {
        LiveManager.instance.LiveInstance.Clear();
        CategoryAttributes categoryAttributes = CategoryManager.instance.categoryParent.transform.GetChild(0).GetComponent<CategoryAttributes>();
        GameObject listGB = categoryAttributes.viewPort;
        if (listGB.transform.childCount > 0)
        {
            for (int p = 0; p < listGB.transform.childCount; p++)
            {
                Destroy(listGB.transform.GetChild(p).gameObject);
            }
        }
        isContinueToWatchActive = false;
        CategoryManager.instance.categoryParent.transform.GetChild(0).gameObject.SetActive(false);
    }
}
