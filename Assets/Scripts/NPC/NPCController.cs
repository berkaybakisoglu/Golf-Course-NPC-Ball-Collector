
using GolfCourse.Manager;
using GolfCourse.NPC;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(NPCTargeting))]
[RequireComponent(typeof(NPCAnimator))]
public class NPCController : MonoBehaviour
{
    public NPCMovement Movement => _movement;
    public NPCTargeting Targeting => _targeting;
    public NPCAnimator Animator => _animator;
    public HealthController HealthController => _healthController;
    
    private INPCState _currentState;
    [SerializeField] private NPCMovement _movement;
    [SerializeField] private NPCTargeting _targeting;
    [SerializeField] private NPCAnimator _animator;
    [SerializeField] private HealthController _healthController;
    [SerializeField] private Transform _handTransform;
    public Transform HandTransform => _handTransform;
    public INPCState CurrentState => _currentState;

    public void Initialize()
    {
        _healthController.Initialize();
        _targeting.Initialize(this,GameManager.Instance.ScoreZone.transform);
        _movement.Initialize(this);
        _healthController.OnHealthDepleted += HandleHealthDepleted;
        TransitionToState(new SearchingState());
    }

    private void Update()
    {
        _currentState?.UpdateState(this);
    }

    public void TransitionToState(INPCState newState)
    {
        if (newState == null)
        {
            Debug.LogError("[NPCController] Cannot transition to a null state.");
            return;
        }

        _currentState?.ExitState(this);
        _currentState = newState;
        _currentState.EnterState(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        _currentState?.OnTriggerEnter(this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        _currentState?.OnTriggerExit(this, other);
    }
    // Expose properties for states to access necessary components

    
    #region Health Event Handlers
    
    /// <summary>
    /// Handles actions when health is depleted.
    /// </summary>
    private void HandleHealthDepleted()
    {
        // Transition to incapacitated state
        TransitionToState(new DeadState());
    }
    #endregion
}