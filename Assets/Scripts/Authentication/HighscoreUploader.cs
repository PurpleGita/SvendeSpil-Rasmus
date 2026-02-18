
using UnityEngine;
using System;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class HighscoreUploader : MonoBehaviour
{

    [SerializeField]
    GameObject _highScoreObject;


    private string highscoreUrl = "https://mauvestepbackend.onrender.com/api/Highscores";


    public void GetPointsAndUploadHighScore()
    {
        try
        {
            if (_highScoreObject == null)
            {
                Debug.LogError("HighscoreUploader: _highScoreObject is not assigned in the inspector!");
                return;
            }

            TextMeshProUGUI textComponent = _highScoreObject.GetComponent<TextMeshProUGUI>();
            if (textComponent == null)
            {
                Debug.LogError("HighscoreUploader: _highScoreObject does not have a TextMeshProUGUI component!");
                return;
            }

            int currentPoints = 0;
            string bossName = "Boss";
            if (int.TryParse(textComponent.text, out currentPoints))
            {
                textComponent.text = currentPoints.ToString();
                UploadHighscore(currentPoints, bossName);
            }
            else
            {
                Debug.LogError($"HighscoreUploader: Could not parse score text '{textComponent.text}' to integer!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"HighscoreUploader: Unexpected error in GetPointsAndUploadHighScore - {ex.Message}\n{ex.StackTrace}");
        }
    }

    public void UploadHighscore(int score, string bossName)
    {
        StartCoroutine(UploadHighscoreCoroutine(score, bossName));
    }

    private IEnumerator UploadHighscoreCoroutine(int score, string bossName)
    {
        string username = PlayerPrefs.GetString("Username", "");

        string json = $"{{\"bossName\":\"{bossName}\",\"username\":\"{username}\",\"score\":{score}}}";

        using (UnityWebRequest request = new UnityWebRequest(highscoreUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            string accessToken = PlayerPrefs.GetString("AccessToken", "");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.SetRequestHeader("Authorization", "Bearer " + accessToken);
            }

            Debug.Log("Sending highscore: " + json);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Highscore uploaded successfully!");
            }
            else
            {
                Debug.LogError("Failed to upload highscore: " + request.downloadHandler.text);
            }
        }
    }

    //public void TestUploadHighscore()
    //{
    //    int testScore = 1000;
    //    string testCharacterName = "TestCharacter";
    //    UploadHighscore(testScore, testCharacterName);
    //}

    public void OpenSite()
    {
        Application.OpenURL("https://mauvestepfrontend.onrender.com/");
    }

    [System.Serializable]
    private class HighscoreResponse
    {
        public string id;
        public int score;
        public string characterName;
        public string userId;
        public string createdAt;
    }
}
