using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace B1.UI
{
    public abstract class UIWindow : MonoBase
    {
        public virtual async UniTask InitAsync()
        {

        }
        public virtual async UniTask ShowAsync()
        {
            gameObject.SetActive(true);
        }
        public virtual async UniTask HideAsync()
        {
            gameObject.SetActive(false);
        }
        public virtual async UniTask CloseAsync()
        {

        }
    }
}
