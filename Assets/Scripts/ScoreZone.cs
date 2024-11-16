using System.Collections.Generic;
using GolfCourse.Manager;
using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    #region Fields

    private List<GolfBall> _collectedGolfBalls = new List<GolfBall>();

    #endregion
    
    #region Public Methods

    public void CollectedGolfBall(GolfBall golfBall)
    {
        _collectedGolfBalls.Add(golfBall);
        GameManager.Instance.InGameUI.SpawnFloatingText("+" + golfBall.Data.PointValue, golfBall.transform.position, () =>
        {
            ScoreManager.Instance.AddScore(golfBall.Data.PointValue);
        });
    }

    #endregion
}