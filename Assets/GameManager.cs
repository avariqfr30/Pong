using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Game States
    public enum GameState { Menu, Playing, GameOver }
    private GameState currentState = GameState.Menu;

    // Scoring
    private int player1Points = 0;
    private int player2Points = 0;
    private int player1RoundWins = 0;
    private int player2RoundWins = 0;
    private int currentRound = 1;
    private const int POINTS_TO_WIN_ROUND = 5; // Points needed to win a round
    private const int ROUNDS_TO_WIN_MATCH = 3; // Rounds needed to win the match

    // UI References
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI winnerText;
    public GameObject startMenuPanel;
    public GameObject gameOverPanel;
    public Button startButton;
    public Button restartButton;
    public Button menuButton;

    // Game Objects
    private Ball ballScript;
    private GameObject[] paddles;
    private GameObject[] walls;

    // Ball reset parameters
    public Vector3 ballStartPosition = Vector3.zero;
    public float resetDelay = 1.5f; // Delay before resetting the ball

    void Awake()
    {
        // Immediately disable all game objects on scene load
        Ball tempBall = FindFirstObjectByType<Ball>();
        if (tempBall != null)
        {
            ballScript = tempBall;
            ballScript.gameObject.SetActive(false);
        }

        // Disable paddles
        PaddleCollision[] paddleComponents = FindObjectsByType<PaddleCollision>(FindObjectsSortMode.None);
        paddles = new GameObject[paddleComponents.Length];
        for (int i = 0; i < paddleComponents.Length; i++)
        {
            paddles[i] = paddleComponents[i].gameObject;
            paddles[i].SetActive(false);
        }

        // Disable walls (any colliders that aren't paddles or ball)
        Collider2D[] allColliders = FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
        List<GameObject> wallList = new List<GameObject>();
        foreach (Collider2D col in allColliders)
        {
            if (col.gameObject != tempBall?.gameObject && col.GetComponent<PaddleCollision>() == null)
            {
                wallList.Add(col.gameObject);
                col.gameObject.SetActive(false);
            }
        }
        walls = wallList.ToArray();
    }

    void Start()
    {
        Debug.Log($"Ball script found: {ballScript != null}");
        if (ballScript != null)
            Debug.Log($"Ball gameObject: {ballScript.gameObject.name}");
        
        Debug.Log($"Paddles found: {paddles.Length}");
        for (int i = 0; i < paddles.Length; i++)
        {
            Debug.Log($"Paddle {i}: {paddles[i].name}");
        }
        
        // Find walls (objects with colliders but not paddles or ball) - already done in Awake
        Debug.Log($"Walls array length: {walls.Length}");
        for (int i = 0; i < walls.Length; i++)
        {
            Debug.Log($"Wall {i}: {walls[i].name}");
        }
        
        // Button listeners will be set up in OnUIRready when UI is created
        
        UpdateScoreUI();
        
        Debug.Log($"GameManager initialized. StartMenuPanel: {startMenuPanel != null}, GameOverPanel: {gameOverPanel != null}, StartButton: {startButton != null}");
    }

    public void OnUIRready()
    {
        Debug.Log("UI is ready, setting initial game state to Menu");
        
        // Setup button listeners now that UI is ready
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (menuButton != null)
            menuButton.onClick.AddListener(GoToMenu);
        
        SetGameState(GameState.Menu);
    }

    /// <summary>
    /// Award a point to the specified player and handle ball reset
    /// - 5 points to win a round
    /// - Ball resets after each point
    /// </summary>
    public void AwardPoint(int player)
    {
        if (currentState != GameState.Playing) return;

        if (player == 1)
        {
            player1Points++;
        }
        else if (player == 2)
        {
            player2Points++;
        }

        UpdateScoreUI();
        CheckWinCondition();

        if (currentState == GameState.Playing)
        {
            StartCoroutine(ResetBallAfterDelay());
        }
    }

    /// <summary>
    /// Handle winning a round
    /// </summary>
    private void WinRound(int winner)
    {
        if (winner == 1)
        {
            player1RoundWins++;
        }
        else if (winner == 2)
        {
            player2RoundWins++;
        }

        // Reset points for next round
        player1Points = 0;
        player2Points = 0;
        currentRound++;

        UpdateScoreUI();

        // Check if match is won
        if (player1RoundWins >= ROUNDS_TO_WIN_MATCH)
        {
            EndGame(1);
        }
        else if (player2RoundWins >= ROUNDS_TO_WIN_MATCH)
        {
            EndGame(2);
        }
        // Else, continue to next round
    }

    /// <summary>
    /// Check if either player has won the current round
    /// </summary>
    private void CheckWinCondition()
    {
        // Check if player 1 won the round
        if (player1Points >= POINTS_TO_WIN_ROUND)
        {
            WinRound(1);
            return;
        }

        // Check if player 2 won the round
        if (player2Points >= POINTS_TO_WIN_ROUND)
        {
            WinRound(2);
            return;
        }
    }

    /// <summary>
    /// Handle end of game and display winner
    /// </summary>
    private void EndGame(int winner)
    {
        SetGameState(GameState.GameOver);
        if (winnerText != null)
        {
            winnerText.text = "Player " + winner + " Wins!";
            winnerText.gameObject.SetActive(true);
        }
        Debug.Log("Player " + winner + " Wins! Final Round Wins: P1 " + player1RoundWins + " - P2 " + player2RoundWins);
    }

    /// <summary>
    /// Update the UI to display current scores
    /// </summary>
    private void UpdateScoreUI()
    {
        if (player1ScoreText != null)
            player1ScoreText.text = player1RoundWins.ToString();

        if (player2ScoreText != null)
            player2ScoreText.text = player2RoundWins.ToString();

        if (roundText != null)
            roundText.text = "Round " + currentRound;

        if (pointsText != null)
            pointsText.text = player1Points + " - " + player2Points;
    }

    /// <summary>
    /// Coroutine to reset ball position and velocity after a delay
    /// </summary>
    private IEnumerator ResetBallAfterDelay()
    {
        yield return new WaitForSeconds(resetDelay);

        if (ballScript != null && currentState == GameState.Playing)
        {
            ballScript.ResetBall();
        }
    }

    /// <summary>
    /// Reset the entire game
    /// </summary>
    public void ResetGame()
    {
        player1Points = 0;
        player2Points = 0;
        player1RoundWins = 0;
        player2RoundWins = 0;
        currentRound = 1;
        if (winnerText != null)
            winnerText.gameObject.SetActive(false);
        UpdateScoreUI();
        SetGameState(GameState.Playing);
    }

    /// <summary>
    /// Set the current game state and update UI accordingly
    /// </summary>
    private void SetGameState(GameState newState)
    {
        Debug.Log($"Setting game state to: {newState}");
        currentState = newState;
        
        // Enable/disable game objects based on state
        bool gameObjectsActive = (currentState == GameState.Playing);
        Debug.Log($"Game objects active: {gameObjectsActive}");
        Debug.Log($"Ball script: {ballScript != null}, Paddles count: {paddles?.Length ?? 0}, Walls count: {walls?.Length ?? 0}");
        
        if (ballScript != null)
        {
            ballScript.gameObject.SetActive(gameObjectsActive);
            Debug.Log($"Ball set active: {gameObjectsActive}");
        }
        else
        {
            Debug.LogError("Ball script is null!");
        }
        
        if (paddles != null)
        {
            foreach (GameObject paddle in paddles)
            {
                if (paddle != null)
                {
                    paddle.SetActive(gameObjectsActive);
                    Debug.Log($"Setting paddle {paddle.name} active: {gameObjectsActive}");
                }
            }
        }
        else
        {
            Debug.LogError("Paddles array is null!");
        }
        
        if (walls != null)
        {
            foreach (GameObject wall in walls)
            {
                if (wall != null)
                {
                    wall.SetActive(gameObjectsActive);
                    Debug.Log($"Setting wall {wall.name} active: {gameObjectsActive}");
                }
            }
        }
        else
        {
            Debug.LogError("Walls array is null!");
        }
        
        // Show/hide UI panels
        Debug.Log($"UI Panels - StartMenu: {startMenuPanel != null}, GameOver: {gameOverPanel != null}");
        if (startMenuPanel != null)
        {
            startMenuPanel.SetActive(currentState == GameState.Menu);
            Debug.Log($"StartMenuPanel set to active: {currentState == GameState.Menu}");
        }
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(currentState == GameState.GameOver);
            Debug.Log($"GameOverPanel set to active: {currentState == GameState.GameOver}");
        }
        
        // Show scores only during playing and game over
        if (player1ScoreText != null)
            player1ScoreText.gameObject.SetActive(currentState != GameState.Menu);
        if (player2ScoreText != null)
            player2ScoreText.gameObject.SetActive(currentState != GameState.Menu);
        if (roundText != null)
            roundText.gameObject.SetActive(currentState != GameState.Menu);
        if (pointsText != null)
            pointsText.gameObject.SetActive(currentState != GameState.Menu);
        
        // Reset ball when starting game
        if (currentState == GameState.Playing && ballScript != null)
        {
            ballScript.ResetBall();
        }
    }

    /// <summary>
    /// Start the game from menu
    /// </summary>
    public void StartGame()
    {
        Debug.Log("StartGame button pressed - starting game");
        ResetGame();
    }

    /// <summary>
    /// Restart the game from game over screen
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("RestartGame button pressed");
        ResetGame();
    }

    /// <summary>
    /// Go back to main menu
    /// </summary>
    public void GoToMenu()
    {
        Debug.Log("GoToMenu button pressed");
        player1Points = 0;
        player2Points = 0;
        player1RoundWins = 0;
        player2RoundWins = 0;
        currentRound = 1;
        if (winnerText != null)
            winnerText.gameObject.SetActive(false);
        UpdateScoreUI();
        SetGameState(GameState.Menu);
    }

    // Getters for current round wins
    public int GetPlayer1Score() => player1RoundWins;
    public int GetPlayer2Score() => player2RoundWins;
    public bool IsGameActive() => currentState == GameState.Playing;
}
