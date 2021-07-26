using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoryPrefab : MonoBehaviour
{
    public List<Image> liveImage;
    public List<TMP_Text> liveNameText;

    public List<GameObject> livesList;

    public void OpenLive(int value)
    {
        LiveManager.instance.GoToLive(livesList[value].name);
    }

}
