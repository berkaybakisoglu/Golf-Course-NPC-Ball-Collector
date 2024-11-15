// GolfBalls/GolfBallSpawner.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GolfBallSpawner : MonoBehaviour
{
    #region Fields

    [SerializeField] private List<GolfBallData> _golfBallDataList;
    [SerializeField] private int _totalBallsToSpawn = 50;

    private Terrain _terrain;
    private Transform _npcTransform;
    private Transform _scoringZoneTransform;

    public event Action<GolfBall> OnGolfBallSpawned;

    #endregion

    #region Initialization

    public void Initialize()
    {
        _terrain = Terrain.activeTerrain;
        if (_terrain == null)
        {
            Debug.LogError("[GolfBallSpawner] No active terrain found in the scene.");
            return;
        }

        // Find the NPC in the scene
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
    }

    #endregion

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
        GolfBall golfBall = GolfBallPool.Instance.GetGolfBall();
        golfBall.InitializeGolfBall(data, position);
        OnGolfBallSpawned?.Invoke(golfBall);
    }

    public void SpawnGolfBallAtPosition(Vector3 position)
    {
        int level = DetermineLevelBasedOnPosition(position);
        GolfBallData selectedData = GetGolfBallDataByLevel(level);

        if (selectedData != null)
        {
            SpawnGolfBall(selectedData, position);
        }
        else
        {
            Debug.LogWarning($"[GolfBallSpawner] No golf ball data found for level {level}.");
        }
    }

    public GolfBall SpawnAnimationGolfBall(GolfBallData data, Vector3 position)
    {
        GolfBallData selectedData = data ?? GetGolfBallDataByLevel(DetermineLevelBasedOnPosition(position));
        GolfBall golfBall = GolfBallPool.Instance.GetGolfBall();
        golfBall.InitializeGolfBall(selectedData, position);

        // Placeholder for spawning with animation
        // Add animation logic here if needed

        return golfBall;
    }

    #endregion

    #region Private Methods

    private int DetermineLevelBasedOnPosition(Vector3 position)
    {
        bool nearObstacle10 = IsNearObstacle(position, 10f);
        bool nearObstacle5 = IsNearObstacle(position, 5f);
        bool nearObstacle2 = IsNearObstacle(position, 2f);
        bool farFromPlayer = IsFarFromPlayer(position, 50f);
        float pathCost = CalculatePathCostToScoringZone(position);

        if (!nearObstacle10 && pathCost < 20f)
        {
            return 1;
        }

        if (nearObstacle5 && pathCost < 40f)
        {
            return 2;
        }

        if (nearObstacle2 && farFromPlayer && pathCost >= 40f)
        {
            return 3;
        }

        return 1; // Default to Level 1
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
        
        float spawnHeightOffset = 0.1f; //todo this definetly needs to get this value from somewhere else
        y += spawnHeightOffset;

        return new Vector3(randomX, y, randomZ);
    }


    private bool IsValidSpawnPosition(Vector3 position)
    {
        return IsFlatSurface(position) && IsPositionReachableByNPC(position);
    }

    private float CalculatePathCostToScoringZone(Vector3 position)
    {
        if (_scoringZoneTransform == null)
        {
            Debug.LogWarning("[GolfBallSpawner] Scoring zone transform is null.");
            return Mathf.Infinity;
        }

        NavMeshPath path = new NavMeshPath();
        bool pathFound = NavMesh.CalculatePath(position, _scoringZoneTransform.position, NavMesh.AllAreas, path);

        if (pathFound && path.status == NavMeshPathStatus.PathComplete)
        {
            float pathLength = 0f;
            for (int i = 1; i < path.corners.Length; i++)
            {
                pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }

            return pathLength;
        }

        return Mathf.Infinity; // Path not reachable
    }

    private bool IsFlatSurface(Vector3 position)
    {
        Vector3 normal = GetTerrainNormal(position);
        return Vector3.Angle(normal, Vector3.up) < 5f; // Threshold for flatness
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

    private bool IsFarFromPlayer(Vector3 position, float minDistance)
    {
        if (_npcTransform == null)
        {
            return false;
        }

        float distance = Vector3.Distance(position, _npcTransform.position);
        return distance >= minDistance;
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
