using UnityEngine;

namespace GolfCourse.NPC.State
{
    public class DeadState : INPCState
    {
        #region Fields

        private NPCController _npc;

        #endregion

        #region Properties

        public NPCStateEnum StateType => NPCStateEnum.Dead;

        #endregion

        #region State Lifecycle Methods

        public void EnterState(NPCController npc)
        {
            _npc = npc;
            _npc.Movement.StopMovement();
            _npc.Animator.SetDead();
        }

        public void UpdateState(NPCController npc)
        {

        }

        public void ExitState(NPCController npc)
        {
            _npc.StopAllCoroutines();
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
    }
}