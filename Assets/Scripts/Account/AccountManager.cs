using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AccountManager : MonoBehaviour
{
    public static AccountManager instance;

    // User data
    private int _userToken;
    private string _name;
    private string _surname;
    private string _email;
    private Sprite _avatarImage;
    private string _position;
    private string _bioText;
    
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
    public Sprite AvatarImage
    {
        get {
            return _avatarImage;
        }
    }
    public string Position
    {
        get {
            return _position;
        }
    }
    public string Bio
    {
        get {
            return _bioText;
        }
    }

    [Header("Profile Attributes")]
    public List<Image> avatarImages = new List<Image>();
    public TMP_Text nameProfile;
    public TMP_Text positionProfile;
    public TMP_Text bioProfile;


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

    public void InitializeUser(string email, string name, string surname, int userId, string avatar, string position, string bio)
    {
        _name = name;
        _surname = surname;
        _email = email;
        _userToken = userId;

        if (!string.IsNullOrEmpty(position))
            _position = position;
        if (!string.IsNullOrEmpty(bio))
            _bioText = bio;
        if (!string.IsNullOrEmpty(avatar))
            WebService.instance.GetProfileAvatar(avatar);
        
        WebService.instance.GetWatchedLives();
        InitializeProfilePage();
    }

    public void SetAutoLogin(string email, string password, string name, string surname, int userId, string avatar, string position, string bio)
    {
        PlayerPrefs.SetString("Email", email);
        PlayerPrefs.SetString("Password", password);
        PlayerPrefs.SetString("Name", name);
        PlayerPrefs.SetString("Surname", surname);
        PlayerPrefs.SetInt("UserId", userId);

        if (!string.IsNullOrEmpty(position))
            PlayerPrefs.SetString("Position", position);
        if (!string.IsNullOrEmpty(bio))
            PlayerPrefs.SetString("Bio", bio);
        Debug.Log($"AvatarLink pref {avatar}");
        if (!string.IsNullOrEmpty(avatar))
            PlayerPrefs.SetString("AvatarLink", avatar);
    }

    public void InitializeUserAutoLogin()
    {
        _name = PlayerPrefs.GetString("Name");
        _surname = PlayerPrefs.GetString("Surname");
        _email = PlayerPrefs.GetString("Email");
        _userToken = PlayerPrefs.GetInt("UserId");

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Position")))
            _position = PlayerPrefs.GetString("Position");
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Bio")))
            _bioText = PlayerPrefs.GetString("Bio");
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("AvatarLink")))
            WebService.instance.GetProfileAvatar(PlayerPrefs.GetString("AvatarLink"));
        
        WebService.instance.GetWatchedLives();
        InitializeProfilePage();
    }

    public void UpdateProfileAvatar(Sprite sprite = null)
    {
        if (sprite != null)
            _avatarImage = sprite;

        foreach (Image avatar in avatarImages)
        {
            avatar.sprite = _avatarImage;
        }

        foreach (Image avatar in LiveManager.instance.LiveImages)
        {
            avatar.sprite = _avatarImage;
        }
    }

    public void ChangeProfileAvatar()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery( ( path ) =>
        {
            Debug.Log( "Image path: " + path );
            if( path != null )
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath( path, markTextureNonReadable: false);
                if( texture == null )
                {
                    Debug.Log( "Couldn't load texture from " + path );
                    return;
                }
                // texture = duplicateTexture(texture);
                _avatarImage = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), new Vector2(.5f,.5f)); 
                // Destroy( texture, 5f );
                WebService.instance.UploadProfileImage(texture);
            }
        });
        Debug.Log( "Permission result: " + permission );
    }

    public void InitializeProfilePage()
    {
        nameProfile.text = $"{Name} {Surname}";
        if (Position != null)
        {
            positionProfile.text = Position;
        }
        if (Bio != null)
        {
            bioProfile.text = Bio;
        }
    }

    public void UpdateProfileInfo(string position, string bio)
    {
        _position = position;
        PlayerPrefs.SetString("Position", position);
        _bioText = bio;
        PlayerPrefs.SetString("Bio", bio);
        InitializeProfilePage();
    }
}
