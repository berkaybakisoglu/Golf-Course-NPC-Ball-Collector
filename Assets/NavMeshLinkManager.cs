// Managers/NavMeshLinkManager.cs
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshLinkManager : Singleton<NavMeshLinkManager>,IManager
{
    private List<NavMeshLink> _navMeshLinks = new List<NavMeshLink>();
    public IReadOnlyList<NavMeshLink> NavMeshLinks => _navMeshLinks.AsReadOnly(); // should never be updated?
    public void Initialize() // maybe serializeField beside find?
    {
        _navMeshLinks.AddRange(GameObject.FindObjectsOfType<NavMeshLink>());
    }
}