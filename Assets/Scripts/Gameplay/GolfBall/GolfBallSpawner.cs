using System;
using System.Collections.Generic;
using GolfCourse.Manager;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GolfBallSpawner : MonoBehaviour
{
    #region Fields

    [SerializeField] private List<GolfBallData> _golfBallDataList;
    [SerializeField] private int _totalBallsToSpawn = 50;
    [Header("Level Determination Thresholds")]
    [SerializeField, Range(0f, 1f)] private float _level1PathCostThreshold = 0.35f;
    [SerializeField, Range(0f, 1f)] private float _level2PathCostThreshold = 0.7f;
    [SerializeField, Range(1f, 5f)] private float _distanceToObstacle = 1f;
    private float _ballSpawnOffsetY = 0.05f;
    private Terrain _terrain;
    private Transform _npcTransform;
    private Transform _scoringZoneTransform;
    private float _maxPathLength;
    private PathAnalyzer _pathAnalyzer;
    private Vector3 _closestNavMeshPointToScoreZone;
    private GolfBallManager _manager;
    public event Action<GolfBall> OnGolfBallSpawned;

    #endregion

    public void Initialize(GolfBallManager manager)
    {
        _manager = manager;
        _terrain = Terrain.activeTerrain; // maybe a terrain manager?
        if (_terrain == null)
        {
            Debug.LogError("[GolfBallSpawner] No active terrain found in the scene.");
            return;
        }
        Vector3 terrainSize = _terrain.terrainData.size;
        _maxPathLength = Vector3.Distance(_terrain.transform.position, _terrain.transform.position + new Vector3(terrainSize.x, 0, terrainSize.z));
        GameObject npc = GameObject.FindGameObjectWithTag("NPC");
        if (npc != null)
        {
            _npcTransform = npc.transform;
        }
        else
        {
            Debug.LogError("[GolfBallSpawner] NPC not found in the scene. Please ensure the NPC has the 'NPC' tag.");
            return;
        }

        _scoringZoneTransform = GameManager.Instance.ScoreZone.transform;
        FindClosestNavMeshPointToScoreZone();
        _pathAnalyzer = new PathAnalyzer();
        if (_pathAnalyzer == null)
        {
            Debug.LogError("[GolfBallSpawner] Failed to initialize PathAnalyzer.");
        }
    }
    

    #region Public Methods

    public void SpawnInitialGolfBalls()
    {
        for (int i = 0; i < _totalBallsToSpawn; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            if (spawnPosition != Vector3.zero)
            {
                int level = DetermineLevelBasedOnPosition(spawnPosition);
                GolfBallData selectedData = GetGolfBallDataByLevel(level);

                if (selectedData != null)
                {
                    spawnPosition.y += _ballSpawnOffsetY;
                    SpawnGolfBall(selectedData, spawnPosition);
                }
                else
                {
                    Debug.LogWarning($"[GolfBallSpawner] No golf ball data found for level {level}.");
                }
            }
            else
            {
                Debug.LogWarning("[GolfBallSpawner] Failed to find valid spawn position for golf ball.");
            }
        }
    }

    public void SpawnGolfBall(GolfBallData data, Vector3 position)
    {
        GolfBall golfBall = _manager.GolfBallPool.GetObject();
        golfBall.InitializeGolfBall(data, position);
        OnGolfBallSpawned?.Invoke(golfBall);
    }

    public GolfBall SpawnAnimationGolfBall(GolfBallData data, Vector3 position)
    {
        GolfBallData selectedData = data ?? GetGolfBallDataByLevel(DetermineLevelBasedOnPosition(position));
        GolfBall golfBall = _manager.GolfBallPool.GetObject();
        golfBall.InitializeGolfBall(selectedData, position);

        return golfBall;
    }

    #endregion

    #region Private Methods

    private int DetermineLevelBasedOnPosition(Vector3 position)
    {
        bool includesLink = false;
        float pathCost = CalculatePathCostToScoringZone(position, ref includesLink);

        float normalizedPathCost = pathCost / _maxPathLength;
        bool isNearObstacle = IsNearObstacle(position, _distanceToObstacle);
        
        if (includesLink || normalizedPathCost >= _level2PathCostThreshold)
        {
            return 3; 
        }

        if (normalizedPathCost >= _level1PathCostThreshold || isNearObstacle)
        {
            return 2; 
        }

        return 1;
    }


    private GolfBallData GetGolfBallDataByLevel(int level)
    {
        return _golfBallDataList.Find(data => data.Level == level);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        const int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomPosition = GetRandomPositionOnTerrain();

            if (IsValidSpawnPosition(randomPosition))
            {
                return randomPosition;
            }
        }

        return Vector3.zero;
    }

    private Vector3 GetRandomPositionOnTerrain()
    {
        Vector3 terrainSize = _terrain.terrainData.size;
        Vector3 terrainPosition = _terrain.transform.position;

        float randomX = Random.Range(terrainPosition.x, terrainPosition.x + terrainSize.x);
        float randomZ = Random.Range(terrainPosition.z, terrainPosition.z + terrainSize.z);
        float y = _terrain.SampleHeight(new Vector3(randomX, 0, randomZ)) + terrainPosition.y;

        return new Vector3(randomX, y, randomZ);
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        return IsFlatSurface(position) && IsPositionReachableByNPC(position); 
    }

    private void FindClosestNavMeshPointToScoreZone() // just to be sure its navmesh not blocking
    {
        NavMeshHit hit;
        float maxDistance = 100f;
        bool found = NavMesh.SamplePosition(_scoringZoneTransform.position, out hit, maxDistance, NavMesh.AllAreas);

        if (found)
        {
            _closestNavMeshPointToScoreZone = hit.position;
        }
    }
    private float CalculatePathCostToScoringZone(Vector3 position, ref bool includesLink)
    {
        includesLink = false;

        NavMeshPath path = new NavMeshPath();
        bool pathFound = NavMesh.CalculatePath(position, _closestNavMeshPointToScoreZone, NavMesh.AllAreas, path);

        if (pathFound && path.status == NavMeshPathStatus.PathComplete)
        {
            float pathLength = 0f;
            for (int i = 1; i < path.corners.Length; i++)
            {
                pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            
            includesLink = _pathAnalyzer.DestinationOnlyReachableViaLink(position, _closestNavMeshPointToScoreZone);

            return pathLength;
        }
        return Mathf.Infinity; 
    }



    private bool IsFlatSurface(Vector3 position)
    {
        Vector3 normal = GetTerrainNormal(position);
        return Vector3.Angle(normal, Vector3.up) < 5f; 
    }

    private Vector3 GetTerrainNormal(Vector3 position)
    {
        Vector3 terrainPosition = _terrain.transform.position;
        Vector3 terrainSize = _terrain.terrainData.size;

        float normalizedX = Mathf.InverseLerp(terrainPosition.x, terrainPosition.x + terrainSize.x, position.x);
        float normalizedZ = Mathf.InverseLerp(terrainPosition.z, terrainPosition.z + terrainSize.z, position.z);

        return _terrain.terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);
    }

    private bool IsNearObstacle(Vector3 position, float radius)
    {
        LayerMask obstacleLayerMask = LayerMask.GetMask("Obstacle");
        Collider[] colliders = Physics.OverlapSphere(position, radius, obstacleLayerMask);
        return colliders.Length > 0;
    }

    private bool IsPositionReachableByNPC(Vector3 position)
    {
        if (_npcTransform == null)
        {
            Debug.LogWarning("[GolfBallSpawner] NPC transform is null.");
            return false;
        }

        NavMeshPath path = new NavMeshPath();
        bool pathFound = NavMesh.CalculatePath(_npcTransform.position, position, NavMesh.AllAreas, path);

        return pathFound && path.status == NavMeshPathStatus.PathComplete;
    }

    #endregion
}
