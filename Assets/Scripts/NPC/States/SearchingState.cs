// NPCs/States/SearchingState.cs
using UnityEngine;

public class SearchingState : INPCState
{
    public NPCStateEnum StateType => NPCStateEnum.Searching;
    public void OnTriggerEnter(NPCController npc, Collider other)
    {
        
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
        if (npc.Movement.HasReachedDestination())
        {
            if (npc.Targeting.TargetGolfBall != null)
            {
                // Transition to CollectingState
                npc.TransitionToState(new CollectingState());
            }
            else
            {
                // Re-decide on the next target
                npc.Targeting.DecideNextTarget();
                if (npc.Targeting.TargetGolfBall != null)
                {
                    npc.Movement.SetDestination(npc.Targeting.TargetGolfBall.transform.position);
                }
                else
                {
                    // No golf balls available; remain in current state or transition to IdleState
                }
            }
        }
    }

    public void ExitState(NPCController npc)
    {
        // Clean up if necessary
    }
}