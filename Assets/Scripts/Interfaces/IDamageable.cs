using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
  public interface IDamageable
  {
    void Damage(int damage, Vector3 hitPoint, DamageType damageType);
  }
}