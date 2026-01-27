using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Scoring
    private int player1Score = 0;
    private int player2Score = 0;
    private const int WINNING_SCORE = 11; // Real ping-pong: first to 11 points wins (with 2-point margin rule)
    private const int MIN_MARGIN = 2; // Must win by 2 points

    // UI References
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI winnerText;

    // Game State
    private bool gameActive = true;
    private Ball ballScript;

    // Ball reset parameters
    public Vector3 ballStartPosition = Vector3.zero;
    public float resetDelay = 1.5f; // Delay before resetting the ball

    void Start()
    {
        ballScript = FindObjectOfType<Ball>();
        UpdateScoreUI();
    }

    /// <summary>
    /// Award a point to the specified player and handle ball reset
    /// Following real ping-pong rules:
    /// - First to 11 points wins
    /// - Must win by 2 points
    /// - Ball resets after each point
    /// </summary>
    public void AwardPoint(int player)
    {
        if (!gameActive) return;

        if (player == 1)
        {
            player1Score++;
        }
        else if (player == 2)
        {
            player2Score++;
        }

        UpdateScoreUI();
        CheckWinCondition();

        if (gameActive)
        {
            StartCoroutine(ResetBallAfterDelay());
        }
    }

    /// <summary>
    /// Check if either player has won based on ping-pong rules
    /// </summary>
    private void CheckWinCondition()
    {
        // Check if player 1 won
        if (player1Score >= WINNING_SCORE && (player1Score - player2Score) >= MIN_MARGIN)
        {
            EndGame(1);
            return;
        }

        // Check if player 2 won
        if (player2Score >= WINNING_SCORE && (player2Score - player1Score) >= MIN_MARGIN)
        {
            EndGame(2);
            return;
        }
    }

    /// <summary>
    /// Handle end of game and display winner
    /// </summary>
    private void EndGame(int winner)
    {
        gameActive = false;
        if (winnerText != null)
        {
            winnerText.text = "Player " + winner + " Wins!";
            winnerText.gameObject.SetActive(true);
        }
        Debug.Log("Player " + winner + " Wins! Final Score: P1 " + player1Score + " - P2 " + player2Score);
    }

    /// <summary>
    /// Update the UI to display current scores
    /// </summary>
    private void UpdateScoreUI()
    {
        if (player1ScoreText != null)
            player1ScoreText.text = player1Score.ToString();

        if (player2ScoreText != null)
            player2ScoreText.text = player2Score.ToString();
    }

    /// <summary>
    /// Coroutine to reset ball position and velocity after a delay
    /// </summary>
    private IEnumerator ResetBallAfterDelay()
    {
        yield return new WaitForSeconds(resetDelay);

        if (ballScript != null && gameActive)
        {
            ballScript.ResetBall();
        }
    }

    /// <summary>
    /// Reset the entire game
    /// </summary>
    public void ResetGame()
    {
        player1Score = 0;
        player2Score = 0;
        gameActive = true;
        if (winnerText != null)
            winnerText.gameObject.SetActive(false);
        UpdateScoreUI();

        if (ballScript != null)
        {
            ballScript.ResetBall();
        }
    }

    // Getters for current scores
    public int GetPlayer1Score() => player1Score;
    public int GetPlayer2Score() => player2Score;
    public bool IsGameActive() => gameActive;
}
