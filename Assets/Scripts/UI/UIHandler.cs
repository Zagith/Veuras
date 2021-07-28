using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
   public static UIHandler instance;

    //Screen object variables
    [Header("Page Components")]
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject singMethod;
    public TMP_Text titleText;
    public Button backButtonStart;

    [Header("Pages")]
    public GameObject homeUI;
    public GameObject loginmethodUI;
    public GameObject openingUI;


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

    void Start()
    {
        homeUI.SetActive(false);
        loginmethodUI.SetActive(false);
    }

    #region Page components change function

        public void LoginScreen() //Back button
        {
            loginUI.SetActive(true);
            registerUI.SetActive(false);
            singMethod.SetActive(false);
            titleText.text = "Log In";
            titleText.gameObject.SetActive(true);
            backButtonStart.onClick.AddListener(SignMethodScreen);
            backButtonStart.gameObject.SetActive(true);
        }
        public void RegisterScreen() // Register button
        {
            loginUI.SetActive(false);
            registerUI.SetActive(true);
            singMethod.SetActive(false);
            titleText.text = "Sign In";
            titleText.gameObject.SetActive(true);
            backButtonStart.onClick.AddListener(SignMethodScreen);
            backButtonStart.gameObject.SetActive(true);
        }

        public void SignMethodScreen()
        {
            loginUI.SetActive(false);
            registerUI.SetActive(false);
            singMethod.SetActive(true);
            titleText.gameObject.SetActive(false);
            backButtonStart.gameObject.SetActive(false);
        }

        public void LiveScreen(GameObject live)
        {
            live.SetActive(true);
            homeUI.SetActive(false);
        }

    #endregion

    #region Change pages functions

        public void HomeScreen()
        {
            homeUI.SetActive(true);
            loginmethodUI.SetActive(false);
        }
        public void MethodLoginScreen()
        {
            loginmethodUI.SetActive(true);
            homeUI.SetActive(false);
            openingUI.SetActive(false);
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
}
