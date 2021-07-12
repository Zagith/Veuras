using Michsky.UI.ModernUIPack;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
   public static UIHandler instance;

    //Screen object variables
    [Header("Page Components")]
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject editUI;

    [Header("Pages")]
    public GameObject homeUI;
    public GameObject albumUI;
    public GameObject settingsUI;
    public GameObject animalInfo;
    public GameObject charStatUI;
    public GameObject characterPackagesUI;
    public GameObject unpackPackagesUI;
    public GameObject noPackagesUI;
    public GameObject completedBook;
    public GameObject noCompletedBook;


    [Header("Modals")]
    public ModalWindowManager settingsErrorModal;
    public ModalWindowManager settingsInfoModal;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    #region Page components change function

        public void LoginScreen() //Back button
        {
            loginUI.SetActive(true);
            registerUI.SetActive(false);
            editUI.SetActive(false);
        }
        public void RegisterScreen() // Register button
        {
            loginUI.SetActive(false);
            registerUI.SetActive(true);
            editUI.SetActive(false);
        }

        public void EditScreen()
        {
            loginUI.SetActive(false);
            registerUI.SetActive(false);
            editUI.SetActive(true);
        }

    #endregion

    #region Change pages functions

        public void HomeScreen()
        {
            albumUI.SetActive(false);
            settingsUI.SetActive(false);
            homeUI.SetActive(true);
            animalInfo.SetActive(false);
            charStatUI.SetActive(false);
            characterPackagesUI.SetActive(false);
            unpackPackagesUI.SetActive(false);
            noPackagesUI.SetActive(false);
        }
        public void SettingsScreen()
        {
            albumUI.SetActive(false);
            settingsUI.SetActive(true);
            homeUI.SetActive(false);
            animalInfo.SetActive(false);
            charStatUI.SetActive(false);
            characterPackagesUI.SetActive(false);
            unpackPackagesUI.SetActive(false);
            noPackagesUI.SetActive(false);
        }

    #endregion

    public void ShowModalSettings(string message, bool isError = false)
    {
        if (!isError)
        {
            settingsInfoModal.windowDescription.text = message;
            settingsInfoModal.OpenWindow();
        }
        else
        {
            settingsErrorModal.windowDescription.text = message;
            settingsErrorModal.OpenWindow();
        }
    }

    public void AwaitVerification(bool _mailSent, string _email, string _output)
    {
        // verify mail ui set active 
        // clear outputs

        if (_mailSent)
        {
           UIHandler.instance.ShowModalSettings($"Ti è stata mandata una mail di verifica all'indiritto {_email}");
        }
        else
        {
            UIHandler.instance.ShowModalSettings(_output, true);
        }
    }
}
