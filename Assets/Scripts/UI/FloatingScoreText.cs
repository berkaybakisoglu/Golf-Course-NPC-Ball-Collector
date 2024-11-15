using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingScoreText : MonoBehaviour 
{
    #region Properties

    public RectTransform RectTransform => _rectTransform;

    #endregion

    #region Fields

    [Header("Animation Settings")]
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private Vector3 _moveOffset = new Vector3(0, 100, 0);
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private float _startScale = 1f;
    [SerializeField] private float _endScale = 2f;

    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _rectTransform;

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the floating text with specific parameters and starts the animation.
    /// </summary>
    /// <param name="text">The text to display (e.g., "+10").</param>
    /// <param name="startPos">The starting screen position.</param>
    /// <param name="targetPos">The target screen position (e.g., score UI position).</param>
    /// <param name="onComplete">Callback invoked upon animation completion.</param>
    public void Initialize(string text, Vector3 startPos, Vector3 targetPos, Action onComplete)
    {
        _text.text = text;
        _rectTransform.position = startPos;
        _rectTransform.localScale = Vector3.one * _startScale;
        _canvasGroup.alpha = 1f;

        AnimateFloatingText(targetPos, onComplete);
    }

    #endregion

    #region Private Methods

    private void AnimateFloatingText(Vector3 targetPos, Action onComplete)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_rectTransform.DOMove(targetPos, _moveDuration).SetEase(Ease.InOutQuad));
        sequence.Join(_canvasGroup.DOFade(0f, _fadeDuration).SetEase(Ease.InOutQuad));
        sequence.Join(_rectTransform.DOScale(_endScale, _moveDuration).SetEase(Ease.InOutQuad));
        sequence.OnComplete(() =>
        {
            ResetFloatingText();
            onComplete?.Invoke();
        });
    }

    private void ResetFloatingText()
    {
        _text.text = "";
        _rectTransform.localScale = Vector3.one * _startScale;
        _canvasGroup.alpha = 0f;

        DOTween.Kill(transform);
    }

    #endregion
}
