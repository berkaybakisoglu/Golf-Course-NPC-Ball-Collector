using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovement : MonoBehaviour
{
    #region Fields
    [Header("Area Settings")]
    [SerializeField] private int _sandAreaIndex;
    [SerializeField] private int _corruptedAreaIndex;
    [SerializeField] private NavMeshAgent _agent;
    private Vector3 _currentTargetPosition;
    private NPCController _npc;

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed = 3.5f;
    [SerializeField] private float _stoppingDistance = 0.3f;
    private float _searchRadius = 3.0f; 

    private NPCAnimator _animatorScript; // Reference to the Animator script
    [SerializeField] private float _rotationSpeed = 5f; // Rotation speed

    // Fields for handling OffMeshLink traversal
    private bool _isTraversingLink = false;
    private OffMeshLinkData _currentLinkData;

    #endregion

    #region Properties

    public NavMeshAgent Agent => _agent;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Initialize NavMeshAgent settings
        _agent.speed = _movementSpeed;
        _agent.stoppingDistance = _stoppingDistance;
        _agent.updateRotation = false; // Disable automatic rotation

        // Get the NPCAnimator component
        _animatorScript = GetComponent<NPCAnimator>();
        if (_animatorScript == null)
        {
            Debug.LogError("NPCAnimator component not found on the NPC.");
        }

        // Disable automatic OffMeshLink traversal to handle it manually
        _agent.autoTraverseOffMeshLink = false;
    }

    private void Update()
    {
        HandleRotation();
        HandleOffMeshLinkTraversal();
        HandleAreaBasedSpeed();
        UpdateAnimatorSpeed(); // Update the Animator's speed parameter
    }

    #endregion

    #region Public Methods

    public void Initialize(NPCController npcController)
    {
        _npc = npcController;
    }

    public void SetDestination(Vector3 position)
    {
        if (!_agent.isActiveAndEnabled)
        {
            Debug.LogWarning("NavMeshAgent is not active or enabled.");
            return;
        }

        NavMeshHit hit;
       
        if (NavMesh.SamplePosition(position, out hit, _searchRadius, NavMesh.AllAreas))
        {
            _currentTargetPosition = hit.position;
            _agent.SetDestination(_currentTargetPosition);
            if (_animatorScript != null)
            {
                _animatorScript.SetMoving(true);
            }
        }
        else
        {
            Debug.LogWarning("Target position is not reachable, and no valid point on the NavMesh was found nearby.");
        }
    }

    public void StopMovement(bool disableAgent = false)
    {
        if (_agent.isActiveAndEnabled)
        {
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;

            if (_animatorScript != null)
            {
                _animatorScript.SetIdle();
            }

            if (disableAgent)
            {
                _agent.enabled = false;
            }
        }
    }

    public bool HasReachedDestination()
    {
        if (!_agent.isActiveAndEnabled)
        {
            Debug.LogWarning("NavMeshAgent is not active or enabled.");
            return false;
        }

        return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
    }

    #endregion

    #region Private Methods
    
    private void HandleRotation()
    {
        Vector3 velocity = _agent.velocity;
        if (velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }
    }

    private void HandleOffMeshLinkTraversal()
    {
        if (_agent.isOnOffMeshLink)
        {
            if (!_isTraversingLink)
            {
                _isTraversingLink = true;
                _currentLinkData = _agent.currentOffMeshLinkData;

                // Trigger the jump animation
                if (_animatorScript != null)
                {
                    _animatorScript.SetJumping();
                }

                // Optionally, stop the agent's movement during the jump
                _agent.velocity = Vector3.zero;

                // Start the traversal coroutine
                StartCoroutine(TraverseOffMeshLink());
            }
        }
        else if (_isTraversingLink)
        {
            // Finished traversing the link
            _isTraversingLink = false;

            // Resume normal movement animations
            if (_animatorScript != null)
            {
                _animatorScript.SetMoving(true);
            }
        }
    }

    private IEnumerator TraverseOffMeshLink()
    {
        Vector3 startPos = _agent.transform.position;
        Vector3 endPos = _agent.currentOffMeshLinkData.endPos + Vector3.up * _agent.baseOffset;

        // Define the duration of the jump (should match your jump animation length)
        float jumpDuration = 2.167f; // Adjust based on your animation
        float elapsedTime = 0f;
        
        while (elapsedTime < jumpDuration)
        {
            // Calculate interpolation factor
            float t = elapsedTime / jumpDuration;

            // Create a simple arc for the jump
            float height = Mathf.Sin(t * Mathf.PI) * 1.0f; // Adjust height as needed
            transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

       
        transform.position = endPos;
        _agent.CompleteOffMeshLink();
        
        _isTraversingLink = false;

        if (_animatorScript != null)
        {
            _animatorScript.SetMoving(true);
        }
    }

    private void HandleAreaBasedSpeed()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            int area = NavMesh.GetAreaFromName("Sand");
            if (area != -1 && (hit.mask & (1 << area)) != 0 && area == _sandAreaIndex)
            {
                _agent.speed = _movementSpeed / 2f;
            }
            else
            {
                area = NavMesh.GetAreaFromName("Corrupted");
                if (area != -1 && (hit.mask & (1 << area)) != 0 && area == _corruptedAreaIndex)
                {
                    _agent.speed = _movementSpeed / 4f;
                }
                else
                {
                    _agent.speed = _movementSpeed;
                }
            }
        }
    }


    private void UpdateAnimatorSpeed()
    {
        if (_animatorScript != null && _agent != null)
        {
            float currentSpeed = _agent.velocity.magnitude;
            float normalizedSpeed = Mathf.Clamp01(currentSpeed / _movementSpeed);
            _animatorScript.SetSpeed(normalizedSpeed);
        }
    }

    #endregion
}
