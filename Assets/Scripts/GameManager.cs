using UnityEngine;
using GameAnalyticsSDK;
using Facebook.Unity;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public GameState gameState = new GameState();
    public enum GameState
    {
        None,
        Playing,
        GameOver
    }

    [SerializeField] private bool SetTargetFrameRate;
    
    public void SetGameState(string state)
    {
        gameState = (GameState)System.Enum.Parse(typeof(GameState), state);
    }

    private void Awake() 
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        } else {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += OnSceneChanged;

        if(SetTargetFrameRate)
            Application.targetFrameRate = 60;

        GameAnalytics.Initialize();

        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        } else {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback ()
    {
        if (FB.IsInitialized) {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        } else {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    void OnSceneChanged(Scene current, Scene next)
    {
        SetGameState("None");
    }

    private void OnHideUnity (bool isGameShown)
    {
        if (!isGameShown) {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        } else {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void StartGame()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");
    }

    public void FinishGame(PlayerData playerData)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", null, null, (int)playerData.RuntimeScore);
    }
}
