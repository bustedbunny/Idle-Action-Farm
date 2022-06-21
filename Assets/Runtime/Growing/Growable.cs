using System;
using Unity.Mathematics;
using UnityEngine;

namespace IdleActionFarm.Runtime.Growing
{
    public class Growable : MonoBehaviour
    {
        // Normalized
        private float _stage;
        [SerializeField] [Range(1f, 20f)] private float growingTime;

        private Vector3 _initialScale;
        private void Awake() { _initialScale = transform.localScale; }

        public void Cut()
        {
            _stage -= 0.5f;
            if (_stage <= 0f)
            {
                enabled = true;
                _stage = 0f;
            }
            else
            {
                enabled = false;
                UpdateTransform();
            }
        }

        private void Update()
        {
            if (_stage >= 1f) return;

            _stage += Time.deltaTime / growingTime;
            _stage = Mathf.Min(_stage, 1f);
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            transform.localScale = math.lerp(Vector3.zero, _initialScale, _stage * 0.5f + 0.5f);
        }
    }
}