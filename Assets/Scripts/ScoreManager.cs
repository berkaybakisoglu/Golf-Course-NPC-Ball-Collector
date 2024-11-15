using System;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
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
    
    public void SetScore(int score)
    {
        _score = score;
        OnScoreChanged?.Invoke(_score);
    }

    #endregion
}