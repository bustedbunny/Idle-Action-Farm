using System;
using System.Collections.Generic;
using IdleActionFarm.Runtime.Pickups;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IdleActionFarm.Runtime.Backpack
{
    public delegate void OnBackPackChange(int _activeSlotInd);

    public class Backpack : MonoBehaviour
    {
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private Vector3 scaleMultiplier = Vector3.one;

        private const float ScaleOffset = 0.1f;

        // Массив слотов
        private Transform[] _backpackSlots;

        // Коллайдер для триггера пикап механики (отключается при переполнении)

        // Стак для рюкзака
        private Stack<GrowablePickup> _backpackStack;
        private int _activeSlot;

        [SerializeField] private BackpackAnimations.Settings animationSettings = new BackpackAnimations.Settings
        {
            jumpTime = 1f,
            rotateTime = 2f,
            flyTime = 2f,
            jumpHeight = 20f,
            rotationSpeed = 360f
        };

        [SerializeField] private Wallet.Wallet wallet;
        public Wallet.Wallet Wallet => wallet;

        public event OnBackPackChange OnBackPackChange;

        private void Awake()
        {
            _backpackSlots = new Transform[40];
            _backpackStack = new Stack<GrowablePickup>(40);

            // Создадим контейнер для инвертаря
            var container = new GameObject("Backpack Container");
            container.transform.SetParent(transform);
            container.transform.localPosition = positionOffset;

            // Сгенерируем слоты-контейнеры для объектов инвертаря
            int i = 0;
            foreach (var posScale in GetPosScale())
            {
                var backPackSlot = new GameObject($"Slot {i}");
                backPackSlot.transform.SetParent(container.transform);
                backPackSlot.transform.localPosition = posScale.c0;
                backPackSlot.transform.localScale = posScale.c1;

                _backpackSlots[i] = backPackSlot.transform;

                i++;
            }
        }

        private void Start() { OnBackPackChange?.Invoke(_activeSlot); }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<GrowablePickup>(out var pickup))
            {
                // По идее проверка не нужна (т.к. коллайдер отключается), но на всякий случай оставлена
                if (pickup.IsFree)
                {
                    Put(pickup);
                }
            }
        }


        public void Put(GrowablePickup pickup)
        {
            if (_activeSlot >= 40)
            {
                return;
            }

            _backpackStack.Push(pickup);
            var slot = _backpackSlots[_activeSlot];
            _activeSlot++;
            pickup.NotifyPicked();
            OnBackPackChange?.Invoke(_activeSlot);


            BackpackAnimations.PutObjectAnimatedAsync(pickup.gameObject, slot, animationSettings);
        }


        public GrowablePickup Take()
        {
            if (_activeSlot <= 0) return null;
            var pickup = _backpackStack.Pop();
            _activeSlot--;
            OnBackPackChange?.Invoke(_activeSlot);
            return pickup;
        }


        // Возвращает 40 относительных позиций и масштабов для слотов рюкзака
        private IEnumerable<float3x2> GetPosScale()
        {
            var tran = transform;
            var lossyScale = tran.lossyScale;
            var size = lossyScale * ScaleOffset;
            size = Vector3.Scale(scaleMultiplier, size);
            for (int i = 0; i < 5; i++)
            {
                var pos = new Vector3(0f, i * size.y, 0f);
                for (int j = 0; j < 2; j++)
                {
                    pos.z = j * size.z;
                    for (int k = 0; k < 4; k++)
                    {
                        pos.x = (k - 1.5f) * size.x;
                        yield return new float3x2 { c0 = pos, c1 = size };
                    }
                }
            }
        }

        // Поскольку гизмо нельзя поворачивать - валидное оно будет только при identity ротации
        // в рантайме ротация учитывается как положено
        private void OnDrawGizmosSelected()
        {
            var tran = transform;
            float3 curPos = tran.position + Vector3.Scale(positionOffset, tran.lossyScale);
            foreach (var posScale in GetPosScale())
            {
                Gizmos.DrawWireCube(posScale.c0 + curPos, posScale.c1);
            }
        }
    }
}