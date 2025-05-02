using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
public class IdleAnimBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private float _minTimeBored;
    [SerializeField]
    private float _maxTimeBored;
    private float _timeUntilBored;

    [SerializeField]
    private int _numberOfBoredAnimations;

    private bool _isBored;
    private float _idleTime;
    private int _boredAnimation;
    private bool _isFirstAnimation;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timeUntilBored = Random.Range(_minTimeBored, _maxTimeBored);
        _isBored = false;
        _idleTime = 0;

        _isFirstAnimation = true;
        _boredAnimation = 0;
        animator.SetFloat("BoredAnimation", _boredAnimation);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_isBored)
        {
            _idleTime += Time.deltaTime;

            if (_idleTime > _timeUntilBored && IsAnimationAtCycleStart(stateInfo))
            {
                _isBored = true;

                if (_isFirstAnimation)
                {
                    _isFirstAnimation = false;
                    _boredAnimation = 0;
                }
                else
                {
                    _boredAnimation = Random.Range(1, _numberOfBoredAnimations);
                }

                animator.SetFloat("BoredAnimation", _boredAnimation);
            }
        }
        else if (IsAnimationAtCycleEnd(stateInfo))
        {
            ResetIdle();
        }

        animator.SetFloat("BoredAnimation", _boredAnimation, 0.4f, Time.deltaTime);
    }

    private void ResetIdle()
    {
        _isBored = false;
        _idleTime = 0;
    }

    private bool IsAnimationAtCycleStart(AnimatorStateInfo stateInfo)
    {
        return stateInfo.normalizedTime % 1 < 0.04f;
    }

    private bool IsAnimationAtCycleEnd(AnimatorStateInfo stateInfo)
    {
        return stateInfo.normalizedTime % 1 > 0.90f;
    }
}