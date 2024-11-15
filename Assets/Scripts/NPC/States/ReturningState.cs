// NPCs/States/ReturningState.cs
using UnityEngine;

public class ReturningState : INPCState
{
    public NPCStateEnum StateType => NPCStateEnum.Returning;
    public ReturningState(GolfBallData data)
    {
        _dropOffPoint = Vector3.zero;
        _collectedData = data;
    }

    private Vector3 _dropOffPoint;
    private GolfBallData _collectedData;

    public void EnterState(NPCController npc)
    {
        if (GameManager.Instance.ScoreZone != null)
        {
            _dropOffPoint = GameManager.Instance.ScoreZone.transform.position;
            npc.Movement.SetDestination(_dropOffPoint);
            npc.Animator.SetMoving(true);
        }
        else
        {
            Debug.LogError("[ReturningState] DropOffPoint not found in the scene.");
            npc.TransitionToState(new SearchingState());
        }
    }

    public void UpdateState(NPCController npc)
    {
    }

    public void ExitState(NPCController npc)
    {
        npc.Animator.SetMoving(false);
    }

    public void OnTriggerEnter(NPCController npc, Collider other)
    {
        if (other.CompareTag("ScoreTarget"))
        {
            npc.Movement.StopMovement();
            npc.TransitionToState(new ScoringState(_collectedData));
        }
    }

    public void OnTriggerExit(NPCController npc, Collider other)
    {
    }
}