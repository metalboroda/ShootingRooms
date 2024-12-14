using Assets.Scripts.Enums;
using Assets.Scripts.EventBus;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Character
{
  public class CharacterHitboxHandler : MonoBehaviour, IDamageable
  {
    [SerializeField] private HitboxType hitboxType;
    [SerializeField] private float minDamageModifier = 0.1f;
    [SerializeField] private float maxDamageModifier = 1f;

    private int _instanceID;
    private float _randomDamageModifier;

    private void Awake()
    {
      _instanceID = transform.root.GetInstanceID();
    }

    private void Start()
    {
      _randomDamageModifier = Random.Range(minDamageModifier, maxDamageModifier);
    }

    public void Damage(int damage)
    {
      int modifiedDamage = Mathf.CeilToInt(damage * _randomDamageModifier);

      EventBus<Events.CharacterDamaged>.Raise(new Events.CharacterDamaged
      {
        ID = _instanceID,
        Damage = modifiedDamage,
      });
    }
  }
}