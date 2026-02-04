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
        // Find existing Canvas or create one
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Ensure EventSystem exists for UI interaction
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
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

        // Create Round Text (Top Center, below scores)
        GameObject roundGO = CreateTextElement("RoundText", canvas.transform, new Vector2(0, -150), TextAnchor.UpperCenter);
        TextMeshProUGUI roundText = roundGO.GetComponent<TextMeshProUGUI>();
        roundText.text = "Round 1";
        roundText.fontSize = 40;

        // Create Points Text (Top Center, below round)
        GameObject pointsGO = CreateTextElement("PointsText", canvas.transform, new Vector2(0, -200), TextAnchor.UpperCenter);
        TextMeshProUGUI pointsText = pointsGO.GetComponent<TextMeshProUGUI>();
        pointsText.text = "0 - 0";
        pointsText.fontSize = 50;

        // Create Winner Text (Top Center, smaller)
        GameObject winnerGO = CreateTextElement("WinnerText", canvas.transform, new Vector2(0, -80), TextAnchor.UpperCenter);
        TextMeshProUGUI winnerText = winnerGO.GetComponent<TextMeshProUGUI>();
        winnerText.text = "Player 1 Wins!";
        winnerText.fontSize = 40;
        winnerText.gameObject.SetActive(false); // Hidden until someone wins

        // Create Start Menu Panel
        GameObject startMenuPanel = CreatePanel("StartMenuPanel", canvas.transform);
        CreateMenuText("PONG", startMenuPanel.transform, new Vector2(0, 100));
        CreateMenuText("Press START to begin", startMenuPanel.transform, new Vector2(0, 0));
        Button startButton = CreateButton("StartButton", startMenuPanel.transform, new Vector2(0, -100), "START");

        // Create Game Over Panel
        GameObject gameOverPanel = CreatePanel("GameOverPanel", canvas.transform);
        CreateMenuText("GAME OVER", gameOverPanel.transform, new Vector2(0, 100));
        Button restartButton = CreateButton("RestartButton", gameOverPanel.transform, new Vector2(-120, -80), "RESTART");
        Button menuButton = CreateButton("MenuButton", gameOverPanel.transform, new Vector2(120, -80), "MENU");

        // Assign to GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.player1ScoreText = p1ScoreText;
            gameManager.player2ScoreText = p2ScoreText;
            gameManager.roundText = roundText;
            gameManager.pointsText = pointsText;
            gameManager.winnerText = winnerText;
            gameManager.startMenuPanel = startMenuPanel;
            gameManager.gameOverPanel = gameOverPanel;
            gameManager.startButton = startButton;
            gameManager.restartButton = restartButton;
            gameManager.menuButton = menuButton;
            Debug.Log("UI elements created and assigned to GameManager");
            Debug.Log($"Start Menu Panel: {startMenuPanel != null}, Game Over Panel: {gameOverPanel != null}");
            Debug.Log($"Start Button: {startButton != null}, Restart Button: {restartButton != null}, Menu Button: {menuButton != null}");
            
            // Notify GameManager that UI is ready
            gameManager.OnUIRready();
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
        textComponent.color = Color.white;

        return textGO;
    }

    /// <summary>
    /// Helper method to create a panel
    /// </summary>
    private GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panelGO = new GameObject(name);
        panelGO.transform.SetParent(parent, false);

        // Add RectTransform
        RectTransform rectTransform = panelGO.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(600, 400);
        rectTransform.anchoredPosition = Vector2.zero;

        // Add Image for background
        Image image = panelGO.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black

        return panelGO;
    }

    /// <summary>
    /// Helper method to create menu text
    /// </summary>
    private void CreateMenuText(string text, Transform parent, Vector2 position)
    {
        GameObject textGO = new GameObject("MenuText");
        textGO.transform.SetParent(parent, false);

        // Add RectTransform
        RectTransform rectTransform = textGO.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400, 100);
        rectTransform.anchoredPosition = position;

        // Add TextMeshProUGUI
        TextMeshProUGUI textComponent = textGO.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.color = Color.white;
        textComponent.fontSize = 48;
    }

    /// <summary>
    /// Helper method to create a button
    /// </summary>
    private Button CreateButton(string name, Transform parent, Vector2 position, string buttonText)
    {
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(parent, false);

        // Add RectTransform
        RectTransform rectTransform = buttonGO.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(200, 60);
        rectTransform.anchoredPosition = position;

        // Add Image
        Image image = buttonGO.AddComponent<Image>();
        image.color = Color.white;

        // Add Button
        Button button = buttonGO.AddComponent<Button>();
        button.interactable = true;

        // Add Text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);

        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(200, 60);

        TextMeshProUGUI textComponent = textGO.AddComponent<TextMeshProUGUI>();
        textComponent.text = buttonText;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = Color.black;
        textComponent.fontSize = 24;

        return button;
    }
}
