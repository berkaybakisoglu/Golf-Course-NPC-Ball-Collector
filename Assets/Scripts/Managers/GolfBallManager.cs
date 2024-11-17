using System;
using System.Collections.Generic;
using UnityEngine;

namespace GolfCourse.Manager
{
    public class GolfBallManager : Singleton<GolfBallManager>, IManager
    {

        #region Properties

        public ObjectPool<GolfBall> GolfBallPool => _golfBallPool;
        public Action<List<GolfBall>> OnAvailableGolfBallsChanged;

        #endregion
        
        #region Fields

        private List<GolfBall> _availableGolfBalls = new List<GolfBall>();
        private GolfBallSpawner _golfBallSpawner;
        private ObjectPool<GolfBall> _golfBallPool;
        [SerializeField] private GolfBall _golfBallPrefab;
        [SerializeField] private int _golfPoolSize = 25;

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

            _golfBallPool = new ObjectPool<GolfBall>(_golfBallPrefab, _golfPoolSize);
            _golfBallSpawner.OnGolfBallSpawned += RegisterGolfBall;
            _golfBallSpawner.Initialize(this);
            _golfBallSpawner.SpawnInitialGolfBalls();
            _golfBallSpawner.OnGolfBallSpawned -= RegisterGolfBall;
        }

        private void RegisterGolfBall(GolfBall golfBall)
        {
            if (!_availableGolfBalls.Contains(golfBall))
            {
                _availableGolfBalls.Add(golfBall);
            }
        }

        public void UnregisterGolfBall(GolfBall golfBall)
        {
            if (_availableGolfBalls.Remove(golfBall))
            {
                OnAvailableGolfBallsChanged?.Invoke(_availableGolfBalls);
            }

            _golfBallPool.ReturnObject(golfBall);
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
}
