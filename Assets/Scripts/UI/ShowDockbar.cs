using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDockbar : MonoBehaviour
{
    public void ShowDockBar()
    {
        UIHandler.instance.DockBarChat();
    }
}
