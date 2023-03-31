using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace B1.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollView : MonoBase
    {
        public ScrollRect Scroll => GetComponent<ScrollRect>();
        public RectTransform Rect => GetComponent<RectTransform>();


        public ScrollViewListItem m_Item = null;

        private Action<int, ScrollViewListItem> m_ItemCallback = null;

        private List<ScrollViewListItem> m_ItemList = new();

        protected override void Awake()
        {
            base.Awake();
            m_Item?.gameObject.SetActive(false);
        }
        public async UniTask InitData(Action<int, ScrollViewListItem> f_ItemCallback)
        {
            m_ItemCallback = f_ItemCallback;
            await DelayAsync();
        }
        public async UniTask CreateList(int f_Count)
        {
            if (m_Item == null) return;
            await CloseAsync();

            for (int i = 0; i < f_Count; i++)
            {
                var index = i;
                var item = GameObject.Instantiate(m_Item, m_Item.transform.parent);
                item.gameObject.SetActive(true);
                m_ItemCallback?.Invoke(index, item);
            }
        }
        public async UniTask CloseAsync()
        {
            UniTask[] tasks = new UniTask[m_ItemList.Count];


            for (int i = 0; i < m_ItemList.Count; i++)
            {
                var tempItem = m_ItemList[i];
                tasks[i] = UniTask.Create(async () =>
                {
                    await tempItem.OnDestroyAsync();
                    GameObject.Destroy(tempItem.gameObject);
                });
            }
            m_ItemList.Clear();

            await UniTask.WhenAll(tasks);
        }
    }
}
