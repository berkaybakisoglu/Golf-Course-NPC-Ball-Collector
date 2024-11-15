using UnityEngine;

public class IdlingState : INPCState
{
    public NPCStateEnum StateType => NPCStateEnum.Idling;
    public void EnterState(NPCController npc)
    {
        npc.Animator.SetIdle();
        
    }

    public void UpdateState(NPCController npc)
    {
    }

    public void ExitState(NPCController npc)
    {
  
    }
    public void OnTriggerEnter(NPCController npc, Collider other)
    {
      
    }

    public void OnTriggerExit(NPCController npc, Collider other)
    {
     
    }
}
