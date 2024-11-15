using UnityEngine;

public interface INPCState
{
    void EnterState(NPCController npc);
    void UpdateState(NPCController npc);
    void ExitState(NPCController npc);
    
    NPCStateEnum StateType { get; }
    
    void OnTriggerEnter(NPCController npc, Collider other);
    void OnTriggerExit(NPCController npc, Collider other);
}

public enum NPCStateEnum
{
    Idling,
    Searching,
    Collecting,
    Returning,
    Scoring,
    Dead

}