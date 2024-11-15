using System;
using System.Collections.Generic;
using UnityEngine;

public class GolfBallManager : Singleton<GolfBallManager>
{
    #region Events

    public Action<List<GolfBall>> OnAvailableGolfBallsChanged;

    #endregion

    #region Fields

    private List<GolfBall> _availableGolfBalls = new List<GolfBall>();
    private GolfBallSpawner _golfBallSpawner;
    private GolfBallPool _golfBallPool;

    #endregion

    #region Public Methods
    public void Initialize()
    {
        _golfBallSpawner = FindObjectOfType<GolfBallSpawner>();
        if (_golfBallSpawner == null)
        {
            Debug.LogError("[GolfBallManager] No GolfBallSpawner found in the scene.");
            return;
        }

        _golfBallPool = GolfBallPool.Instance;
        if (_golfBallPool == null)
        {
            Debug.LogError("[GolfBallManager] No GolfBallPool found in the scene.");
            return;
        }

        _golfBallSpawner.OnGolfBallSpawned += RegisterGolfBall;
        _golfBallSpawner.Initialize();
        _golfBallSpawner.SpawnInitialGolfBalls();
        _golfBallSpawner.OnGolfBallSpawned -= RegisterGolfBall;
    }
    public void RegisterGolfBall(GolfBall golfBall)
    {
        if (!_availableGolfBalls.Contains(golfBall))
        {
            _availableGolfBalls.Add(golfBall);
            // Optional: Subscribe to golfBall events if needed
            // golfBall.OnCollected += UnregisterGolfBall;
        }
    }

    public void UnregisterGolfBall(GolfBall golfBall)
    {
        if (_availableGolfBalls.Remove(golfBall))
        {
            OnAvailableGolfBallsChanged?.Invoke(_availableGolfBalls);
            // Optional: Unsubscribe from golfBall events if needed
            // golfBall.OnCollected -= UnregisterGolfBall;
        }

        _golfBallPool.ReturnGolfBall(golfBall);
    }

    public void SpawnGolfBallAtPosition(Vector3 position)
    {
        _golfBallSpawner?.SpawnGolfBallAtPosition(position);
    }

    public void SpawnGolfBall(GolfBallData data, Vector3 position)
    {
        _golfBallSpawner?.SpawnGolfBall(data, position);
    }

    public GolfBall SpawnAnimationGolfBall(GolfBallData data, Vector3 position)
    {
        return _golfBallSpawner?.SpawnAnimationGolfBall(data, position);
    }

    public List<GolfBall> GetActiveGolfBalls()
    {
        return _availableGolfBalls;
    }

    #endregion
    
}
