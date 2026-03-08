using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

//THIS IS THE MAIN SINGLETON THAT STORES A LOT OF THE GAMES IMPORTANT REFFERENCES,
//ALSO CONTROLS THE ACTION STACK AND HAS IMPORTANT FUNCTIONS
namespace Custom
{
    public class GameRoot : MonoBehaviour
    { 
        public static GameRoot Instance { get; private set; }

        public event Action OnPlayerDeath;

        [Header("Refferences")]
        public AIManager aiManager;
        public List<Transform> playerSpawns;
        public PlayerController playerController;
        [HideInInspector] public ActionStack stack;
        public bool gamePaused = false;

        [Header("Settings")]
        [SerializeField] bool canStopGame = false;
        public float timeAfterDeath = 8f;

        [Header("Prefabs")]

        public int frames;

        [Header("Actions")]
        public GameRunningAction running;
        public GamePausedAction paused;
        public GameEndAction end;

        public event Action OnPause;
        InputAction pauseAction;

        [Header("User Interface")]
        public GameObject PausedScreen;
        public GameObject DeathScreen;

        [Header("Post Processing")]
        public Volume playingPostProcessing;
        public Volume pausePostProcessing;
        public Volume deadPostProcessing;

        void Awake()
        {
            if (Instance != null && Instance != this) // SINGLETON SETUP
            {
                Destroy(Instance.gameObject);
            }

            Instance = this;

            // CREATING ACTIONS
            running = new GameRunningAction();
            paused = new GamePausedAction();
            end = new GameEndAction();

            // STARTING THE GAME ACTION
            stack = GetComponent<ActionStack>();
            stack.Push(running);

            // SETS UP PAUSE BUTTON OUTSIDE OF THE PLAYER
            PauseButtonSetup();
        }

        void PauseButtonSetup()
        {// SETS UP PAUSE BUTTON OUTSIDE OF THE PLAYER
            pauseAction = new InputAction("Pause", InputActionType.Button);
            pauseAction.AddBinding("<Keyboard>/escape");
            pauseAction.AddBinding("<Gamepad>/start");

            pauseAction.started += PauseStarted;
            pauseAction.Enable();

            OnPause += TogglePause;
        }

        void PauseStarted(InputAction.CallbackContext ctx)
        {
            OnPause?.Invoke();
        }

        public void PlayerDeath()
        {
            OnPlayerDeath?.Invoke();
            stack.Push(end);
        }

        public void TogglePause()
        {
            if (gamePaused) UnpauseGame();
            else PauseGame();
        }

        public void PauseGame()
        {
            if (!playerController.isAlive) return;
            stack.Push(paused);
            gamePaused = true;
        }

        public void UnpauseGame()
        {
            GamePausedAction pause = new GamePausedAction();
            if (stack.current != null && stack.current.GetType() == pause.GetType())
            {
                stack.Push(running);
                gamePaused = false;
            }
        }

        public void ToMainMenu()
        {
            SceneManager.LoadScene(1);
        }

        public void StopGame()
        {
            if (!canStopGame) return;
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        }
    }
}
