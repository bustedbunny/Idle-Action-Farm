using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using IdleActionFarm.Runtime.Wallet;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace IdleActionFarm.Runtime.UI
{
    public class WalletUI : MonoBehaviour
    {
        [SerializeField] private Wallet.Wallet wallet;

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private RectTransform targetElement;

        [SerializeField] private Camera playerCamera;

        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private float coinFlyTime = 0.5f;

        private void Awake()
        {
            wallet.OnWalletChange += OnWalletChange;
            shakeSettings.position = transform.position;
        }

        private void OnDestroy() { wallet.OnWalletChange -= OnWalletChange; }

        private uint _walletInternal;

        private void OnWalletChange(int difference, GameObject from)
        {
            if (from == null)
            {
                _walletInternal.AddIntToLong(difference);
                text.text = difference.ToString();
            }
            else
            {
                WalletAnimation(from.transform, difference).Forget();
            }
        }


        [SerializeField] private Animations.ShakeSettings shakeSettings = new Animations.ShakeSettings
        {
            time = 0.25f, power = 5f
        };

        private async UniTaskVoid WalletAnimation(Transform from, int difference)
        {
            await AddToWalletAnimationAsync(from, targetElement);
            AddCoins(difference).Forget();
            await transform.ShakeElement(shakeSettings);
            transform.position = shakeSettings.position;
        }

        [SerializeField] private float coinCountTime = 2f;

        private async UniTask AddCoins(int value)
        {
            var div = TimeSpan.FromMilliseconds(coinCountTime / value);

            for (int i = 0; i < value; i++)
            {
                _walletInternal++;
                text.text = _walletInternal.ToString();
                await UniTask.Delay(div);
            }
        }

        private async UniTask AddToWalletAnimationAsync(Transform from, Transform to)
        {
            var ct = CancellationTokenSource.CreateLinkedTokenSource(from.GetCancellationTokenOnDestroy(),
                    to.GetCancellationTokenOnDestroy())
                .Token;

            Vector3 initPosition = RectTransformUtility.WorldToScreenPoint(playerCamera, from.position);

            var coin = Instantiate(coinPrefab, text.canvas.transform);
            var tran = coin.transform;
            tran.position = initPosition;

            var endScale = tran.localScale;

            var time = coinFlyTime;
            while (time >= 0f)
            {
                time -= Time.deltaTime;

                tran.position = math.lerp(to.position, initPosition, time / coinFlyTime);
                tran.localScale = math.lerp(endScale, Vector3.zero, time / coinFlyTime);

                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            Destroy(coin);
        }
    }

    public static class Animations
    {
        [Serializable]
        public struct ShakeSettings
        {
            public float time;
            public float power;
            [NonSerialized] public Vector3 position;
        }

        public static async UniTask ShakeElement(this Transform element, ShakeSettings settings)
        {
            var ct = element.GetCancellationTokenOnDestroy();
            var time = settings.time;
            while (time >= 0f)
            {
                time -= Time.deltaTime;
                var timeForSin = new float2(time, time * 1.333f) * 50f;
                var sin = math.sin(timeForSin);
                element.position = settings.position + (Vector3)(Vector2)sin * settings.power;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }
    }
}