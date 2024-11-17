using System.Collections;
using System.Collections.Generic;
using GolfCourse.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace GolfCourse.Manager
{
    public class GameManager : Singleton<GameManager>,IManager
    {
        #region Fields

        [SerializeField] private ScoreZone scoreZone;
        [SerializeField] private InGameUI _inGameUI;

        #endregion

        #region Properties

        public ScoreZone ScoreZone => scoreZone;
        public InGameUI InGameUI => _inGameUI;

        #endregion

        #region Unity Methods

        void Start()
        {
            Initialize();
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            NavMeshLinkManager.Instance.Initialize();
            GolfBallManager.Instance.Initialize();
            ScoreManager.Instance.Initialize();
            NPCManager.Instance.Initialize();

        }

        #endregion



    }
}

