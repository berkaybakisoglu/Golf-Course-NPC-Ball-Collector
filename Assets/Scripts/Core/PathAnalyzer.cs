using UnityEngine;
using UnityEngine.AI;

public class PathAnalyzer
{
    private const float Epsilon = 1e-4f;

    /// <summary>
    /// Determines if the destination is only reachable via traversing NavMeshLinks by temporarily disabling them.
    /// </summary>
    public bool DestinationOnlyReachableViaLink(Vector3 start, Vector3 destination)
    {
        // Get all NavMeshLinks
        var navMeshLinks = NavMeshLinkManager.Instance.NavMeshLinks;

        // Disable all links
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