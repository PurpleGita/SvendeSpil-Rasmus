using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToRegisterManager : MonoBehaviour
{
    public void GoToRegisterScene()
    {
        SceneManager.LoadScene("RegisterScene");
    }
}
