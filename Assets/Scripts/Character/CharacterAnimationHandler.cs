using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class CharacterAnimationHandler : MonoBehaviour
    {
        [SerializeField] private float crossfadeDuration = 0.2f;

        [field: Header("")]
        [field: SerializeField] public CharacterAnimationDataSO CharacterAnimationData { get; private set; }

        private Coroutine _animationRoutine;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void CrossfadeAnimation(string animationName, Action onComplete = null)
        {
            _animator.CrossFadeInFixedTime(animationName, crossfadeDuration);

            if (onComplete != null)
            {
                if (_animationRoutine != null)
                {
                    StopCoroutine(_animationRoutine);
                }

                _animationRoutine = StartCoroutine(WaitForAnimationToEnd(animationName, onComplete));
            }
        }

        private IEnumerator WaitForAnimationToEnd(string animationName, Action onComplete)
        {
            yield return new WaitForEndOfFrame();

            var currentState = _animator.GetCurrentAnimatorStateInfo(0);

            while (currentState.IsName(animationName) == false)
            {
                currentState = _animator.GetCurrentAnimatorStateInfo(0);
                yield return null;
            }

            while (currentState.normalizedTime < 1.0f)
            {
                currentState = _animator.GetCurrentAnimatorStateInfo(0);
                yield return null;
            }

            onComplete?.Invoke();
        }

        public void StopAnimationRoutine()
        {
            if (_animationRoutine != null)
            {
                StopCoroutine(_animationRoutine);

                _animationRoutine = null; 
            }
        }
    }
}