using Assets.Scripts.EventBus;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class CharacterHandler : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;

        [Header("Injury Settings")]
        [SerializeField] private float injuryDelay = 2f;
        [SerializeField] private int injuryHealthThrashold = 30;
        [SerializeField] private float injuryDeathDelay = 7.5f;

        private int _currentHealth;

        private Coroutine _injuryRoutine;
        private Coroutine _injuryDeathRoutine;

        private EventBinding<Events.CharacterDamaged> _characterDamaged;

        private void OnEnable()
        {
            _characterDamaged = new EventBinding<Events.CharacterDamaged>(OnCharacterDamaged);
            EventBus<Events.CharacterDamaged>.Register(_characterDamaged);
        }

        private void OnDisable()
        {
            EventBus<Events.CharacterDamaged>.Unregister(_characterDamaged);
        }

        private void Start()
        {
            _currentHealth = maxHealth;

            SendCharacterHealthChangedEvent();
        }

        private void OnCharacterDamaged(Events.CharacterDamaged characterDamaged)
        {
            if (characterDamaged.ID != transform.GetInstanceID()) return;

            _currentHealth -= characterDamaged.Damage;

            if (_currentHealth <= injuryHealthThrashold)
            {
                _injuryRoutine = StartCoroutine(DoInjury());
            }

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;

                if (_injuryRoutine != null)
                {
                    StopCoroutine(_injuryRoutine);

                    _injuryRoutine = null;
                }

                EventBus<Events.CharacterDead>.Raise(new Events.CharacterDead
                {
                    ID = transform.GetInstanceID(),
                });
            }

            SendCharacterHealthChangedEvent();
        }

        private IEnumerator DoInjury()
        {
            yield return new WaitForSeconds(injuryDelay);

            EventBus<Events.CharacterInjured>.Raise(new Events.CharacterInjured
            {
                ID = transform.GetInstanceID(),
            });
        }

        public void InjuryDeath()
        {
            _injuryDeathRoutine = StartCoroutine(DoInjuryDeath());
        }

        private IEnumerator DoInjuryDeath()
        {
            yield return new WaitForSeconds(injuryDeathDelay);

            EventBus<Events.CharacterDead>.Raise(new Events.CharacterDead
            {
                ID = transform.GetInstanceID(),
            });

            if (_injuryRoutine != null)
            {
                StopCoroutine(_injuryRoutine);

                _injuryRoutine = null;
            }

            _injuryDeathRoutine = null;
        }

        private void SendCharacterHealthChangedEvent()
        {
            EventBus<Events.CharacterHealthChanged>.Raise(new Events.CharacterHealthChanged
            {
                ID = transform.GetInstanceID(),
                MaxHealth = maxHealth,
                CurrentHealth = _currentHealth,
            });
        }
    }
}