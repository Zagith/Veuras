using Michsky.UI.ModernUIPack;
using UnityEngine;

public class OpenSettings : MonoBehaviour
{
    public ModalWindowManager settingsErrorModal;

    public void OpenSettingsModal()
    {
        settingsErrorModal.OpenWindow();
    }
}
