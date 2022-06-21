using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleActionFarm.Runtime.Growing
{
    public class Blade : MonoBehaviour
    {
        private void Awake()
        {
            foreach (var blade in GetComponentsInChildren<BladeCollisionController>())
            {
                blade.OnBladeCollision += OnCollision;
            }

            _cutGrowables = new HashSet<Growable>();
        }

        private HashSet<Growable> _cutGrowables;

        public void HarvestingBegan() { _cutGrowables.Clear(); }

        private void OnCollision(Growable target)
        {
            if (!enabled) return;

            if (_cutGrowables.Add(target))
            {
                target.Cut();
            }
        }
    }
}