// Scripts/InGameUI.cs
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Button _exitButton;
    [SerializeField] private RectTransform scoreTextRectTransform; // Assign the RectTransform of the score text UI
    [SerializeField] private FloatingScoreText _floatingScoreText;
    private int _currentDisplayedScore = 0;
    private Tween _scoreTween; // Reference to the active tween

    private void Start()
    {
        _exitButton.onClick.AddListener(OnExitButtonClicked);
        _scoreText.text = "Score: 0";
        ScoreManager.Instance.OnScoreChanged += UpdateScore;
    }

    private void UpdateScore(int newScore)
    {
        // Kill existing tween if any to prevent overlapping
        if (_scoreTween != null && _scoreTween.IsActive())
        {
            _scoreTween.Kill();
        }

        // Start a new tween to animate the score
        _scoreTween = DOTween.To(() => _currentDisplayedScore, x => {
                _currentDisplayedScore = x;
                _scoreText.text = "Score: " + x;
            }, newScore, 0.5f)
            .SetEase(Ease.OutCubic);
    }


    private void OnExitButtonClicked()
    {
        // Exit the game or load the main menu
        GameSceneManager.Instance.LoadScene(GameSceneManager.MainMenuSceneName);
    }

    /// <summary>
    /// Spawns a floating text that moves towards the score UI.
    /// </summary>
    /// <param name="text">The text to display (e.g., "+10").</param>
    /// <param name="worldPosition">The world position where the score was collected.</param>
    public void SpawnFloatingText(string text, Vector3 worldPosition,Action onComplete)
    {
        // Convert the world position to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        // If the position is behind the camera, clamp it to the center
        if (screenPos.z < 0)
        {
            screenPos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }
        Vector3 targetPos = scoreTextRectTransform.position;
        _floatingScoreText.Initialize(text, screenPos, targetPos,onComplete);
    }
}
