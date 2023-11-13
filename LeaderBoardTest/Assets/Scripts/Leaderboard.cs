using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public GameObject leaderboardCanvas;
    public GameObject[] leaderboardEntries;

    public static Leaderboard instance;
    private void Awake() {  instance = this; }

    // called when we log into PlayFab
    public void OnLoggedIn ()
    {
        leaderboardCanvas.SetActive(true);
        DisplayLeaderboard();
    }

    public void SetLeaderboardEntry(int newScore)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "UpdateHighscore",
            FunctionParameter = new { score = newScore },
        };

        PlayFabClientAPI.ExecuteCloudScript(request,
            result => DisplayLeaderboard(),
            Error => Debug.Log(Error.ErrorMessage)
        );
    }

    // get the "Highscore" leaderboard and display it on-screen
    public void DisplayLeaderboard()
    {
        // request to get the "Highscore" leaderboard
        GetLeaderboardRequest getLeaderboardRequest = new GetLeaderboardRequest
        {
            StatisticName = "FastestTime",
            MaxResultsCount = 10
        };

        // send the request to the API
        PlayFabClientAPI.GetLeaderboard(getLeaderboardRequest,
            // callback for if the request SUCCEEDED
            result => UpdateLeaderboardUI(result.Leaderboard),
            // callback for if the request FAILED
            error => Debug.Log(error.ErrorMessage)
        );
    }

    // updates th leaderboard UI elements
    void UpdateLeaderboardUI (List<PlayerLeaderboardEntry> leaderboard)
    {
        for(int x = 0; x < leaderboardEntries.Length; x++)
        {
            leaderboardEntries[x].SetActive(x < leaderboard.Count);
            if (x >= leaderboard.Count)
                continue;
            leaderboardEntries[x].transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = (leaderboard[x].Position + 1) + ". " + leaderboard[x].DisplayName;
            leaderboardEntries[x].transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = (-(float)leaderboard[x].StatValue * 0.001f).ToString("F2");
        }
    }
}
