using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (ChatGui))]
public class NamePickGui : MonoBehaviour
{
    public static NamePickGui instance;
    private const string UserNamePlayerPref = "NamePickUserName";

    public ChatGui chatNewComponent;

    public InputField idInput;

    void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        this.chatNewComponent = FindObjectOfType<ChatGui>();


        string prefsName = PlayerPrefs.GetString(NamePickGui.UserNamePlayerPref);
        if (!string.IsNullOrEmpty(prefsName))
        {
            this.idInput.text = prefsName;
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
        chatNewComponent.UserName = AccountManager.instance.Name;
        Debug.Log($"chat name {AccountManager.instance.Name}");
		chatNewComponent.Connect();
        enabled = false;

        PlayerPrefs.SetString(NamePickGui.UserNamePlayerPref, chatNewComponent.UserName);
    }
}