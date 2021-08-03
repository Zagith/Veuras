using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public static AccountManager instance;

    // User data
    private int _userToken;
    private string _name;
    private string _surname;
    private string _email;
    
    public int UserToken
    {
        get {
            return _userToken;
        }
    }
    public string Name
    {
        get {
            return _name;
        }
    }
    public string Email
    {
        get {
            return _email;
        }
    }
    public string Surname
    {
        get {
            return _surname;
        }
    }

    public bool CanAutoLogin()
    {
        return !string.IsNullOrEmpty(PlayerPrefs.GetString("Email")) && !string.IsNullOrEmpty(PlayerPrefs.GetString("Password")) && 
            !string.IsNullOrEmpty(PlayerPrefs.GetString("Name")) && !string.IsNullOrEmpty(PlayerPrefs.GetString("Surname")) && 
                PlayerPrefs.GetInt("UserId") > 0;
    }

    // Start is called before the first frame update
    void Awake()
    {
        Instantiate();
    }

    void Start()
    {
        if (AccountManager.instance.CanAutoLogin())
        {
            AccountManager.instance.InitializeUserAutoLogin();
        }
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

    public void InitializeUser(string email, string name, string surname, int userId)
    {
        _name = name;
        _surname = surname;
        _email = email;
        _userToken = userId;
    }

    public void SetAutoLogin(string email, string password, string name, string surname, int userId)
    {
        PlayerPrefs.SetString("Email", email);
        PlayerPrefs.SetString("Password", password);
        PlayerPrefs.SetString("Name", name);
        PlayerPrefs.SetString("Surname", surname);
        PlayerPrefs.SetInt("UserId", userId);
    }

    public void InitializeUserAutoLogin()
    {
        _name = PlayerPrefs.GetString("Name");
        _surname = PlayerPrefs.GetString("Surname");
        _email = PlayerPrefs.GetString("Email");
        _userToken = PlayerPrefs.GetInt("UserId");
    }
}
