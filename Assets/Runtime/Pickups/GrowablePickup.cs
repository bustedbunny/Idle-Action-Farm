using System;
using UnityEngine;

namespace IdleActionFarm.Runtime.Pickups
{
    // TODO Сделать интерфейсом/абстрактным классом.
    public class GrowablePickup : MonoBehaviour
    {
        [SerializeField] private uint moneyPrice;
        public uint MoneyPrice => moneyPrice;
        private Collider _collider;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void NotifyPicked()
        {
            _collider.enabled = false;
            _rigidbody.isKinematic = true;
        }

        public bool IsFree => _collider.enabled;
    }
}