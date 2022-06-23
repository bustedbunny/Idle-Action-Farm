using System;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IdleActionFarm.Runtime.Growing
{
    public delegate void GrowableHarvested(Growable growable, bool isHarvested);

    public class Growable : MonoBehaviour
    {
        // Normalized
        private float _stage;
        [SerializeField] [Range(1f, 20f)] private float growingTime;
        [SerializeField] private GameObject pickupPrefab;

        public event GrowableHarvested OnCut;
        private Vector3 _initialScale;

        private void Awake()
        {
            _initialScale = transform.localScale;
            _stage = 1f;
        }

        public void Cut()
        {
            if (!enabled) return;

            _stage -= 0.5f;
            if (_stage <= 0f)
            {
                _stage = 0f;
                enabled = false;
            }

            var pickup = Instantiate(pickupPrefab);
            var pos = transform.position;
            pos.y += transform.lossyScale.y * 2f;
            pickup.transform.position = pos;


            UpdateTransform();
            OnCut?.Invoke(this, _stage == 0f);
        }

        public async UniTask GrowAsync()
        {
            var ct = this.GetCancellationTokenOnDestroy();
            while (true)
            {
                _stage += Time.deltaTime / growingTime;
                if (_stage >= 1f)
                {
                    _stage = 1f;
                    UpdateTransform();
                    break;
                }

                UpdateTransform();
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }

        private void UpdateTransform()
        {
            var t = _stage * 0.9f + 0.1f;
            transform.localScale = math.lerp(Vector3.zero, _initialScale, t);
        }
    }
}