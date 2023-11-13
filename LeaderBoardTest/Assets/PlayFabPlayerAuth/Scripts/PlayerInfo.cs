using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
public class PlayerInfo : MonoBehaviour
{
    [HideInInspector]
    public PlayerProfileModel profile;

    public static PlayerInfo instance;
    private void Awake() { instance = this; }

    public void OnLoggedIn()
    {
        GetPlayerProfileRequest getProfileRequest = new GetPlayerProfileRequest
        {
            PlayFabId = LogInRegister.instance.playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true,
            }
        };

        PlayFabClientAPI.GetPlayerProfile(getProfileRequest,
            result =>
            {
                profile = result.PlayerProfile;
                Debug.Log("Loaded in Player: " + profile.DisplayName);
            },
            error => Debug.Log(error.ErrorMessage)
            );
    }
}
