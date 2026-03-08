using UnityEngine;
using UnityEngine.SceneManagement;

public class TestingManager : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
