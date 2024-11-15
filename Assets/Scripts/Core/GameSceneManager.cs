// Managers/GameSceneManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSceneManager : Singleton<GameSceneManager>
{
    #region Constants

    public const string MainMenuSceneName = "MainMenuScene";
    public const string GameSceneName = "GameScene";

    #endregion
    
    #region Fields

    [SerializeField] private CanvasGroup _fadeCanvasGroup;
    [SerializeField] private float _fadeDuration = 1f;

    private bool _isTransitioning;

    #endregion

    #region Public Methods
    
    public void LoadScene(string sceneName)
    {
        if (!_isTransitioning)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }
    }

    #endregion

    #region Private Methods

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    
    private IEnumerator LoadSceneRoutine(string sceneName) //todo would be much better with a proper loading screen
    {
        _isTransitioning = true;
        
        yield return StartCoroutine(Fade(1f));
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            // If the loading is almost complete
            if (asyncLoad.progress >= 0.9f)
            {
                // Finish loading the scene
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
        yield return StartCoroutine(Fade(0f));
        _isTransitioning = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = _fadeCanvasGroup.alpha;
        float timer = 0f;

        while (timer <= _fadeDuration)
        {
            _fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / _fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        _fadeCanvasGroup.alpha = targetAlpha;
    }

    #endregion
}
