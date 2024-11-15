using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovement : MonoBehaviour
{
    #region Fields

    [SerializeField] private NavMeshAgent _agent;
    private Vector3 _currentTargetPosition;
    private NPCController _npc;
    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed = 3.5f;
    [SerializeField] private float _stoppingDistance = 0.3f;
    private float _searchRadius =  3.0f; 

    private NPCAnimator _animatorScript; // Reference to the Animator script
    [SerializeField] private float _rotationSpeed = 5f; // Rotation speed

    #endregion

    #region Properties

    public NavMeshAgent Agent => _agent;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        _agent.speed = _movementSpeed;
        _agent.stoppingDistance = _stoppingDistance;
        _agent.updateRotation = false; // Disable automatic rotation

        // Get the NPCAnimator component
        _animatorScript = GetComponent<NPCAnimator>();
        if (_animatorScript == null)
        {
            Debug.LogError("NPCAnimator component not found on the NPC.");
        }
    }

    private void Update()
    {
        HandleRotation();
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

    #endregion
}
