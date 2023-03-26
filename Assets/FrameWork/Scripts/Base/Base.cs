using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace B1
{
    public class Base : ILog
    {
        public void Log<T>(T message) => Debug.Log($" 【 {GetType()} 】\n{message}");
        public void LogWarning<T>(T message) => Debug.LogWarning($" 【 {GetType()} 】\n{message}");
        public void LogError<T>(T message) => Debug.LogError($" 【 {GetType()} 】\n{message}");
        public async UniTask DelayAsync(int f_DelatTime = 0)
        {
            await UniTask.Delay(f_DelatTime);
        }
    }
}
