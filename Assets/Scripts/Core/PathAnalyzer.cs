using UnityEngine;
using UnityEngine.AI;

public class PathAnalyzer
{
    public bool DestinationOnlyReachableViaLink(Vector3 start, Vector3 destination)
    {
        var navMeshLinks = NavMeshLinkManager.Instance.NavMeshLinks;
        
        foreach (var link in navMeshLinks)
        {
            link.enabled = false;
        }
        
        NavMeshPath path = new NavMeshPath();
        bool hasPath = NavMesh.CalculatePath(start, destination, NavMesh.AllAreas, path);
        
        foreach (var link in navMeshLinks)
        {
            link.enabled = true;
        }
        if (!hasPath || path.status != NavMeshPathStatus.PathComplete)
        {
            return true;
        }

        return false;
    }
}