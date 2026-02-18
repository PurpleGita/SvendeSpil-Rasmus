using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;


public class LoadingScreenManager : MonoBehaviour
{
    public Image blackOverlay; 
    public TMP_Text loadingText; 

    public void StartLoading(string sceneName)
    {
        StartCoroutine(LoadSceneWithFade(sceneName));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        
        // Fade to black
        yield return StartCoroutine(FadeToBlack());
        yield return null;
        //loadingText.gameObject.SetActive(true);
        StartCoroutine(LoadingTextEffect());
        yield return null;
        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator LoadingTextEffect()
    {
        int dotCount = 0;
        while (true)
        {

            // Update the text with the appropriate number of dots
            loadingText.text = "LOADING" + new string('.', dotCount);

            // Increment the dot count (reset to 0 after 3)
            dotCount = (dotCount + 1) % 4;

            // Wait for 1 second before updating again
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator FadeToBlack()
    {
        float duration = 1f; // Duration of the fade
        float elapsedTime = 0f;
        blackOverlay.gameObject.SetActive(true);
        Color color = blackOverlay.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / duration); // Gradually increase alpha
            blackOverlay.color = color;
            yield return null;
        }
       
    }
}