using System;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace IdleActionFarm.Runtime.Backpack
{
    public static class BackpackAnimations
    {
        [Serializable]
        public struct Settings
        {
            public float jumpTime;
            public float rotateTime;
            public float flyTime;
            public float jumpHeight;
            public float rotationSpeed;
        }

        public static async UniTaskVoid PutObjectAnimatedAsync(GameObject obj, Transform slot, Settings settings)
        {
            // Поднимает объект в воздух
            var tran = obj.transform;
            var time = settings.rotateTime;
            while (time >= 0f)
            {
                time -= Time.deltaTime;
                tran.Translate(new Vector3(0f, settings.jumpHeight * Time.deltaTime));
                tran.Rotate(Vector3.up, settings.rotationSpeed * Time.deltaTime);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            // Повертим объект немного в воздухе
            time = settings.jumpTime;
            while (time >= 0f)
            {
                time -= Time.deltaTime;
                tran.Rotate(Vector3.up, 20f * Time.deltaTime);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            await FlyObject(obj, slot, settings.flyTime);


            tran.SetParent(slot);
            tran.localRotation = Quaternion.identity;
            tran.localPosition = Vector3.zero;
        }

        // Анимация полёта объекта до выбранного трансформа
        public static async UniTask FlyObject(GameObject obj, Transform to, float flyTime)
        {
            var tran = obj.transform;
            var initPos = tran.position;
            var time = flyTime;
            while (time >= 0f)
            {
                time -= Time.deltaTime;
                var curPos = math.lerp(to.position, initPos, time / flyTime);
                tran.position = curPos;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
    }
}