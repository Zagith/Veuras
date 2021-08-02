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
}
