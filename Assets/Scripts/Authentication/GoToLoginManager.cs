using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToLoginManager : MonoBehaviour
{

    public void GoToLoginScene()
    {
        SceneManager.LoadScene("LoginScene");
    }
}
