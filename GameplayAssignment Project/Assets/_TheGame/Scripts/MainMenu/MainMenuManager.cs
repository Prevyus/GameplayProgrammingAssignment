using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{ // MANAGES BUTTON IN THE MAIN MENU
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void StopGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
