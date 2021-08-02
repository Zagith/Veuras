using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (ChatGui))]
public class NamePickGui : MonoBehaviour
{
    public static NamePickGui instance;
    private const string UserNamePlayerPref = "NamePickUserName";

    public ChatGui chatNewComponent;

    private string idInput;

    void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        this.chatNewComponent = FindObjectOfType<ChatGui>();


        string prefsName = $"{PlayerPrefs.GetString("Name")} {PlayerPrefs.GetString("Surname")}";
        if (!string.IsNullOrEmpty(prefsName))
        {
            this.idInput = prefsName;
        }
    }


    // new UI will fire "EndEdit" event also when loosing focus. So check "enter" key and only then StartChat.
    public void EndEditOnEnter()
    {
        // if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        // {
        //     this.StartChat();
        // }
    }

    public void StartChat(GameObject live)
    {
        ChatGui chatNewComponent = FindObjectOfType<ChatGui>();
        ChatGui.instance.ChatPanel.transform.SetParent(live.transform.GetChild(1).transform, false);
        if (!string.IsNullOrEmpty(idInput))
            chatNewComponent.UserName = idInput;
        else
            chatNewComponent.UserName = $"{AccountManager.instance.Name} {AccountManager.instance.Surname}";
		chatNewComponent.Connect();
        enabled = false;

        PlayerPrefs.SetString(NamePickGui.UserNamePlayerPref, chatNewComponent.UserName);
    }
}