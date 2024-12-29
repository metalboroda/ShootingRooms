using System.Collections.Generic;
using Assets.Scripts.EventBus;
using Assets.Scripts.WeaponSystem;
using EventBus;
using UnityEngine;

namespace Components.Character
{
    public class CharacterWeaponControllerComponent
    {
        private readonly List<GameObject> _weaponPrefabs;
        private int _currentWeaponIndex = 0;

        private readonly Transform _weaponHolder;
        private readonly Transform _characterTransform;
        private WeaponBase _currentWeapon;

        public WeaponBase CurrentWeapon => _currentWeapon;
        public int WeaponCount => _weaponPrefabs.Count;
        public int CurrentWeaponIndex => _currentWeaponIndex;

        public CharacterWeaponControllerComponent(List<GameObject> weaponPrefabs, Transform weaponHolder, Transform characterTransform)
        {
            _weaponPrefabs = weaponPrefabs;
            _weaponHolder = weaponHolder;
            _characterTransform = characterTransform;

            SpawnWeapon(0);
        }

        public void SpawnWeapon(int index)
        {
            if (index < 0 || index >= _weaponPrefabs.Count) return;

            foreach (Transform child in _weaponHolder)
            {
                Object.Destroy(child.gameObject);
            }

            GameObject weaponInstance = Object.Instantiate(_weaponPrefabs[index], _weaponHolder);

            _currentWeapon = weaponInstance.GetComponent<WeaponBase>();
            _currentWeaponIndex = index;
            
            EventBus<Events.WeaponEquipped>.Raise(new Events.WeaponEquipped
            {
                CharacterID = _characterTransform.GetInstanceID(),
                Weapon = _currentWeapon,
            });
        }

        public void TryAttack(Vector3 targetPoint)
        {
            _currentWeapon.TryAttack(targetPoint);
        }
    }
}