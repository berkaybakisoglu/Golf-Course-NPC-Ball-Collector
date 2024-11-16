using System;
using DG.Tweening;
using GolfCourse.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GolfCourse.UI
{
    public class InGameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Button _exitButton;
        [SerializeField] private RectTransform scoreTextRectTransform;
        [SerializeField] private FloatingScoreText _floatingScoreText;
        private int _currentDisplayedScore = 0;
        private Tween _scoreTween;

        private void Start()
        {
            _exitButton.onClick.AddListener(OnExitButtonClicked);
            _scoreText.text = "Score: 0";
            ScoreManager.Instance.OnScoreChanged += UpdateScore;
        }

        private void UpdateScore(int newScore)
        {
            if (_scoreTween != null && _scoreTween.IsActive())
            {
                _scoreTween.Kill();
            }

            _scoreTween = DOTween.To(() => _currentDisplayedScore, x =>
                {
                    _currentDisplayedScore = x;
                    _scoreText.text = "Score: " + x;
                }, newScore, 0.5f)
                .SetEase(Ease.OutCubic);
        }


        private void OnExitButtonClicked()
        {
            GameSceneManager.Instance.LoadScene(GameSceneManager.MainMenuSceneName);
        }

        public void SpawnFloatingText(string text, Vector3 worldPosition, Action onComplete)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

            if (screenPos.z < 0)
            {
                screenPos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            }

            Vector3 targetPos = scoreTextRectTransform.position;
            _floatingScoreText.Initialize(text, screenPos, targetPos, onComplete);
        }
    }
}
