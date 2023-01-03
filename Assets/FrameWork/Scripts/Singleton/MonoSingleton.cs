using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace B1
{
    public abstract class MonoSingleton<T> : MonoBase
        where T : MonoSingleton<T>
    {
        public static T Instance = null;
        public virtual void Awake()
        {
            Instance = Instance == null ? (T)this : Instance;
        }
    }
}