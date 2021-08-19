using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageAttributes : MonoBehaviour
{
    public Image avatarImage;
    public TMP_Text messageText;

    public GameObject AnswerListGB;

    public GameObject ArrowAnsers;

    public bool IsOpenedAnswer;

    public void OpenCloseAnswerModal()
	{
		gameObject.GetComponent<Animator>().Play(IsOpenedAnswer ? "Chat_AnswerPanel_close" : "Chat_AnswerPanel_open");
        IsOpenedAnswer = IsOpenedAnswer ? false : true;
	}
}
