using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GolfCourse.NPC
{
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

        [SerializeField] private float _rotationSpeed = 5f;
        [SerializeField] private float _sandDebuff = 2f;
        [SerializeField] private float _corruptedDebuff = 4f;
        private bool _isTraversingLink = false;
        
        private Coroutine _currentTraversalCoroutine;
        private Coroutine _rotationCoroutine;

        #endregion

        #region Properties

        public NavMeshAgent Agent => _agent;

        #endregion

        #region Unity Methods

        private void Update()
        {
            HandleRotation();
            HandleOffMeshLinkTraversal();
            HandleAreaBasedSpeed();
            UpdateAnimatorSpeed();
        }

        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            if (_npc != null && _npc.Animator != null)
            {
                _npc.Animator.OnJumpAirEnd -= HandleJumpAirEnd;
                _npc.Animator.OnJumpLandEnd -= HandleJumpLandEnd;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(NPCController npcController)
        {
            _npc = npcController;
            if (_agent == null)
            {
                _agent = GetComponent<NavMeshAgent>();
            }
            _agent.speed = _movementSpeed;
            _agent.stoppingDistance = _stoppingDistance;
            _agent.updateRotation = false; 
            _agent.autoTraverseOffMeshLink = false;

            // Subscribe to animation events
            _npc.Animator.OnJumpAirEnd += HandleJumpAirEnd;
            _npc.Animator.OnJumpLandEnd += HandleJumpLandEnd;
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
                _npc.Animator.SetMoving(true);
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
                _npc.Animator.SetMoving(false);
                if (disableAgent)
                {
                    _agent.enabled = false;
                }
            }
        }

        #endregion

        #region Private Methods
        
        private void HandleRotation()
        {
            if (_isTraversingLink) return;
            Vector3 velocity = _agent.velocity;
            if (velocity.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            }
        }
        private IEnumerator RotateTowardsHorizontal(Vector3 direction, Action onComplete)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            while (Quaternion.Angle(transform.rotation, targetRotation) > 5)
            {
                Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime * 100f);
                transform.rotation = newRotation;
                yield return null;
            }
            
            transform.rotation = targetRotation;
            onComplete?.Invoke();
        }

        private void HandleOffMeshLinkTraversal()
        {
            if (_agent.isOnOffMeshLink && !_isTraversingLink)
            {
                // Calculate desired direction for the jump on the horizontal plane
                Vector3 startPos = _agent.transform.position;
                Vector3 endPos = _agent.currentOffMeshLinkData.endPos + Vector3.up * _agent.baseOffset;

                Vector3 directionToEnd = (endPos - startPos).normalized;
                directionToEnd.y = 0; // Ignore Y component for horizontal rotation
                if (directionToEnd == Vector3.zero)
                {
                    directionToEnd = transform.forward; // Default to current forward if direction is zero
                }

                // Start rotation coroutine if not already rotating
                if (_rotationCoroutine == null)
                {
                    _rotationCoroutine = StartCoroutine(RotateTowardsHorizontal(directionToEnd, () =>
                    {
                        _rotationCoroutine = null;
                        _isTraversingLink = true;
                        _agent.velocity = Vector3.zero;
                        _npc.Animator.SetJumping();
                    }));
                }
            }
            else if (!_agent.isOnOffMeshLink && _isTraversingLink)
            {
                _isTraversingLink = false;
                _npc.Animator.SetMoving(true);
            }
        }

        private void HandleJumpAirEnd()
        {
            if (_agent.isOnOffMeshLink)
            {
                if (_currentTraversalCoroutine != null)
                {
                    StopCoroutine(_currentTraversalCoroutine);
                }
                _currentTraversalCoroutine = StartCoroutine(TraverseOffMeshLink());
            }
        }

        private void HandleJumpLandEnd()
        {
            if (_isTraversingLink)
            {
                CompleteJump();
            }
        }

        private IEnumerator TraverseOffMeshLink()
        {
            Vector3 startPos = _agent.transform.position;
            Vector3 endPos = _agent.currentOffMeshLinkData.endPos + Vector3.up * _agent.baseOffset;

            float jumpDuration = 0.5f;
            float elapsedTime = 0f;

            while (elapsedTime < jumpDuration)
            {
                float t = elapsedTime / jumpDuration;
                float height = Mathf.Sin(t * Mathf.PI) * 1.0f;
                transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;

        
                Vector3 direction = (endPos - startPos).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;
        }


        private void CompleteJump()
        {
            if (_agent.isOnOffMeshLink)
            {
                Vector3 endPos = _agent.currentOffMeshLinkData.endPos + Vector3.up * _agent.baseOffset;
                transform.position = endPos;
                _agent.CompleteOffMeshLink();
            }

            _isTraversingLink = false;
            _npc.Animator.SetMoving(true);

            if (_currentTraversalCoroutine != null)
            {
                StopCoroutine(_currentTraversalCoroutine);
                _currentTraversalCoroutine = null;
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
                    _agent.speed = _movementSpeed / _sandDebuff;
                }
                else
                {
                    area = NavMesh.GetAreaFromName("Corrupted");
                    if (area != -1 && (hit.mask & (1 << area)) != 0 && area == _corruptedAreaIndex)
                    {
                        _agent.speed = _movementSpeed / _corruptedDebuff;
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
            float currentSpeed = _agent.velocity.magnitude;
            float normalizedSpeed = Mathf.Clamp01(currentSpeed / _movementSpeed);
            _npc.Animator.SetSpeed(normalizedSpeed);
        }

        #endregion
    }
}
