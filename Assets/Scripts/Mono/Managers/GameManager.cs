using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Image _playGameTexture;

    [Header("Increases difficulty every 10 seconds by:")]
    [SerializeField] private float increaseForwardSpeed = 5f;
    [SerializeField] private float increaseSpawnChense = 3;
    [SerializeField] private float decreaseMaxTimeToSpawn = 0.4f;

    private bool startCutscene = false;
    private bool gameStarted = false;
    private bool gameOver = false;

    private Vector2 _initialTouchPosition;

    /// <summary>
    /// Loads the image to inform the player about the action needed to start the game.
    /// On mobile, it will display "Tap the screen" and on PC, it will display "Press space".
    /// </summary>
    private void Awake()
    {
        _playGameTexture.sprite = Resources.Load<Sprite>(CheckIsMobile.Check() ? "Menu/Btn_Mobie" : "Menu/Btn_PC");
    }

    private void Update()
    {
        // If the cutscene is already starting, exit the method
        if (startCutscene)
            return;

        // Check for player input to start the cutscene
        if (Input.touchCount > 0)
        {
            // To prevent accidental triggering of the cutscene, especially
            // on phones where users might unintentionally perform a "slide" gesture when exiting the game, 
            // it's important to detect only a "tap".
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                _initialTouchPosition = touch.position;

            if (touch.phase == TouchPhase.Ended && Vector2.Distance(_initialTouchPosition, touch.position) < 50)
                StartingCutscene();
        }

        // Check for player input to start the cutscene
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartingCutscene();
        }

        if(CheckForExitInput()) 
        { 
            ExitGame(); 
        }
    }

    public void IncreaseDifficulty()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Vehicle>().forwardSpeed += increaseForwardSpeed;
        SpawningBarriers[] spawningBarrier = FindObjectsByType<SpawningBarriers>(FindObjectsSortMode.None);
        foreach (SpawningBarriers barrier in spawningBarrier) { barrier.spawnChense += increaseSpawnChense; }
        SpawningVehicles.Instance.maxTimeToSpawn -= decreaseMaxTimeToSpawn;

        Debug.Log("Difficulty increased!");
    }

    /// <summary>
    /// Initiates the cutscene from the timeline. After the timeline ends and the player's first move, the game begins.
    /// </summary>
    private void StartingCutscene()
    {
        GameObject.Find("MainMenu")?.SetActive(false);
        Instantiate(Resources.Load("Timeline_Game"));
        startCutscene = true;
        Debug.Log("Cutscene started...");
    }

    /// <summary>
    /// Reloads the current scene after the end game animation triggers the "Restart" method.
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("Restart game started...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Check for PC and Mobile device
    /// </summary>
    private bool CheckForExitInput()
    {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return);
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
            Application.Quit();
#endif
    }

    /// <summary>
    /// Starts the game when the player interacts (moves left or right).
    /// </summary>
    public void StartGame()
    {
        gameStarted = true;
        Debug.Log("Game started.");
    }

    /// <summary>
    /// Called when the vehicle's health reaches 100% damage.
    /// </summary>
    public void GameOver()
    {
        gameOver = true;
        Debug.Log("Game over.");
    }

    /// <summary>
    /// The game starts as soon as the player makes a move.
    /// </summary>
    /// <returns>True if the game has started, otherwise false.</returns>
    public bool isGameStarted()
    {
        return gameStarted;
    }

    /// <summary>
    /// The game is considered over when the vehicle is 100% damaged.
    /// </summary>
    /// <returns>True if the game is over, otherwise false.</returns>
    public bool isGameOver()
    {
        return gameOver;
    }
}
