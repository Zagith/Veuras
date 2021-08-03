using SimpleJSON;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;

    // public TMP_Text warningLoginText;
    // public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField nameRegisterField;
    public TMP_InputField surnameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    

    //Register variables
    [Header("Edit")]
    public TMP_InputField usernameEditRegisterField;
    public TMP_InputField emailEditRegisterField;
    public TMP_Text nationEditField;

    [HideInInspector]
    public string userToken;

    // public TMP_Text warningRegisterText;

    void Start()
    {
        Instantiate();
    }

    void Instantiate()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    public void ClearInputs()
    {
        nameRegisterField.text = "";
        surnameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, nameRegisterField.text, surnameRegisterField.text));
    }

    private IEnumerator Login(string _email, string _password)
    {
        WWWForm form = new WWWForm();
        form.AddField("Email", _email);
        form.AddField("Password", Cryptography.GetSHA512(_password));

        Debug.Log(Cryptography.GetSHA512(_password));
        using (UnityWebRequest w = UnityWebRequest.Post($"{WebService.instance.WebHost}login.php", form))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError(w.error);
            }
            
            string result = w.downloadHandler.text;
            string message = "Login Success!";
            switch (result)
            {
                case "Missing Email":
                    message = "Missing Email";
                    break;
                case "Missing Password":
                    message = "Missing Password";
                    break;
                case "Invalid Email":
                    message = "Dati Errati";
                    break;
            }
            if (message == "Login Success!")
            {
                string results = w.downloadHandler.text;
                Debug.Log(results);
                JSONArray jsonArray = JSON.Parse(Regex.Replace(results, @"\s+", "")) as JSONArray;
                AccountManager.instance.InitializeUser(_email, jsonArray[0].AsObject["Nome"], jsonArray[0].AsObject["Cognome"], jsonArray[0].AsObject["UserId"]);
                AccountManager.instance.SetAutoLogin(_email, _password, jsonArray[0].AsObject["Nome"], jsonArray[0].AsObject["Cognome"], jsonArray[0].AsObject["UserId"]);
                UIHandler.instance.HomeScreen();
            }
            else
            {
                Debug.Log($"[ERROR] {message}");
                UIHandler.instance.ShowModalSettings(message, true);
            }
            Debug.Log("[Login] " + message);
        }
    }

    private IEnumerator Register(string _email, string _password, string _name, string _surname)
    {
        ClearInputs();
        if (_name == "")
        {
            //If the username field is blank show a warning
            UIHandler.instance.ShowModalSettings("Nome obbligatorio", true);
            Debug.Log("[Registration] Missing Username");
        }
        else if (_surname == "")
        {
            //If the username field is blank show a warning
            UIHandler.instance.ShowModalSettings("Cognome obbligatorio", true);
            Debug.Log("[Registration] Missing Username");
        }
        else if (passwordLoginField.text.Length < 5)
        {
            UIHandler.instance.ShowModalSettings("La password è troppo corta, minimo 5 caratteri", true);
            Debug.Log("[Registration] Password Does Not Match!");
        }
        else if(passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            UIHandler.instance.ShowModalSettings("La password non corrisponde", true);
            Debug.Log("[Registration] Password Does Not Match!");
        }
        else 
        {
            WWWForm form = new WWWForm();
            form.AddField("Name", _name);
            form.AddField("Surname", _surname);
            form.AddField("Email", _email);
            form.AddField("Password", Cryptography.GetSHA512(_password));
            
            using (UnityWebRequest w = UnityWebRequest.Post($"{WebService.instance.WebHost}adduser.php", form))
            {
                yield return w.SendWebRequest();
                if (w.isNetworkError || w.isHttpError)
                {
                    Debug.LogError(w.error);
                }
                string result = w.downloadHandler.text;

                if (result == "errore")
                {
                    UIHandler.instance.ShowModalSettings("Email esistente", true);
                    Debug.Log("[Registration] Missing Username");
                }
                else
                {        
                    StartCoroutine(Login(_email, _password));
                    UIHandler.instance.ShowTransitionDrag();
                }
            }
        }
    }
}