using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.Events;
public class LogInRegister : MonoBehaviour
{
    [HideInInspector]
    public string playFabId;

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public TextMeshProUGUI displayText;

    public UnityEvent onLoggedIn;

    public static LogInRegister instance;
    private void Awake() { instance = this; }

    public void OnLoginButton()
    {
        // request to login a user
        LoginWithPlayFabRequest loginRequest = new LoginWithPlayFabRequest
        {
            Username = usernameInput.text,
            Password = passwordInput.text,
        };

        // send the request to the API
        PlayFabClientAPI.LoginWithPlayFab(loginRequest,
            // callback function for if register SUCCEEDED
            result =>
            {
                SetDisplayText("Logged in as: " + result.PlayFabId, Color.green);
                playFabId = result.PlayFabId;


                if (onLoggedIn != null)
                {
                    onLoggedIn.Invoke();
                }
            },
            // callback function for if register FAILED
            error => SetDisplayText(error.ErrorMessage, Color.red)
            );
    }

    public void OnRegisterButton()
    {
        // request to register a new user
        RegisterPlayFabUserRequest registerRequest = new RegisterPlayFabUserRequest
        {
            Username = usernameInput.text,
            DisplayName = usernameInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };
        // send the request to the API
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest,
            // callback funtion for if register SUCCEEDED
            result => SetDisplayText("Registered a new account as: " + result.PlayFabId, Color.green),
            // callback function for if register FAILED
            error => SetDisplayText(error.ErrorMessage, Color.red)
        );

    }

    void SetDisplayText (string text, Color color)
    { 
        displayText.text = text;
        displayText.color = color;
    }
}
