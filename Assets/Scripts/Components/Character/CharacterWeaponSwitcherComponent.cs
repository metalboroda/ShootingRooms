using Assets.Scripts.Components.Character;
using Components.Character;
using UnityEngine;

public class CharacterWeaponSwitcherComponent
{
    private readonly float _switchCooldown;
    private float _lastSwitchTime;

    private readonly CharacterWeaponControllerComponent _weaponController;

    public CharacterWeaponSwitcherComponent(CharacterWeaponControllerComponent weaponController, float switchCooldown = 0.2f)
    {
        _switchCooldown = switchCooldown;
        _lastSwitchTime = -switchCooldown;
        _weaponController = weaponController;
    }

    public void SwitchWeapon(int direction)
    {
        if (Time.time - _lastSwitchTime < _switchCooldown) return;

        int newIndex = _weaponController.CurrentWeaponIndex;

        if (direction > 0)
        {
            newIndex = (_weaponController.CurrentWeaponIndex + 1) % _weaponController.WeaponCount;
        }
        else if (direction < 0)
        {
            newIndex = (_weaponController.CurrentWeaponIndex - 1 + _weaponController.WeaponCount) % _weaponController.WeaponCount;
        }

        if (newIndex != _weaponController.CurrentWeaponIndex)
        {
            _weaponController.SpawnWeapon(newIndex);

            _lastSwitchTime = Time.time;
        }
    }
}