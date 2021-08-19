using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerButton : MonoBehaviour
{
    public void AnswerButtonEvent()
    {
        Transform parent = gameObject.transform.parent;
        ChatGui.instance.AnswerEvent(parent.gameObject, parent.GetChild(1).GetComponent<TMP_Text>().text, parent.GetChild(0).GetChild(0).GetComponent<Image>().sprite);
    }
}
