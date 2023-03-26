using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace B1
{
    public class MonoBase : MonoBehaviour, ILog
    {
        protected void Log<T>(T message)
        {
            Debug.Log($" 【 {GetType()} 】\n{message}");
        }
        protected void LogError<T>(T message)
        {
            Debug.LogError($" 【 {GetType()} 】\n{message}");
        }
        protected void LogWarning<T>(T message)
        {
            Debug.LogWarning($" 【 {GetType()} 】\n{message}");
        }
        protected async UniTask DelayAsync(int f_DeltaTime = 0)
        {
            await UniTask.Delay(f_DeltaTime);
        }
        protected virtual void Awake() { }
        protected virtual void OnDestroy() { }
    }
}
