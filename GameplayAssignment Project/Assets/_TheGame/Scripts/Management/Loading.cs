using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{ // CONTROLS THE FAKE LOADING SCENE

    public float loadingTime = 3f;
    public TextMeshProUGUI loadingText;
    public float dotSpeed = 0.1f;
    float dotTimer = 0;
    int index = 0;

    private void Start()
    {
        loadingTime = UnityEngine.Random.Range(loadingTime - 1f, loadingTime + 1f);
    }

    private void Update()
    {
        // TIMER FOR LOADING THE NEXT SCENE
        if (loadingTime > 0) loadingTime -= Time.deltaTime;
        else LoadNextScene();

        // CONTROLLING THE LOADING TEXT WITH THE CHANGING 3 DOTS
        if (dotTimer > 0) dotTimer -= Time.deltaTime;
        else
        {
            dotTimer = dotSpeed;
            index = index == 3 ? 0 : index + 1;
            switch (index)
            {
                case 0:
                    loadingText.text = "Loading";
                    break;

                case 1:
                    loadingText.text = "Loading.";
                    break;

                case 2:
                    loadingText.text = "Loading..";
                    break;

                case 3:
                    loadingText.text = "Loading...";
                    break;
            }
        }
    }

    void LoadNextScene()
    {// GOES TO THE NEXT SCENE DEPENDING ON WHICH ONE IT WAS AT LAST
        SceneManager.LoadScene(PersistsOnLoad.Instance.lastInMainMenu ? 2 : 0);
        PersistsOnLoad.Instance.lastInMainMenu = !PersistsOnLoad.Instance.lastInMainMenu;
        loadingTime = 999;
        Destroy(gameObject);
    }
}
