using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace B1.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollView : MonoBehaviour
    {
        public ScrollRect m_Scroll => GetComponent<ScrollRect>();
        public RectTransform m_Rect => GetComponent<RectTransform>();
        public RectTransform m_Item = null;
        Action m_UpdateCallback = null;

        public async UniTask InitAsync(Action f_Callback)
        {

        }
        public async UniTask SetListDataAsync<T>(List<T> f_Data)
        {

        }
        public async UniTask UpdateListAsync()
        {

        }
        public async UniTask CloseAsync()
        {

        }
    }
}
