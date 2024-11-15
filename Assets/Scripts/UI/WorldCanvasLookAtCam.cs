using UnityEngine;
public class WorldCanvasLookAtCam : MonoBehaviour
{
    #region Fields

    private Camera _mainCamera;
    private Transform _cachedTransform;
    private Vector3 _directionToCamera;

    // Frequency control for updates, otherwise this makes too many draw calls if too many objects have it
    [SerializeField] private bool _useReducedUpdateFrequency = false;
    [SerializeField] private float _updateInterval = 0.2f;
    private float _timeSinceLastUpdate = 0f;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        _cachedTransform = transform;
        _mainCamera = Camera.main;

        if (_mainCamera == null)
        {
            Debug.LogError("[WorldCanvasLookAtCam] Main camera not found.");
        }
    }

    private void LateUpdate()
    {
        if (_mainCamera == null) return;

        if (_useReducedUpdateFrequency)
        {
            _timeSinceLastUpdate += Time.deltaTime;
            if (_timeSinceLastUpdate < _updateInterval) return;
            _timeSinceLastUpdate = 0f;
        }

        UpdateBillboardRotation();
    }

    #endregion

    #region Private Methods

    private void UpdateBillboardRotation()
    {
        _directionToCamera = _mainCamera.transform.position - _cachedTransform.position;
        _directionToCamera.y = 0; 
        
        if (_directionToCamera != Vector3.zero)
        {
            _cachedTransform.rotation = Quaternion.LookRotation(_directionToCamera);
        }
    }

    #endregion
}