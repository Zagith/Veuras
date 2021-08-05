using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOffDock : MonoBehaviour
{
    public void SetOff()
    {
        UIHandler.instance.DockBarVisibility();
    }
}
