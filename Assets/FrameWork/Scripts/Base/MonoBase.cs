using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace B1
{
    public class MonoBase : MonoBehaviour, Log
    {
        public void Log<T>(T message)
        {
            Debug.Log($" 【 {GetType()} 】\n{message}");
        }
        public void LogError<T>(T message)
        {
            Debug.LogError($" 【 {GetType()} 】\n{message}");
        }
        public void LogWarning<T>(T message)
        {
            Debug.LogWarning($" 【 {GetType()} 】\n{message}");
        }
    }
}
