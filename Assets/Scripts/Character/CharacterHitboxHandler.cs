﻿using Assets.Scripts.Enums;
using Assets.Scripts.EventBus;
using Assets.Scripts.Interfaces;
using UltimatePooling;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class CharacterHitboxHandler : MonoBehaviour, IDamageable
    {
        [SerializeField] private HitboxType hitboxType;
        [Space]
        [SerializeField] private float minDamageModifier = 0.1f;
        [SerializeField] private float maxDamageModifier = 1f;

        [Header("")]
        [SerializeField] private GameObject parentObject;

        [Header("VFX")]
        [SerializeField] private GameObject bulletDamagePrefab;
        [SerializeField] private GameObject woundPrefab;

        private int _instanceID;
        private float _randomDamageModifier;

        private void Awake()
        {
            _instanceID = parentObject.transform.GetInstanceID();
        }

        private void Start()
        {
            _randomDamageModifier = Random.Range(minDamageModifier, maxDamageModifier);
        }

        public void Damage(int damage, Vector3 hitPoint, DamageType damageType)
        {
            int modifiedDamage = Mathf.CeilToInt(damage * _randomDamageModifier);

            EventBus<Events.CharacterDamaged>.Raise(new Events.CharacterDamaged
            {
                ID = _instanceID,
                Damage = modifiedDamage,
            });

            if (bulletDamagePrefab != null && damageType == DamageType.Bullet)
            {
                UltimatePool.spawn(bulletDamagePrefab, hitPoint, Quaternion.identity);
            }

            if (woundPrefab != null)
            {
                GameObject wound = UltimatePool.spawn(woundPrefab, hitPoint, Quaternion.identity);

                wound.transform.SetParent(transform);
            }
        }
    }
}