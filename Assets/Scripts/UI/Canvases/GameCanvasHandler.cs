using Assets.Scripts.EventBus;
using EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Canvases
{
    public class GameCanvasHandler : MonoBehaviour
    {
        [Header("Ammo")]
        [SerializeField] private Text clipCapacity;
        [SerializeField] private Text totalCapacity;

        private EventBinding<Events.WeaponAmmoEvent> _weaponAmmoEvent;

        private void OnEnable()
        {
            _weaponAmmoEvent = new EventBinding<Events.WeaponAmmoEvent>(OnWeaponAmmoEvent);
            EventBus<Events.WeaponAmmoEvent>.Register(_weaponAmmoEvent);
        }

        private void OnDisable()
        {
            EventBus<Events.WeaponAmmoEvent>.Unregister(_weaponAmmoEvent);
        }

        private void OnWeaponAmmoEvent(Events.WeaponAmmoEvent weaponAmmoEvent)
        {
            clipCapacity.text = $"{weaponAmmoEvent.CurrentAmmo}";
            totalCapacity.text = $"{weaponAmmoEvent.MaxAmmo}";
        }
    }
}