using UnityEngine;

public class ScoringState : INPCState
{
    public NPCStateEnum StateType => NPCStateEnum.Scoring;
    private GolfBallData _collectedData;
    private NPCController _npc;
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
        npc.Animator.OnScoringAnimationThrowEnd += ThrowBallIntoScoreZone;
        npc.Animator.OnScoreAnimationEnd += HandleScoreAnimationEnd;
        npc.Animator.SetScoring(true);
    }
    
    private void HandleScoreAnimationEnd()
    {
        _npc.TransitionToState(new SearchingState());
    }

    private void ThrowBallIntoScoreZone()
    {
        Vector3 spawnPosition = _npc.HandTransform.position;
        GolfBall ballToThrow = GolfBallManager.Instance.SpawnAnimationGolfBall(_collectedData, spawnPosition);
        ballToThrow.ThrowGolfBallInto(spawnPosition, GameManager.Instance.ScoreZone.transform.position, 1,3);
    }

    public void UpdateState(NPCController npc)
    {
        // Implement any necessary update logic for the scoring state
        // For example, check if scoring animation is completed
    }

    public void ExitState(NPCController npc)
    {
        npc.Animator.OnScoringAnimationThrowEnd -= ThrowBallIntoScoreZone;
        npc.Animator.OnScoreAnimationEnd -= HandleScoreAnimationEnd;
        npc.Animator.SetScoring(false);
        // Clear the reference to npc to prevent potential memory leaks
        _npc = null;
    }
}