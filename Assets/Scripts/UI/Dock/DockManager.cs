using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DockManager : MonoBehaviour
{
    public static DockManager instance;

    public List<GameObject> dockElement = new List<GameObject>();

    public GameObject dockBar;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void UpdateDockBar()
    {
        List<LiveDTO> lives = LiveManager.instance.liveList.Where(s => s.LiveDate <= System.DateTime.Now && 
            s.LiveDate.AddHours(1) >= System.DateTime.Now).OrderBy(s => s.LiveDate).ToList();

        if (lives.Any())
        {
            int i = 0;
            foreach (LiveDTO live in lives)
            {
                if (i < 4)
                {
                    GameObject liveGB = Instantiate(dockElement[i]);
                    liveGB.transform.SetParent(dockBar.transform, false);
                    liveGB.transform.GetChild(0).GetComponent<Image>().sprite = live.Sprite;
                    liveGB.GetComponent<Button>().onClick.AddListener(delegate { UIHandler.instance.LiveScreen(
                        LiveManager.instance.liveListGB.Where(n => n.name == $"live {live.Name}").FirstOrDefault());});
                    i++;
                }
            }
        }
    }
}
