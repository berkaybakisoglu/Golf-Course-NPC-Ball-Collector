using System;
using UnityEngine;

namespace GolfCourse.Manager
{
    public class ScoreManager : Singleton<ScoreManager>, IManager
    {
        #region Fields

        private int _score;

        #endregion

        #region Events

        public event Action<int> OnScoreChanged;

        #endregion

        #region Properties

        public int CurrentScore => _score;

        #endregion

        #region Public Methods

        public void Initialize()
        {
            SetScore(0);

        }

        public void AddScore(int score)
        {
            _score += score;
            OnScoreChanged?.Invoke(_score);
        }

        public void ResetScore()
        {
            _score = 0;
            OnScoreChanged?.Invoke(_score);
        }

        private void SetScore(int score)
        {
            _score = score;
            OnScoreChanged?.Invoke(_score);
        }

        #endregion
    }
}