using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoryManager : MonoBehaviour
{
    public static CategoryManager instance;

    public GameObject incomingCategory;

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
}
