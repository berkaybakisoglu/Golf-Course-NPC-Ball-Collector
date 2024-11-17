using System;
using UnityEngine;

namespace GolfCourse.NPC
{
    [RequireComponent(typeof(Animator))]
    public class NPCAnimator : MonoBehaviour
    {
        #region Fields

        private Animator _animator;
        
        private int _isMovingHash;
        private int _isCollectingHash;
        private int _isScoringHash;
        private int _isDeadHash;
        private int _isJumpingHash;
        private int _speedHash;

        #endregion

        #region Properties

        
        public event Action OnCollectAnimationEnd;
        public event Action OnCollectAnimationCollect;
        public event Action OnScoreAnimationEnd;
        public event Action OnScoringAnimationThrowEnd;

        public event Action OnJumpAirEnd;
        public event Action OnJumpLandEnd;
        
        #endregion


        #region Unity Methods

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _isMovingHash = Animator.StringToHash("isMoving");
            _isCollectingHash = Animator.StringToHash("isCollecting");
            _isScoringHash = Animator.StringToHash("isScoring");
            _isDeadHash = Animator.StringToHash("isDead");
            _isJumpingHash = Animator.StringToHash("isJumping");
            _speedHash = Animator.StringToHash("speed");
        }

        #endregion

        #region Public Methods

        public void SetIdle()
        {
            _animator.SetBool(_isMovingHash, false);
            _animator.SetBool(_isScoringHash, false);
            _animator.SetBool(_isCollectingHash, false);
            _animator.SetBool(_isJumpingHash, false);
        }
        
        public void SetMoving(bool isMoving)
        {
            _animator.SetBool(_isMovingHash, isMoving);
        }
        
        public void SetCollecting(bool isCollecting)
        {
            _animator.SetBool(_isCollectingHash, isCollecting);
        }

        public void SetDead()
        {
            _animator.SetTrigger(_isDeadHash);
        }

        public void SetJumping()
        {
            _animator.SetTrigger(_isJumpingHash);
        }

        public void SetSpeed(float speed)
        {
            _animator.SetFloat(_speedHash, speed);
        }
        
        public void OnCollectAnimationEnded()
        {
            OnCollectAnimationEnd?.Invoke();
        }

        public void OnCollectAnimationCollectEnded()
        {
            OnCollectAnimationCollect?.Invoke();
        }

        public void OnScoreAnimationEnded()
        {
            OnScoreAnimationEnd?.Invoke();
        }

        public void OnScoringAnimationThrowEnded()
        {
            OnScoringAnimationThrowEnd?.Invoke();
        }

        public void OnJumpLanded()
        {
            OnJumpLandEnd?.Invoke();
        }

        public void OnJumpAired()
        {
            OnJumpAirEnd?.Invoke();
        }
        
        public void SetScoring(bool isScoring)
        {
            _animator.SetBool(_isScoringHash, isScoring);
        }

        #endregion


    }
}
