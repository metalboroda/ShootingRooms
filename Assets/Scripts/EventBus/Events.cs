using Assets.Scripts.Enums;
using Assets.Scripts.EventBus;
using Assets.Scripts.WeaponSystem;
using UnityEngine;

namespace EventBus
{
    public static class Events
    {
        #region Controls
        public struct MoveInput : IEvent
        {
            public Vector2 Axis;
        }

        public struct LookInput : IEvent
        {
            public Vector2 Axis;
        }

        public struct ShootPressed : IEvent { }
        public struct ShootThrowablePressed : IEvent { }

        public struct ScrollInput : IEvent
        {
            public Vector2 Axis;
        }

        public struct ReloadWeaponPressed : IEvent { }
        #endregion

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

        public struct CharacterHealthChanged : IEvent
        {
            public int ID;
            public int MaxHealth;
            public int CurrentHealth;
        }

        public struct CharacterInjured : IEvent
        {
            public int ID;
        }

        public struct CharacterDead : IEvent
        {
            public int ID;
            public CharacterType CharacterType;
        }
        #endregion

        #region Weapon System
        public struct WeaponEquipped : IEvent
        {
            public int CharacterID;
            public WeaponBase Weapon;
        }
        
        public struct WeaponUsed : IEvent
        {
            public int WeaponID;
        }

        public struct PlayerWeaponRecoiled : IEvent { }

        public struct WeaponAmmoEvent : IEvent
        {
            public int CurrentAmmo;
            public int MaxAmmo;
        }
        #endregion
    }
}