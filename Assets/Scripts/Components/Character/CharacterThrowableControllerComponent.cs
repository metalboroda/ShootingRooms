using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.WeaponSystem;

public class CharacterThrowableControllerComponent
{
    private readonly List<GameObject> _throwablePrefabs;
    private readonly int _currentThrowableWeaponIndex = 0;

    private readonly Transform _weaponHolder;

    public int ThrowableWeaponCount => _throwablePrefabs.Count;
    public int CurrentThrowableWeaponIndex => _currentThrowableWeaponIndex;

    public CharacterThrowableControllerComponent(List<GameObject> throwablePrefabs, Transform weaponHolder)
    {
        _throwablePrefabs = throwablePrefabs;
        _weaponHolder = weaponHolder;
    }

    public void TryThrow(Vector3 targetPoint)
    {
        if (_currentThrowableWeaponIndex < 0 || _currentThrowableWeaponIndex >= _throwablePrefabs.Count) return;

        GameObject weaponInstance = Object.Instantiate(_throwablePrefabs[_currentThrowableWeaponIndex], _weaponHolder.position, Quaternion.identity);
        WeaponBase throwableWeapon = weaponInstance.GetComponent<WeaponBase>();

        throwableWeapon.TryAttack(targetPoint);
    }
}