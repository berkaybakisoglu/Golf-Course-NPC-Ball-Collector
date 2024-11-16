using UnityEngine;
using System.Collections.Generic;

public class GolfBallPool : Singleton<GolfBallPool>
{
    #region Fields

    [SerializeField] private GolfBall _golfBallPrefab;
    [SerializeField] private int _initialPoolSize = 50;

    private readonly Queue<GolfBall> _pool = new Queue<GolfBall>();

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        InitializePool();
    }

    #endregion

    #region Public Methods
    public GolfBall GetGolfBall()
    {
        GolfBall golfBall = _pool.Count > 0 ? _pool.Dequeue() : Instantiate(_golfBallPrefab);
        golfBall.name = $"GolfBall_{_pool.Count}";
        golfBall.gameObject.SetActive(true);
        return golfBall;
    }
    
    public void ReturnGolfBall(GolfBall golfBall)
    {
        golfBall.gameObject.SetActive(false);
        _pool.Enqueue(golfBall);
    }

    #endregion

    #region Private Methods

    private void InitializePool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            GolfBall golfBall = Instantiate(_golfBallPrefab);
            golfBall.name = $"GolfBall_{i}";
            golfBall.gameObject.SetActive(false);
            _pool.Enqueue(golfBall);
        }
    }

    #endregion
}