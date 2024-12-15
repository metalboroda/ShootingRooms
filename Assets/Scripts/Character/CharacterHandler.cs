using Assets.Scripts.Character.Enemy;
using Assets.Scripts.Enums;
using Assets.Scripts.EventBus;
using UnityEngine;

namespace Assets.Scripts.Character
{
  public class CharacterHandler : MonoBehaviour
  {
    [SerializeField] private int maxHealth = 100;

    private int _currentHealth;

    private EnemyController _enemyController;

    private EventBinding<Events.CharacterDamaged> _characterDamaged;

    private void Awake()
    {
      _enemyController = GetComponent<EnemyController>();
    }

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
    }

    private void OnCharacterDamaged(Events.CharacterDamaged characterDamaged)
    {
      if (characterDamaged.ID != transform.GetInstanceID())
      {
        return;
      }

      _currentHealth -= characterDamaged.Damage;

      if (_currentHealth <= 0)
      {
        _currentHealth = 0;

        EventBus<Events.CharacterDead>.Raise(new Events.CharacterDead
        {
          ID = transform.GetInstanceID(),
        });
      }
    }
  }
}