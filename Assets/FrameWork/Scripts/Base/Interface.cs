
using UnityEngine;

namespace B1
{
    interface ILog
    {
        public void Log<T>(T message) => Debug.Log($" 【 {GetType()} 】\n{message}");
        public void LogWarning<T>(T message) => Debug.LogError($" 【 {GetType()} 】\n{message}");
        public void LogError<T>(T message) => Debug.LogWarning($" 【 {GetType()} 】\n{message}");
    }
}