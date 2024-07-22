using System;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#elif UNITY_IOS || UNITY_IPHONE
using UnityEngine.SocialPlatforms.GameCenter;
// External dependencies IOS
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using AppleAuth.Native;
#endif
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

using System.Text;


public class GPManager : MonoBehaviour
{
    public dataInCloudManager dataCloud;

    public string Token;
    public string Error;

    void Awake()
    {
        Debug.Log("Awake");
#if UNITY_ANDROID
        PlayGamesPlatform.Activate();
#elif UNITY_IOS || UNITY_IPHONE
        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                Debug.Log("Successfully authenticated with Game Center");
                // Handle successful authentication here
            }
            else
            {
                Debug.Log("Failed to authenticate with Game Center");
                // Handle failed authentication here
            }
        });
#endif

    }

    async void Start()
    {
        Debug.Log("Jeu démarré");
#if !UNITY_EDITOR
        try
        {
            await UnityServices.InitializeAsync();
        }catch (Exception){ }

#if UNITY_ANDROID
        try
        {
            await LoginGooglePlayGames();
            await SignInWithGooglePlayGamesAsync(Token);
        }
        catch (Exception) { }
#elif UNITY_IOS || UNITY_IPHONE
        try
        {
        await LoginToApple();
        await SignInWithICloudAsync(Token);
        }
        catch (Exception) { }
#endif

#endif

        Debug.Log("End wait connect account");
        // Une fois que s'est bien enregistré
        dataCloud.endWaitConnecToAccount();
    }

    /*
     * Task LoginGooglePlayGames()
     * Task SignInWithGooglePlayGamesAsync(string authCode)
    */
    #region ANDROID
    //Fetch the Token / Auth code
#if UNITY_ANDROID
    public Task LoginGooglePlayGames()
    {
        var tcs = new TaskCompletionSource<object>();
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Login with Google Play games successful.");
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    Debug.Log("Authorization code: " + code);
                    Token = code;
                    // This token serves as an example to be used for SignInWithGooglePlayGames
                    tcs.SetResult(null);
                });
            }
            else
            {
                Error = "Failed to retrieve Google play games authorization code";
                Debug.Log("Login Unsuccessful");
                tcs.SetException(new Exception("Failed"));
            }
        });
        return tcs.Task;
    }


    async Task SignInWithGooglePlayGamesAsync(string authCode)
    {
        if (authCode == null || authCode == "")
            return;

        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}"); //Display the Unity Authentication PlayerID
            Debug.Log("SignIn is successful.");
            // appeler manager de connexion
            dataCloud.playerConnectedToGooglePlay();// authCode, AuthenticationService.Instance.PlayerId);
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
#endif

    #endregion

    /*
     * void HandleAuthenticated(bool success)
     * void Update()
     * void Initialize()
     * void LoginToApple()
     * Task SignInWithICloudAsync(string authCode)
    */
    #region APPLE

    IAppleAuthManager m_AppleAuthManager;

#if UNITY_IOS || UNITY_IPHONE
    private void HandleAuthenticated(bool success)
    {
        if (success)
        {
            Debug.Log("Successfully authenticated with Game Center");
            // Handle successful authentication here
        }
        else
        {
            Debug.Log("Failed to authenticate with Game Center");
            // Handle failed authentication here
        }
    }

    public void Update()
    {
        if (m_AppleAuthManager != null)
        {
            m_AppleAuthManager.Update();
        }
    }
#endif
    public void Initialize()
    {
        var deserializer = new PayloadDeserializer();
        m_AppleAuthManager = new AppleAuthManager(deserializer);
    }

    public Task LoginToApple()
    {
        var tcs = new TaskCompletionSource<object>();

        // Initialize the Apple Auth Manager
        if (m_AppleAuthManager == null)
        {
            Initialize();
        }

        // Set the login arguments
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        // Perform the login
        m_AppleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                var appleIDCredential = credential as IAppleIDCredential;
                if (appleIDCredential != null)
                {
                    var idToken = Encoding.UTF8.GetString(
                        appleIDCredential.IdentityToken,
                        0,
                        appleIDCredential.IdentityToken.Length);
                    //Debug.Log("Sign-in with Apple successfully done. IDToken: " + idToken);
                    Token = idToken;
                    tcs.SetResult(null);
                }
                else
                {
                    //Debug.Log("Sign-in with Apple error. Message: appleIDCredential is null");
                    Error = "Retrieving Apple Id Token failed.";
                    tcs.SetException(new Exception("Retrieving Apple Id Token failed."));
                }
            },
            error =>
            {
                //Debug.Log("Sign-in with Apple error. Message: " + error);
                Error = "Retrieving Apple Id Token failed.";
                tcs.SetException(new Exception("Retrieving Apple Id Token failed."));
            }
        );
        return tcs.Task;
    }

    async Task SignInWithICloudAsync(string authCode)
    {
        if (authCode == null || authCode == "")
            return;

        try
        {
            await AuthenticationService.Instance.SignInWithAppleAsync(authCode);
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}"); //Display the Unity Authentication PlayerID
            Debug.Log("SignIn is successful.");
            // appeler manager de connexion
            dataCloud.playerConnectedToICloud();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
    #endregion
}