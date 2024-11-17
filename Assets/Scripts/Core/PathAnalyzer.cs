using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class PathAnalyzer
{
    #region Fields

    private const float HeightTolerance = 0.5f;
    private const float Epsilon = 1e-4f;

    #endregion


    #region Public Methods

    public bool PathIncludesNavMeshLink(NavMeshPath path)
    {
        foreach (var link in NavMeshLinkManager.Instance.NavMeshLinks)
        {
            if (PathIntersectsLink(path, link))
                return true;
        }

        return false;
    }

    #endregion

    #region Private Methods

        private bool PathIntersectsLink(NavMeshPath path, NavMeshLink link)
    {
        var linkStart = link.transform.TransformPoint(link.startPoint);
        var linkEnd = link.transform.TransformPoint(link.endPoint);

        for (int i = 1; i < path.corners.Length; i++)
        {
            var segmentStart = path.corners[i - 1];
            var segmentEnd = path.corners[i];

            if (Mathf.Abs(segmentStart.y - linkStart.y) > HeightTolerance &&
                Mathf.Abs(segmentEnd.y - linkEnd.y) > HeightTolerance)
                continue;

            if (LinesIntersect(segmentStart, segmentEnd, linkStart, linkEnd))
                return true;
        }

        return false;
    }

    private bool LinesIntersect(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
    {
        var p1 = new Vector2(a1.x, a1.z);
        var p2 = new Vector2(a2.x, a2.z);
        var p3 = new Vector2(b1.x, b1.z);
        var p4 = new Vector2(b2.x, b2.z);

        if (p1 == p2 || p3 == p4)
            return false;

        return LinesIntersect2D(p1, p2, p3, p4);
    }

    private bool LinesIntersect2D(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float d1 = Direction(p3, p4, p1);
        float d2 = Direction(p3, p4, p2);
        float d3 = Direction(p1, p2, p3);
        float d4 = Direction(p1, p2, p4);

        if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) && ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
            return true;

        if (Mathf.Abs(d1) < Epsilon && OnSegment(p3, p4, p1)) return true;
        if (Mathf.Abs(d2) < Epsilon && OnSegment(p3, p4, p2)) return true;
        if (Mathf.Abs(d3) < Epsilon && OnSegment(p1, p2, p3)) return true;
        if (Mathf.Abs(d4) < Epsilon && OnSegment(p1, p2, p4)) return true;

        return false;
    }

    private bool OnSegment(Vector2 p1, Vector2 p2, Vector2 p)
    {
        return Mathf.Min(p1.x, p2.x) - Epsilon <= p.x && p.x <= Mathf.Max(p1.x, p2.x) + Epsilon &&
               Mathf.Min(p1.y, p2.y) - Epsilon <= p.y && p.y <= Mathf.Max(p1.y, p2.y) + Epsilon;
    }

    private float Direction(Vector2 pi, Vector2 pj, Vector2 pk)
    {
        return (pk.x - pi.x) * (pj.y - pi.y) - (pj.x - pi.x) * (pk.y - pi.y);
    }


    #endregion

    
}