using UnityEngine;

public sealed class GuideAnimationLoop
{
    private readonly Animator _animator;
    private readonly string _triggerName;
    private bool _isLooping;

    public GuideAnimationLoop(Animator animator, string triggerName)
    {
        _animator = animator;
        _triggerName = string.IsNullOrEmpty(triggerName) ? "Shining" : triggerName;
    }

    public bool IsLooping => _isLooping;

    public void SetLooping(bool looping)
    {
        if (_animator == null)
        {
            _isLooping = false;
            return;
        }

        _isLooping = looping;
        if (looping)
        {
            _animator.ResetTrigger(_triggerName);
            _animator.SetTrigger(_triggerName);
            return;
        }

        _animator.ResetTrigger(_triggerName);
        _animator.Play("New State", 0, 0f);
    }

    public void Tick()
    {
        if (!_isLooping || _animator == null)
        {
            return;
        }

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (_animator.IsInTransition(0))
        {
            return;
        }

        if (stateInfo.IsName("KeyboardShining") || stateInfo.IsName("SnackShining"))
        {
            return;
        }

        _animator.SetTrigger(_triggerName);
    }
}
