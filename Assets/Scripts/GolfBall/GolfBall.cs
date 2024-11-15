using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(GolfBallData))]
public class GolfBall : MonoBehaviour
{
    #region Properties

    public GolfBallData Data => _data;

    #endregion

    #region Fields

    [SerializeField] private Renderer _golfBallRenderer;
    [SerializeField] private float _lineWidth = 0.05f;
    [SerializeField] private int _trajectoryResolution = 20;

    private GolfBallData _data;
    private LineRenderer _lineRenderer;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        if (_golfBallRenderer == null)
        {
            _golfBallRenderer = GetComponent<Renderer>();
        }
    }

    #endregion

    #region Public Methods

    public void InitializeGolfBall(GolfBallData data, Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        _data = data;
        _golfBallRenderer.material = data.Material;
    }

    public void Collect()
    {
        GolfBallManager.Instance.UnregisterGolfBall(this);
    }

    public void ThrowGolfBallInto(Vector3 startPos, Vector3 endPos, float duration, float height = 5f)
    {
        ConfigureLineRenderer();
        Vector3[] pathPoints = CalculateParabolicTrajectory(startPos, endPos, height, _trajectoryResolution);

        Sequence throwSequence = DOTween.Sequence();
        transform.position = startPos;

        throwSequence.Append(transform.DOPath(pathPoints, duration, PathType.CatmullRom)
            .SetOptions(false)
            .SetEase(Ease.Linear))
            .OnUpdate(UpdateLineRendererPath)
            .OnComplete(OnThrowComplete);
    }

    #endregion

    #region Private Methods

    private void ConfigureLineRenderer()
    {
        if (_lineRenderer == null)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        _lineRenderer.positionCount = 0;
        _lineRenderer.startWidth = _lineWidth;
        _lineRenderer.endWidth = _lineWidth;
        _lineRenderer.material = _data.Material;
        _lineRenderer.enabled = true;
    }

    private void UpdateLineRendererPath()
    {
        if (_lineRenderer == null)
            return;

        _lineRenderer.positionCount += 1;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, transform.position);
    }

    private void OnThrowComplete()
    {
        _lineRenderer.enabled = false;
        GolfBallManager.Instance.UnregisterGolfBall(this);
        GameManager.Instance.ScoreZone.CollectGolfBall(this);
    }

    private Vector3[] CalculateParabolicTrajectory(Vector3 start, Vector3 end, float height, int resolution)
    {
        Vector3[] points = new Vector3[resolution];
        float increment = 1f / (resolution - 1);

        for (int i = 0; i < resolution; i++)
        {
            float t = i * increment;
            float x = Mathf.Lerp(start.x, end.x, t);
            float z = Mathf.Lerp(start.z, end.z, t);
            float y = Mathf.Lerp(start.y, end.y, t) + height * 4f * t * (1f - t); // Peak at t=0.5
            points[i] = new Vector3(x, y, z);
        }

        return points;
    }

    #endregion
}
