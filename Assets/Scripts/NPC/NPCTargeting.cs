using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class NPCTargeting : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (TargetGolfBall != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, TargetGolfBall.transform.position);
            Gizmos.DrawSphere(TargetGolfBall.transform.position, 0.5f);
        }

        if (GolfBallManager.Instance != null)
        {
            foreach (GolfBall ball in GolfBallManager.Instance.GetActiveGolfBalls())
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(ball.transform.position, 0.3f);
            }
        }
    }

    [Header("Scoring Weights")]
    [SerializeField] private float priorityWeight = 10f;
    [SerializeField] private float distanceWeight = 1f;
    [SerializeField] private float pathWeight = 0.5f;

    public GolfBall TargetGolfBall { get; private set; }
    private NPCController _npcController;
    private Dictionary<GolfBall, float> _distanceToScoringZone = new Dictionary<GolfBall, float>();
    private List<GolfBall> _availableGolfBalls = new List<GolfBall>();

    private Transform _scoreZone;
    private NavMeshPath _path;

    private void InitializeDistanceCache()
    {
        _distanceToScoringZone.Clear();
        foreach (GolfBall golfBall in _availableGolfBalls)
        {
            float distanceToScoringZone = Vector3.Distance(golfBall.transform.position, _scoreZone.position);
            _distanceToScoringZone[golfBall] = distanceToScoringZone;
        }
    }

    public void Initialize(NPCController npcController)
    {
        _npcController = npcController;
        _scoreZone = GameManager.Instance?.ScoreZone?.transform;
        _path = new NavMeshPath();
        _availableGolfBalls = GolfBallManager.Instance.GetActiveGolfBalls();
        InitializeDistanceCache();
        GolfBallManager.Instance.OnAvailableGolfBallsChanged += HandleAvailableGolfBallsChanged;
    }

    private void OnDestroy()
    {
        if (GolfBallManager.Instance != null)
        {
            GolfBallManager.Instance.OnAvailableGolfBallsChanged -= HandleAvailableGolfBallsChanged;
        }
    }

    private void HandleAvailableGolfBallsChanged(List<GolfBall> updatedGolfBalls)
    {
        _availableGolfBalls = updatedGolfBalls;
        InitializeDistanceCache();
        DecideNextTarget();
    }

    /// <summary>
    /// Decides the next target golf ball based on multiple factors.
    /// </summary>
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

        // Dynamic weight adjustment
        float dynamicPriorityWeight = priorityWeight * (1f + healthFactor); // Higher priority weight when health is high

        GolfBall bestBall = null;
        float bestScore = Mathf.NegativeInfinity;

        foreach (GolfBall golfBall in _availableGolfBalls)
        {
            float pathDistance = 0f;
            if (agent.CalculatePath(golfBall.transform.position, _path) && _path.corners.Length > 1)
            {
                for (int i = 1; i < _path.corners.Length; i++)
                {
                    pathDistance += Vector3.Distance(_path.corners[i - 1], _path.corners[i]);
                }
            }
            else
            {
                pathDistance = Vector3.Distance(currentPosition, golfBall.transform.position);
            }

            // Use cached distance to cart
            if (!_distanceToScoringZone.TryGetValue(golfBall, out float distanceToScoringZone))
            {
                // If not cached, calculate and cache it
                distanceToScoringZone = Vector3.Distance(golfBall.transform.position, _scoreZone.position);
                _distanceToScoringZone[golfBall] = distanceToScoringZone;
            }

            // Scoring formula with dynamic weights
            float score = CalculateScore(golfBall, pathDistance, distanceToScoringZone, healthFactor, dynamicPriorityWeight);

            if (score > bestScore)
            {
                bestScore = score;
                bestBall = golfBall;
            }
        }

        TargetGolfBall = bestBall;
    }

    private float CalculateScore(GolfBall golfBall, float pathDistance, float distanceToScoringZone, float healthFactor, float dynamicPriorityWeight)
    {
        return (golfBall.Data.PointValue * dynamicPriorityWeight)
               - (pathDistance * distanceWeight * (1f - healthFactor))
               - (distanceToScoringZone * pathWeight);
    }
}
