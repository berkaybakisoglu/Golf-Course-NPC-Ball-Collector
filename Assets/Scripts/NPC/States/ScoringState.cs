using UnityEngine;
using System.Collections;
using GolfCourse.Manager;

public class ScoringState : INPCState
{
    public NPCStateEnum StateType => NPCStateEnum.Scoring;
    private GolfBallData _collectedData;
    private NPCController _npc;
    
    private float _rotationDuration = 0.67f; 
    private bool _isRotating = false;
    private Coroutine _rotatingCourutine;

    public ScoringState(GolfBallData data)
    {
        _collectedData = data;
    }

    public void OnTriggerEnter(NPCController npc, Collider other)
    {
        
    }

    public void OnTriggerExit(NPCController npc, Collider other)
    {
       
    }

    public void EnterState(NPCController npc)
    {
        _npc = npc;
        npc.Movement.StopMovement();
        _rotatingCourutine = _npc.StartCoroutine(RotateTowardsScoreZone());
    }

    private IEnumerator RotateTowardsScoreZone()
    {
        _isRotating = true;
        
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

        _isRotating = false;

        // After rotation, initiate scoring animations
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
            1, // Adjust force as needed
            1  // Adjust torque as needed
        );
    }

    public void UpdateState(NPCController npc)
    {
        // Optionally, handle any update logic here.
        // For example, ensure that rotation has completed before proceeding.
        // Currently handled by the coroutine.
    }

    public void ExitState(NPCController npc)
    {
        _npc.Animator.OnScoringAnimationThrowEnd -= ThrowBallIntoScoreZone;
        _npc.Animator.OnScoreAnimationEnd -= HandleScoreAnimationEnd;
        _npc.Animator.SetScoring(false);
        _npc = null;
    }
}
