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
        private void Awake() { _collider = GetComponent<Collider>(); }

        public void NotifyPicked() { _collider.enabled = false; }
        public bool IsFree => _collider.enabled;
    }
}