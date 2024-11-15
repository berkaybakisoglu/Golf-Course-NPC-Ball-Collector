// NPCs/NPCAnimation.cs

using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCAnimator : MonoBehaviour
{
    private Animator _animator;

    // Parameter hashes for performance
    private int _isMovingHash;
    private int _isCollectingHash;
    private int _isScoringHash;
    private int _isDeadHash;
    public event Action OnCollectAnimationEnd;
    public event Action OnCollectAnimationCollect;
    public event Action OnScoreAnimationEnd;
    public event Action OnScoringAnimationThrowEnd;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        // Initialize parameter hashes
        _isMovingHash = Animator.StringToHash("isMoving");
        _isCollectingHash = Animator.StringToHash("isCollecting");
        _isScoringHash = Animator.StringToHash("isScoring");
        _isDeadHash = Animator.StringToHash("isDead");
    }

    public void SetIdle()
    {
        _animator.SetBool(_isMovingHash, false);
        _animator.SetBool(_isScoringHash, false);
        _animator.SetBool(_isCollectingHash, false);

    }

    /// <summary>
    /// Sets the moving state of the NPC.
    /// </summary>
    /// <param name="isMoving">Whether the NPC is moving.</param>
    public void SetMoving(bool isMoving)
    {
        _animator.SetBool(_isMovingHash, isMoving);
    }

    /// <summary>
    /// Triggers the attack animation.
    /// </summary>
    public void SetCollecting(bool isCollecting)
    {
        _animator.SetBool(_isCollectingHash,isCollecting);
    }
    
    public void SetDead()
    {
        _animator.SetTrigger(_isDeadHash);
    }
    
    /// <summary>
    /// Method called by Animation Event at the end of collect animation.
    /// </summary>
    public void OnCollectAnimationEnded()
    {
        OnCollectAnimationEnd?.Invoke();
    }

    public void OnCollectAnimationCollectEnded()
    {
        OnCollectAnimationCollect?.Invoke();
    }
    
    public void OnScoreAnimationEnded()
    {
        OnScoreAnimationEnd?.Invoke();
    }

    public void OnScoringAnimationThrowEnded()
    {
        OnScoringAnimationThrowEnd?.Invoke();
    }
    

    /// <summary>
    /// Optional: Method to handle other animation triggers.
    /// </summary>
    /// <param name="triggerName">Name of the trigger parameter.</param>
    public void TriggerAnimation(string triggerName)
    {
        _animator.SetTrigger(triggerName);
    }

    public void SetScoring(bool isScoring)
    {
        _animator.SetBool(_isScoringHash, isScoring);
    }
}