using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using IdleActionFarm.Runtime.Backpack;
using IdleActionFarm.Runtime.Pickups;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IdleActionFarm.Runtime.Barn
{
    public class Barn : MonoBehaviour
    {
        [SerializeField] private int sellInterval;
        private Dictionary<int, Backpack.Backpack> _backpacks;

        private void Awake() { _backpacks = new Dictionary<int, Backpack.Backpack>(); }

        private void OnTriggerEnter(Collider other)
        {
            var backPack = other.GetComponent<Backpack.Backpack>();
            var id = other.GetInstanceID();
            if (_backpacks.ContainsKey(id)) return;
            _backpacks.Add(other.GetInstanceID(), backPack);
            if (_backpacks.Count == 1)
            {
                ProcessBackpacks().Forget();
            }
        }


        private async UniTaskVoid ProcessBackpacks()
        {
            var ct = this.GetCancellationTokenOnDestroy();
            while (_backpacks.Count > 0)
            {
                foreach (var backpack in _backpacks.Values)
                {
                    var obj = backpack.Take();
                    if (obj == null) continue;
                    backpack.Wallet.AddMoney(obj.MoneyPrice, gameObject);

                    GrabPickupAsync(obj).Forget();
                }

                await UniTask.Delay(sellInterval, false, PlayerLoopTiming.Update, ct);
            }
        }

        private async UniTaskVoid GrabPickupAsync(GrowablePickup pickup)
        {
            pickup.transform.parent = null;
            await BackpackAnimations.FlyObject(pickup.gameObject, transform, 0.5f);
            Destroy(pickup.gameObject);
        }

        private void OnTriggerExit(Collider other) { _backpacks.Remove(other.GetInstanceID()); }
    }
}