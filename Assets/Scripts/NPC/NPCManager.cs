using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optionally, prevent destruction on scene load
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Initialize()
    {
        // Find the player in the scene
        GameObject npc = GameObject.FindGameObjectWithTag("NPC");
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
