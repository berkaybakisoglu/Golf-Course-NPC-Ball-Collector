using UnityEngine;

namespace GolfCourse.NPC.State
{
    public class SearchingState : INPCState
    {
        #region Properties

        public NPCStateEnum StateType => NPCStateEnum.Searching;

        #endregion


        #region State Lifecycle Methods

        public void EnterState(NPCController npc)
        {
            npc.Targeting.DecideNextTarget();

            if (npc.Targeting.TargetGolfBall != null)
            {
                npc.Movement.SetDestination(npc.Targeting.TargetGolfBall.transform.position);
                npc.Animator.SetMoving(true);
            }
            else
            {
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

        #endregion

        #region Unity Methods

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

        #endregion

        #region Private Methods

        #endregion
    }
}