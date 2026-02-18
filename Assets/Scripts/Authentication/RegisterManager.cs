using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_Text feedbackText;

    private string registerUrl = "https://mauvestepbackend.onrender.com/api/Auth/register";

    public void Register()
    {
        if (string.IsNullOrWhiteSpace(usernameField.text))
        {
            feedbackText.text = "Username is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(emailField.text))
        {
            feedbackText.text = "Email is required.";
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

        if (!IsValidEmail(emailField.text))
        {
            feedbackText.text = "Please enter a valid email address.";
            return;
        }

        if (passwordField.text.Length < 6)
        {
            feedbackText.text = "Password must be at least 6 characters long.";
            return;
        }

        StartCoroutine(RegisterCoroutine());
    }

    private IEnumerator RegisterCoroutine()
    {
        feedbackText.text = "Creating account...";

        string json = $"{{\"username\":\"{usernameField.text}\",\"email\":\"{emailField.text}\",\"password\":\"{passwordField.text}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(registerUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            Debug.Log("Sending registration request: " + json);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Parse the response
                RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(request.downloadHandler.text);

                if (response.success)
                {
                    feedbackText.text = "Registration successful! Redirecting to login...";
                    Debug.Log("User ID: " + response.userId);
                    yield return new WaitForSeconds(2);
                    SceneManager.LoadScene("LoginScene");
                }
                else
                {
                    feedbackText.text = "Registration failed: " + response.message;
                }
            }
            else
            {
                feedbackText.text = "Registration failed: " + request.downloadHandler.text;
                Debug.LogError("Error: " + request.error);
                Debug.LogError("Response Code: " + request.responseCode);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    [System.Serializable]
    private class RegisterResponse
    {
        public bool success;
        public string message;
        public string userId;
    }
}