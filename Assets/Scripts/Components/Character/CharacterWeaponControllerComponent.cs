using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.WeaponSystem;

namespace Assets.Scripts.Components.Character
{
    public class CharacterWeaponControllerComponent
    {
        private readonly List<GameObject> _weaponPrefabs;
        private int _currentWeaponIndex = 0;

        private readonly Transform _weaponHolder;
        private WeaponBase _currentWeapon;

        public WeaponBase CurrentWeapon => _currentWeapon;
        public int WeaponCount => _weaponPrefabs.Count;
        public int CurrentWeaponIndex => _currentWeaponIndex;

        public CharacterWeaponControllerComponent(List<GameObject> weaponPrefabs, Transform weaponHolder)
        {
            _weaponPrefabs = weaponPrefabs;
            _weaponHolder = weaponHolder;

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
        }

        public void TryAttack(Vector3 targetPoint)
        {
            _currentWeapon.TryAttack(targetPoint);
        }
    }
}