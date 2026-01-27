using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISetup : MonoBehaviour
{
    void Start()
    {
        CreateUI();
    }

    /// <summary>
    /// Automatically creates all UI elements for the game
    /// </summary>
    private void CreateUI()
    {
        // Find or create Canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Create Player 1 Score Text (Top Left, above player area)
        GameObject p1ScoreGO = CreateTextElement("P1Score", canvas.transform, new Vector2(100, -100), TextAnchor.UpperLeft);
        TextMeshProUGUI p1ScoreText = p1ScoreGO.GetComponent<TextMeshProUGUI>();
        p1ScoreText.text = "0";
        p1ScoreText.fontSize = 80;

        // Create Player 2 Score Text (Top Right, above player area)
        GameObject p2ScoreGO = CreateTextElement("P2Score", canvas.transform, new Vector2(-100, -100), TextAnchor.UpperRight);
        TextMeshProUGUI p2ScoreText = p2ScoreGO.GetComponent<TextMeshProUGUI>();
        p2ScoreText.text = "0";
        p2ScoreText.fontSize = 80;

        // Create Winner Text (Top Center, smaller)
        GameObject winnerGO = CreateTextElement("WinnerText", canvas.transform, new Vector2(0, -80), TextAnchor.UpperCenter);
        TextMeshProUGUI winnerText = winnerGO.GetComponent<TextMeshProUGUI>();
        winnerText.text = "Player 1 Wins!";
        winnerText.fontSize = 40;
        winnerText.gameObject.SetActive(false); // Hidden until someone wins

        // Assign to GameManager
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.player1ScoreText = p1ScoreText;
            gameManager.player2ScoreText = p2ScoreText;
            gameManager.winnerText = winnerText;
            Debug.Log("UI elements created and assigned to GameManager");
        }
        else
        {
            Debug.LogError("GameManager not found in scene!");
        }
    }

    /// <summary>
    /// Helper method to create a text element
    /// </summary>
    private GameObject CreateTextElement(string name, Transform parent, Vector2 position, TextAnchor alignment)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);

        // Add RectTransform
        RectTransform rectTransform = textGO.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        // Position based on alignment
        if (alignment == TextAnchor.UpperLeft)
        {
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.offsetMin = position;
            rectTransform.offsetMax = new Vector2(position.x + 250, position.y - 150);
        }
        else if (alignment == TextAnchor.UpperRight)
        {
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = new Vector2(position.x - 250, position.y);
            rectTransform.offsetMax = new Vector2(position.x, position.y - 150);
        }
        else if (alignment == TextAnchor.UpperCenter)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.sizeDelta = new Vector2(400, 100);
            rectTransform.anchoredPosition = new Vector2(position.x, position.y);
        }

        // Add TextMeshProUGUI
        TextMeshProUGUI textComponent = textGO.AddComponent<TextMeshProUGUI>();
        textComponent.text = "Text";
        textComponent.alignment = ConvertAnchorToAlignment(alignment);
        textComponent.color = Color.white;

        return textGO;
    }

    /// <summary>
    /// Convert TextAnchor to TextAlignmentOptions
    /// </summary>
    private TextAlignmentOptions ConvertAnchorToAlignment(TextAnchor anchor)
    {
        return anchor switch
        {
            TextAnchor.UpperLeft => TextAlignmentOptions.TopLeft,
            TextAnchor.UpperRight => TextAlignmentOptions.TopRight,
            TextAnchor.UpperCenter => TextAlignmentOptions.Top,
            TextAnchor.MiddleCenter => TextAlignmentOptions.Center,
            _ => TextAlignmentOptions.TopLeft,
        };
    }
}
