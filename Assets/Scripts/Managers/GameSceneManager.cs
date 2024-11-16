using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace GolfCourse.Manager
{
    public class GameSceneManager : Singleton<GameSceneManager>
    {
        #region Constants

        public const string MainMenuSceneName = "MainMenuScene";
        public const string GameSceneName = "GameScene";

        #endregion

        #region Fields
        
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
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;
            while (!asyncLoad.isDone)
            {
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }
            
            _isTransitioning = false;
        }
        

        #endregion
    }
}
