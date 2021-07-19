using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public static AccountManager instance;

    // User data
    private string _userToken;
    private string _name;
    private string _surname;
    private string _email;
    
    public string UserToken
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

    // Start is called before the first frame update
    void Awake()
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

    public void InitializeUser(string email, string name, string surname)
    {
        _name = name;
        _surname = surname;
        _email = email;
    }
}
