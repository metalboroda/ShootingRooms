using Assets.Scripts.EventBus;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class CharacterHandler : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 100;

        [Header("Injury Settings")]
        [SerializeField] private float injuryDelay = 0.5f;
        [SerializeField] private int injuryHealthThreshold = 30;
        [SerializeField] private float injuryDeathDelay = 10f;

        private int _currentHealth;

        private Coroutine _injuryRoutine;
        private Coroutine _injuryDeathRoutine;

        private EventBinding<Events.CharacterDamaged> _characterDamagedBinding;

        private void OnEnable()
        {
            _characterDamagedBinding = new EventBinding<Events.CharacterDamaged>(OnCharacterDamaged);
            EventBus<Events.CharacterDamaged>.Register(_characterDamagedBinding);
        }

        private void OnDisable()
        {
            EventBus<Events.CharacterDamaged>.Unregister(_characterDamagedBinding);
        }

        private void Start()
        {
            _currentHealth = maxHealth;

            RaiseHealthChangedEvent();
        }

        private void OnCharacterDamaged(Events.CharacterDamaged characterDamaged)
        {
            if (characterDamaged.ID != transform.GetInstanceID()) return;

            _currentHealth = Mathf.Max(_currentHealth - characterDamaged.Damage, 0);

            HandleInjury();

            if (_currentHealth <= 0)
            {
                HandleDeath();
            }

            RaiseHealthChangedEvent();
        }

        private void HandleInjury()
        {
            if (_currentHealth > 0 && _currentHealth <= injuryHealthThreshold && _injuryRoutine == null)
            {
                _injuryRoutine = StartCoroutine(DoInjuryRoutine());
            }
        }

        private void HandleDeath()
        {
            StopRoutine(ref _injuryRoutine);

            EventBus<Events.CharacterDead>.Raise(new Events.CharacterDead
            {
                ID = transform.GetInstanceID()
            });
        }

        private IEnumerator DoInjuryRoutine()
        {
            yield return new WaitForSeconds(injuryDelay);

            EventBus<Events.CharacterInjured>.Raise(new Events.CharacterInjured
            {
                ID = transform.GetInstanceID()
            });
        }

        public void InjuryDeath()
        {
            _injuryDeathRoutine ??= StartCoroutine(DoInjuryDeath());
        }

        private IEnumerator DoInjuryDeath()
        {
            yield return new WaitForSeconds(injuryDeathDelay);

            HandleDeath();

            _injuryDeathRoutine = null;
        }

        private void RaiseHealthChangedEvent()
        {
            EventBus<Events.CharacterHealthChanged>.Raise(new Events.CharacterHealthChanged
            {
                ID = transform.GetInstanceID(),
                MaxHealth = maxHealth,
                CurrentHealth = _currentHealth
            });
        }

        public void StopAllRoutines()
        {
            StopRoutine(ref _injuryRoutine);
            StopRoutine(ref _injuryDeathRoutine);
        }

        private void StopRoutine(ref Coroutine routine)
        {
            if (routine != null)
            {
                StopCoroutine(routine);

                routine = null;
            }
        }
    }
}