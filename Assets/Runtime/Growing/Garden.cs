using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IdleActionFarm.Runtime.Growing
{
    public class Garden : MonoBehaviour
    {
        [SerializeField] private GameObject growablePrefab;
        [SerializeField] private int numOfGrowables;
        [SerializeField] private Transform container;


        private HashSet<Growable> _growables;

        private bool _shouldGrow;

        private void Awake()
        {
            _growables = new HashSet<Growable>();
            var curPosition = container.position;
            var curScale = container.localScale;

            var sqrt = Mathf.Sqrt(numOfGrowables);
            if (sqrt - (int)sqrt > 0)
            {
                throw new Exception("Must be square rootable");
            }

            for (int i = 0; i < sqrt; i++)
            {
                var pos = new Vector3(0f, 1f, 0f);
                pos.x = i / sqrt - 1 / 3f;
                for (int j = 0; j < sqrt; j++)
                {
                    pos.z = j / sqrt - 1 / 3f;
                    var newGrowable = Instantiate(growablePrefab,
                        curPosition + Vector3.Scale(curScale, pos),
                        Quaternion.identity,
                        transform);
                    var growable = newGrowable.GetComponentInChildren<Growable>();
                    _growables.Add(growable);
                    growable.OnCut += OnGrowableCut;
                }
            }
        }

        private int _curCount;

        private void OnGrowableCut(Growable growable, bool isHarvested)
        {
            if (!isHarvested) return;
            _curCount++;
            if (_curCount < numOfGrowables) return;

            RegrowAll().Forget();

            _curCount = 0;
        }

        private async UniTask RegrowAll()
        {
            var tasks = new List<UniTask>(9);
            tasks.AddRange(Enumerable.Select(_growables, growable => growable.GrowAsync()));

            await UniTask.WhenAll(tasks);

            foreach (var growable in _growables)
            {
                growable.enabled = true;
            }
        }
    }
}