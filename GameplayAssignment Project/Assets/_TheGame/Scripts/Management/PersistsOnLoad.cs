using UnityEngine;

public class PersistsOnLoad : MonoBehaviour
{// OBJECT THAT KEEPS EXISTING EVEN AFTER CHANGING SCENES, STORES IF THE LAST SCENE WAS THE MENU OR NOT
    public static PersistsOnLoad Instance { get; private set; }

    public bool lastInMainMenu = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}