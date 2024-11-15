// NPCs/States/CollectingState.cs

using UnityEngine;

public class CollectingState : INPCState
{
    #region Fields

    private NPCController _npc;
    private GolfBall _targetBall;

    #endregion

    #region Properties

    public NPCStateEnum StateType => NPCStateEnum.Collecting;

    #endregion

    #region State Lifecycle Methods

    public void EnterState(NPCController npc)
    {
        _npc = npc;
        _targetBall = _npc.Targeting.TargetGolfBall;

        if (_targetBall != null)
        {
            PrepareForCollection();
        }
        else
        {
            TransitionToSearchingState();
        }
    }

    public void UpdateState(NPCController npc)
    {
    }

    public void ExitState(NPCController npc)
    {
        if (_npc == null) return;

        CleanupCollectionState();
    }

    #endregion

    #region Collision Handlers

    public void OnTriggerEnter(NPCController npc, Collider other)
    {
    }

    public void OnTriggerExit(NPCController npc, Collider other)
    {
    }

    #endregion

    #region Private Methods

    private void PrepareForCollection()
    {
        _npc.Animator.SetMoving(false);

        _npc.Animator.OnCollectAnimationEnd += HandleCollectAnimatorEnd;
        _npc.Animator.OnCollectAnimationCollect += HandleCollectAnimationCollect;

        _npc.Animator.SetCollecting(true);
    }

    private void CleanupCollectionState()
    {
        // Unsubscribe from animation events
        _npc.Animator.OnCollectAnimationEnd -= HandleCollectAnimatorEnd;
        _npc.Animator.OnCollectAnimationCollect -= HandleCollectAnimationCollect;

        // Reset collecting state in animator
        _npc.Animator.SetCollecting(false);

        // Unparent the target ball and trigger its collection behavior
        if (_targetBall != null)
        {
            _targetBall.transform.SetParent(null);
            _targetBall.Collect();
        }

        // Clean up references to prevent memory issues
        _targetBall = null;
    }

    private void HandleCollectAnimationCollect()
    {
        if (_npc == null)
        {
            Debug.LogWarning("[CollectingState] NPCController reference is missing during collect animation.");
            return;
        }

        // Ensure the target ball is assigned correctly
        _targetBall = _npc.Targeting.TargetGolfBall;
        if (_targetBall != null)
        {
            AttachGolfBallToHand();
        }
        else
        {
            Debug.LogWarning("[CollectingState] TargetGolfBall is null during CollectingState collection phase.");
        }
    }

    private void HandleCollectAnimatorEnd()
    {
        if (_npc == null)
        {
            Debug.LogError("[CollectingState] NPCController reference is missing.");
            return;
        }

        // Clean up collection-related event handlers and transition to the next state
        _npc.Animator.OnCollectAnimationEnd -= HandleCollectAnimatorEnd;

        TransitionToReturningState();
    }

    private void AttachGolfBallToHand()
    {
        _targetBall.transform.SetParent(_npc.HandTransform);
        _targetBall.transform.localPosition = Vector3.zero;
    }

    private void TransitionToReturningState()
    {
        _npc.TransitionToState(new ReturningState(_targetBall?.Data));
    }

    private void TransitionToSearchingState()
    {
        _npc.TransitionToState(new SearchingState());
    }

    #endregion
}