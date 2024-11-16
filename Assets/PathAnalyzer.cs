// Utilities/PathAnalyzer.cs

using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class PathAnalyzer
{
    public PathAnalyzer()
    {
    }

    public bool PathIncludesNavMeshLink(NavMeshPath path)
    {
        foreach (var link in NavMeshLinkManager.Instance.NavMeshLinks)
        {
            if (PathIntersectsLink(path, link))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the given path intersects with a specific NavMeshLink.
    /// </summary>
    /// <param name="path">The NavMeshPath.</param>
    /// <param name="link">The NavMeshLink to check against.</param>
    /// <returns>True if the path intersects with the link; otherwise, false.</returns>
    private bool PathIntersectsLink(NavMeshPath path, NavMeshLink link)
    {
        Vector3 linkStart = link.startPoint + link.transform.position;
        Vector3 linkEnd = link.endPoint + link.transform.position;

        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 segmentStart = path.corners[i - 1];
            Vector3 segmentEnd = path.corners[i];

            if (LinesIntersect(segmentStart, segmentEnd, linkStart, linkEnd))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines if two line segments intersect in the XZ plane.
    /// </summary>
    private bool LinesIntersect(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
    {
        // Project onto XZ plane for 2D intersection
        Vector2 p1 = new Vector2(a1.x, a1.z);
        Vector2 p2 = new Vector2(a2.x, a2.z);
        Vector2 p3 = new Vector2(b1.x, b1.z);
        Vector2 p4 = new Vector2(b2.x, b2.z);

        return LinesIntersect2D(p1, p2, p3, p4);
    }

    /// <summary>
    /// Determines if two 2D line segments intersect.
    /// </summary>
    private bool LinesIntersect2D(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float d1 = Direction(p3, p4, p1);
        float d2 = Direction(p3, p4, p2);
        float d3 = Direction(p1, p2, p3);
        float d4 = Direction(p1, p2, p4);

        if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
            ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Calculates the direction of point pk relative to the line formed by pi and pj.
    /// </summary>
    private float Direction(Vector2 pi, Vector2 pj, Vector2 pk)
    {
        return (pk.x - pi.x) * (pj.y - pi.y) - (pj.x - pi.x) * (pk.y - pi.y);
    }
}
