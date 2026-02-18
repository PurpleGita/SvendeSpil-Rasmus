using UnityEngine;
using System;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;


public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_Text feedbackText;
    public LoadingScreenManager loadingScreenManager;

    private string loginUrl = "https://mauvestepbackend.onrender.com/api/Auth/login";

    public void Login()
    {

        if (string.IsNullOrWhiteSpace(usernameField.text))
        {
            feedbackText.text = "Username is required.";
            return; 
        }
        if (string.IsNullOrWhiteSpace(passwordField.text))
        {
            feedbackText.text = "Password is required.";
            return;
        }
        if (usernameField.text.Length < 3 || usernameField.text.Length > 20)
        {
            feedbackText.text = "Username must be between 3 and 20 characters.";
            return;
        }
        if (passwordField.text.Length < 6)
        {
            feedbackText.text = "Password must be at least 6 characters long.";
            return;
        }

        StartCoroutine(LoginCoroutine());
    }

    private IEnumerator LoginCoroutine()
    {
        feedbackText.text = "Sending login request...";
        var loginData = new
        {
            username = usernameField.text,
            password = passwordField.text
        };

        // Manually construct the JSON string
        string json = $"{{\"username\":\"{usernameField.text}\",\"password\":\"{passwordField.text}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(loginUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            Debug.Log("Sending login request: " + json);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                feedbackText.text = "Login successful!";
                //Debug.Log("Response: " + request.downloadHandler.text);

                // Parse the nested response structure
                LoginResponseWrapper response = JsonUtility.FromJson<LoginResponseWrapper>(request.downloadHandler.text);
                
                if (response.success && response.data != null)
                {
                    string accessToken = response.data.accessToken;
                    string refreshToken = response.data.refreshToken;
                    
                    PlayerPrefs.SetString("AccessToken", accessToken);
                    PlayerPrefs.SetString("RefreshToken", refreshToken);

                    string username = DecodeJWT(accessToken);
                    if (!string.IsNullOrEmpty(username))
                    {
                        PlayerPrefs.SetString("Username", username);
                    }

                    loadingScreenManager.StartLoading("FightSketch");
                }
                else
                {
                    feedbackText.text = "Login failed: Invalid response";
                }

            }
            else
            {
                feedbackText.text = "Login failed:" + request.downloadHandler.text;
                Debug.LogError("Error: " + request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }

    private string DecodeJWT(string token)
    {
        try
        {
            // Split the token into its parts (Header, Payload, Signature)
            string[] parts = token.Split('.');
            if (parts.Length < 2)
            {
                Debug.LogError("Invalid JWT token");
                return null;
            }

            // Decode the Payload (Base64 URL encoded)
            string payload = parts[1];
            string decodedPayload = Encoding.UTF8.GetString(Convert.FromBase64String(PadBase64(payload)));

            Debug.Log("Decoded Payload: " + decodedPayload);

            // Parse the JSON payload to extract the username
            var payloadData = JsonUtility.FromJson<PayloadData>(decodedPayload);
            //Debug.Log("Decoded Payload Data: " + payloadData.unique_name);
            return payloadData.unique_name;
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to decode JWT: " + ex.Message);
            return null;
        }
    }

    private string PadBase64(string base64)
    {
        // Add padding if necessary
        switch (base64.Length % 4)
        {
            case 2: return base64 + "==";
            case 3: return base64 + "=";
            default: return base64;
        }
    }

    [System.Serializable]
    private class LoginResponseWrapper
    {
        public bool success;
        public LoginResponseData data;
    }

    [System.Serializable]
    private class LoginResponseData
    {
        public UserInfo user;
        public string accessToken;
        public string refreshToken;
        public int expiresIn;
    }

    [System.Serializable]
    private class UserInfo
    {
        public string id;
        public string username;
        public string email;
    }

    [System.Serializable]
    private class PayloadData
    {
        public string unique_name;
    }
}
