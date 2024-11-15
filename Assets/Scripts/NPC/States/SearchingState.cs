// NPCs/States/SearchingState.cs
using UnityEngine;

public class SearchingState : INPCState
{
    public NPCStateEnum StateType => NPCStateEnum.Searching;
    public void OnTriggerEnter(NPCController npc, Collider other)
    {
        if (npc.Targeting.TargetGolfBall != null && other.gameObject == npc.Targeting.TargetGolfBall.gameObject)
        {
            npc.Movement.StopMovement();
            npc.TransitionToState(new CollectingState());
        }
    }


    public void OnTriggerExit(NPCController npc, Collider other)
    {
    
    }

    public void EnterState(NPCController npc)
    {
        // Decide on the next target
        npc.Targeting.DecideNextTarget();

        if (npc.Targeting.TargetGolfBall != null)
        {
            // Set destination to the target golf ball
            npc.Movement.SetDestination(npc.Targeting.TargetGolfBall.transform.position);
            npc.Animator.SetMoving(true);
        }
        else
        {
            // No golf balls available; NPC can idle or wait
            npc.Animator.SetMoving(false);
            Debug.Log("[SearchingState] No golf balls available.");
            ExitState(npc);
        }
    }

    public void UpdateState(NPCController npc)
    {

    }

    public void ExitState(NPCController npc)
    {
     
    }
}