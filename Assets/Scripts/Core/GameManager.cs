using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private ScoreZone _scoreZone;
    [SerializeField] private InGameUI _inGameUI;
    public ScoreZone ScoreZone => _scoreZone;
    public InGameUI InGameUI => _inGameUI;
    
    void Start()
    {
        NavMeshLinkManager.Instance.Initialize();
        GolfBallManager.Instance.Initialize();
        NPCManager.Instance.Initialize();
    }
    
}
