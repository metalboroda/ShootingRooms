using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.EventBus
{
  public static class Events
  {
    #region Character
    public struct PlayerInitialized : IEvent
    {
      public int ID;
      public Transform Transform;
      public Transform CameraTarget;
    }

    public struct CharacterDamaged : IEvent
    {
      public int ID;
      public int Damage;
    }

    public struct CharacterDead : IEvent
    {
      public int ID;
      public CharacterType CharacterType;
    }
    #endregion

    #region Weapon System
    public struct WeaponUsed : IEvent
    {
      public int ID;
    }
    #endregion
  }
}