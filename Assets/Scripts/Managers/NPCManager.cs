using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GolfCourse.Manager
{
    public class NPCManager : Singleton<NPCManager>, IManager
    {
        public void Initialize()
        {
            GameObject npc = GameObject.FindGameObjectWithTag("NPC"); // maybe more later on?
            if (npc != null)
            {
                NPCController npcController = npc.GetComponent<NPCController>();
                if (npcController != null)
                {
                    npcController.Initialize();
                }
            }
            else
            {
                Debug.LogError("[NPCManager] NPC not found in the scene. Please ensure the NPC has the 'NPC' tag.");
            }
        }
    }
}
