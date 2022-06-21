using System;
using UnityEngine;

namespace IdleActionFarm.Runtime.Growing
{
    public delegate void GrowableEvent(Growable target);

    public class BladeCollisionController : MonoBehaviour
    {
        public event GrowableEvent OnBladeCollision;

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