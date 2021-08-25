using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseLive : MonoBehaviour
{
    public Image avatarLive;

    public void CloseLiveEvent()
    {
        UIHandler.instance.CLoseLive(this.gameObject);
        ChatGui.instance.Disconnect();
    }

    public void OpenProfile()
    {
        UIHandler.instance.OpenProfilePage(this.gameObject);
        ChatGui.instance.Disconnect();
    }
}
