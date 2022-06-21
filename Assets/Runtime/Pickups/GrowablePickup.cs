using System;
using UnityEngine;

namespace IdleActionFarm.Runtime.Pickups
{
    // TODO Сделать интерфейсом/абстрактным классом.
    public class GrowablePickup : MonoBehaviour
    {
        private Collider _collider;
        private void Awake() { _collider = GetComponent<Collider>(); }

        public void NotifyPicked() { _collider.enabled = false; }

        public void NotifyDropped() { _collider.enabled = true; }
        public bool IsFree => _collider.enabled;
    }
}