using System.Collections;
using System.Collections.Generic;
using GolfCourse.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace GolfCourse.Manager
{
    public class GameManager : Singleton<GameManager>,IManager
    {
        [SerializeField] private ScoreZone scoreZone;
        [SerializeField] private InGameUI _inGameUI;
        public ScoreZone ScoreZone => scoreZone;
        public InGameUI InGameUI => _inGameUI;
    
        void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            NavMeshLinkManager.Instance.Initialize();
            GolfBallManager.Instance.Initialize();
            ScoreManager.Instance.Initialize();
            NPCManager.Instance.Initialize();

        }
    }
}

