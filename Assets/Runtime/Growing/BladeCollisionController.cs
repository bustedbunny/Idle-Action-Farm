using System;
using UnityEngine;

namespace IdleActionFarm.Runtime.Growing
{
    public delegate void OnBladeCollision(Growable target);

    public class BladeCollisionController : MonoBehaviour
    {
        public event OnBladeCollision OnBladeCollision;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.body.TryGetComponent<Growable>(out var growable))
            {
                OnBladeCollision?.Invoke(growable);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Growable>(out var growable))
            {
                OnBladeCollision?.Invoke(growable);
            }
        }
    }
}