using System.Collections.Generic;
using GolfCourse.Manager;
using UnityEngine;
using UnityEngine.AI;

namespace GolfCourse.NPC
{
    public class NPCTargeting : MonoBehaviour
    {
        #region Fields

        [Header("Scoring Weights")]
        [SerializeField]
        private float priorityWeight = 10f; // A decision SO might be much better but for testing

        [SerializeField]
        private float distanceWeight = 1f;

        [SerializeField]
        private float pathWeight = 0.5f;

        private NPCController _npcController;
        private Dictionary<GolfBall, float> _distanceToScoringZone = new Dictionary<GolfBall, float>();
        private List<GolfBall> _availableGolfBalls = new List<GolfBall>();

        private Transform _scoreZone;
        private NavMeshPath _path;
        
        #endregion

        #region Public Properties

        public GolfBall TargetGolfBall { get; private set; }

        #endregion
        

        #region Unity Events

        private void OnDestroy()
        {
            if (GolfBallManager.Instance != null)
            {
                GolfBallManager.Instance.OnAvailableGolfBallsChanged -= HandleAvailableGolfBallsChanged;
            }
        }

        #endregion
        
        public void Initialize(NPCController npcController,Transform scoreZone)
        {
            _npcController = npcController;
            _scoreZone = scoreZone;
            _path = new NavMeshPath();
            _availableGolfBalls = GolfBallManager.Instance.GetActiveGolfBalls();
            InitializeDistanceCache();

            if (GolfBallManager.Instance != null)
            {
                GolfBallManager.Instance.OnAvailableGolfBallsChanged += HandleAvailableGolfBallsChanged;
            }
        }

        private void InitializeDistanceCache()
        {
            _distanceToScoringZone.Clear();

            foreach (var golfBall in _availableGolfBalls)
            {
                float distance = Vector3.Distance(golfBall.transform.position, _scoreZone.position);
                _distanceToScoringZone[golfBall] = distance;
            }
        }
        

        #region Event Handlers

        private void HandleAvailableGolfBallsChanged(List<GolfBall> updatedGolfBalls)
        {
            _availableGolfBalls = updatedGolfBalls;
            InitializeDistanceCache();
            DecideNextTarget();
        }

        #endregion

        #region Public Methods

        public void DecideNextTarget()
        {
            if (_npcController == null || _availableGolfBalls == null || _availableGolfBalls.Count == 0)
            {
                TargetGolfBall = null;
                return;
            }

            float healthFactor = _npcController.HealthController.CurrentHealth / _npcController.HealthController.MaxHealth;
            Vector3 currentPosition = transform.position;
            NavMeshAgent agent = _npcController.Movement.Agent;
            
            float dynamicPriorityWeight = priorityWeight * (1f + healthFactor);

            GolfBall bestBall = null;
            float bestScore = Mathf.NegativeInfinity;

            foreach (var golfBall in _availableGolfBalls)
            {
                float pathDistance = CalculatePathDistance(agent, golfBall.transform.position, currentPosition);
                if (!_distanceToScoringZone.TryGetValue(golfBall, out float distanceToScoringZone))
                {
                    distanceToScoringZone = Vector3.Distance(golfBall.transform.position, _scoreZone.position);
                    _distanceToScoringZone[golfBall] = distanceToScoringZone;
                }
                
                float score = CalculateScore(golfBall, pathDistance, distanceToScoringZone, healthFactor, dynamicPriorityWeight);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestBall = golfBall;
                }
            }

            TargetGolfBall = bestBall;
        }

        #endregion

        #region Private Methods

        private float CalculatePathDistance(NavMeshAgent agent, Vector3 targetPosition, Vector3 currentPosition)
        {
            float pathDistance = 0f;

            if (agent.CalculatePath(targetPosition, _path) && _path.corners.Length > 1)
            {
                for (int i = 1; i < _path.corners.Length; i++)
                {
                    pathDistance += Vector3.Distance(_path.corners[i - 1], _path.corners[i]);
                }
            }
            else
            {
                pathDistance = Vector3.Distance(currentPosition, targetPosition);
            }

            return pathDistance;
        }

        private float CalculateScore(GolfBall golfBall, float pathDistance, float distanceToScoringZone, float healthFactor, float dynamicPriorityWeight)
        {
            return (golfBall.Data.PointValue * dynamicPriorityWeight)
                   - (pathDistance * distanceWeight * (1f - healthFactor))
                   - (distanceToScoringZone * pathWeight);
        }

        #endregion
    }
}
