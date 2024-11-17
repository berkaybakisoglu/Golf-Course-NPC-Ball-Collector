using UnityEngine;
using System.Collections;
using GolfCourse.Manager;

namespace GolfCourse.NPC.State
{
    public class ScoringState : INPCState
    {
        #region Fields

        private GolfBallData _collectedData;
        private NPCController _npc;

        private float _rotationDuration = 0.67f;
        private Coroutine _rotatingCoroutine;

        #endregion

        #region Properties

        public NPCStateEnum StateType => NPCStateEnum.Scoring;

        #endregion

        #region Constructors

        public ScoringState(GolfBallData data)
        {
            _collectedData = data;
        }

        #endregion

        #region State Lifecycle Methods

        public void EnterState(NPCController npc)
        {
            _npc = npc;
            npc.Movement.StopMovement();
            _rotatingCoroutine = _npc.StartCoroutine(RotateTowardsScoreZone());
        }

        public void UpdateState(NPCController npc)
        {
        }

        public void ExitState(NPCController npc)
        {
            _npc.Animator.OnScoringAnimationThrowEnd -= ThrowBallIntoScoreZone;
            _npc.Animator.OnScoreAnimationEnd -= HandleScoreAnimationEnd;
            _npc.Animator.SetScoring(false);
            _npc = null;
        }

        #endregion

        #region Unity Methods

        public void OnTriggerEnter(NPCController npc, Collider other)
        {
            
        }

        public void OnTriggerExit(NPCController npc, Collider other)
        {
          
        }

        #endregion

        #region Private Methods

        private IEnumerator RotateTowardsScoreZone()
        {
            Vector3 direction = GameManager.Instance.ScoreZone.transform.position - _npc.transform.position;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion initialRotation = _npc.transform.rotation;
                float elapsed = 0f;

                while (elapsed < _rotationDuration)
                {
                    _npc.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / _rotationDuration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                _npc.transform.rotation = targetRotation;
            }
            
            InitializeScoringAnimations();
        }

        private void InitializeScoringAnimations()
        {
            _npc.Animator.OnScoringAnimationThrowEnd += ThrowBallIntoScoreZone;
            _npc.Animator.OnScoreAnimationEnd += HandleScoreAnimationEnd;
            _npc.Animator.SetScoring(true);
        }

        private void HandleScoreAnimationEnd()
        {
            _npc.TransitionToState(new SearchingState());
        }

        private void ThrowBallIntoScoreZone()
        {
            Vector3 spawnPosition = _npc.HandTransform.position;
            GolfBall ballToThrow = GolfBallManager.Instance.SpawnAnimationGolfBall(_collectedData, spawnPosition);
            ballToThrow.ThrowGolfBallInto(
                spawnPosition,
                GameManager.Instance.ScoreZone.transform.position,
                1, // they can get from somewhere else to arrangable
                1 
            );
        }

        #endregion
    }
}
