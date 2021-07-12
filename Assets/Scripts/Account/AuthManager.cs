using Firebase;
using Firebase.Auth;
using SimpleJSON;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance;

    //Firebase variables
    [Header("Firebase")]
    public FirebaseAuth auth;    
    public FirebaseUser User;

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

    public IEnumerator CheckAndFixDependancies()
    {
        var checkAndFixDependanciesTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(predicate: () => checkAndFixDependanciesTask.IsCompleted);

        var dependencyResult = checkAndFixDependanciesTask.Result;

        if (dependencyResult == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyResult);
            }
    }

    private void InitializeFirebase()
    {
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        StartCoroutine(CheckAutoLogin());

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private IEnumerator CheckAutoLogin()
    {
        yield return new WaitForEndOfFrame();

        if (User != null)
        {
            var reloadUserTask = User.ReloadAsync();

            yield return new WaitUntil(predicate: () => reloadUserTask.IsCompleted);

            AutoLogin();
        }
        else
        {
            UIHandler.instance.LoginScreen();
            UIHandler.instance.SettingsScreen();
        }
        
        // GameManager.instance.loader.SetActive(false);
    }

    private void AutoLogin()
    {
        if (User != null)
        {
                Debug.Log($"IsEmailVerified {User.IsEmailVerified}");
            if (User.IsEmailVerified)
            {
                // PlayerManager.instance.InitializeUser(User.UserId, User.Email);
                UIHandler.instance.HomeScreen();
                UIHandler.instance.EditScreen();
            }
            else
            {
                StartCoroutine(SendVerificationEmail());
            }
        }
        else
        {
            
            UIHandler.instance.LoginScreen();

        }
    }
    private void AuthStateChanged(object sender, System.EventArgs e)
    {
        if (auth.CurrentUser != User)
        {
            bool signedIn = User != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && User != null)
            {
                Debug.Log("Signed out");
                UIHandler.instance.LoginScreen();
                UIHandler.instance.SettingsScreen();
            }

            User = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log($"Signed in: {User.IsEmailVerified}");
                if (User.IsEmailVerified)
                {
                    UIHandler.instance.HomeScreen();
                    UIHandler.instance.EditScreen();
                }
                else
                {
                    UIHandler.instance.LoginScreen();
                    UIHandler.instance.SettingsScreen();
                }
            }
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
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            UIHandler.instance.ShowModalSettings(message, true);
            Debug.Log("[Login] " + message);
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            AccountManager.instance.InitializeUser(User.UserId, User.Email);

            // TODO: edit filends
            usernameEditRegisterField.text = AccountManager.instance.Name;
            emailEditRegisterField.text = AccountManager.instance.Email;

            UIHandler.instance.HomeScreen();
            UIHandler.instance.EditScreen();

            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, AccountManager.instance.UserToken);
            Debug.Log("[Login] Logged In");
        }
    }

    private IEnumerator Register(string _email, string _password, string _name, string _surname)
    {
        if (_name == "")
        {
            ClearInputs();
            //If the username field is blank show a warning
            UIHandler.instance.ShowModalSettings("Nome obbligatorio", true);
            Debug.Log("[Registration] Missing Username");
        }
        else if (_surname == "")
        {
            ClearInputs();
            //If the username field is blank show a warning
            UIHandler.instance.ShowModalSettings("Cognome obbligatorio", true);
            Debug.Log("[Registration] Missing Username");
        }
        else if(passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            ClearInputs();
            //If the password does not match show a warning
            UIHandler.instance.ShowModalSettings("La password non corrisponde", true);
            Debug.Log("[Registration] Password Does Not Match!");
        }
        else 
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                ClearInputs();
                UIHandler.instance.ShowModalSettings(message, true);
                Debug.Log("[Registration] " + message);
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile{DisplayName = _name};

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        Debug.Log("[Registration] Username Set Failed!");
                    }
                    else
                    {
                        WWWForm form = new WWWForm();
                        form.AddField("UserId", User.UserId);
                        form.AddField("Name", _name);
                        form.AddField("Surname", _surname);
                        form.AddField("EmailVerified", 0);

                        using (var w = UnityWebRequest.Post($"{WebService.instance.WebHost}adduser.php", form))
                        {
                            yield return w.SendWebRequest();
                            if (w.result == UnityWebRequest.Result.ConnectionError || w.result == UnityWebRequest.Result.ProtocolError)
                            {
                                Debug.LogError(w.error);
                            }
                            else
                            {
                                StartCoroutine(SendVerificationEmail());
                            }
                        }
                        ClearInputs();
                        
                        //Now return to login screen
                        UIHandler.instance.LoginScreen();
                    }
                }
            }
        }
    }

    private IEnumerator SendVerificationEmail()
    {
        if (User != null)
        {
            var mailTask = User.SendEmailVerificationAsync();

            yield return new WaitUntil(predicate: () => mailTask.IsCompleted);

            if (mailTask.Exception != null)
            {
                // FirebaseException firebaseException = (FirebaseException)mailTask.Exception.GetBaseException();
                // AuthError error = (AuthError)firebaseException.ErrorCode;

                // string output = "Unkown error, try again";

                // switch (error)
                // {
                //     case AuthError.Cancelled:
                //         output = "Verification task was cancelled";
                //     break;
                //     case AuthError.InvalidRecipientEmail:
                //         output = "Invalid Email";
                //     break;
                //     case AuthError.TooManyRequests:
                //         output = "Too many requests";
                //     break;
                // }

                // UIHandler.instance.AwaitVerification(false, User.Email, output);
            }
            else
            {
                UIHandler.instance.AwaitVerification(true, User.Email, null);
            }
        }
    }
}