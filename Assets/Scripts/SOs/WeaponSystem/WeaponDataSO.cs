using UnityEngine;

namespace Assets.Scripts.SOs.WeaponSystem
{
  [CreateAssetMenu(fileName = "Weapon Data", menuName = "SOs/Weapon System/Weapon Data")]
  public class WeaponDataSO : ScriptableObject
  {
    public string WeaponName;
    public int Damage;
    public int PrpojectileSpeed;
    public float AttackRate;
    public float Spread;
    public int ProjectilesPerShot;
    public float Range;
  }
}