using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace B1.UI
{
    public class ScrollViewListItem : MonoBase, IOnDestroyAsync
    {
        public RectTransform m_Rect => GetComponent<RectTransform>();

        public async UniTask OnLoadAsync()
        {

        }
        private void OnBecameVisible()
        {
            Debug.Log(" 当前可见 ");
        }
        private void OnBecameInvisible()
        {
            Debug.Log(" 当前不可见 ");
        }
        public T GetCom<T>(EUIElementName f_Child = EUIElementName.None) where T : Component
        {
            Transform tran = null;
            switch (f_Child)
            {
                case EUIElementName.None:
                    tran = m_Rect;
                    break;
                default:
                    tran = m_Rect.Find(f_Child.ToString());
                    break;
            }
            return tran?.GetComponent<T>();
        }

        public async UniTask<(bool result, T component)> GetValueAsync<T>(EScrollViewListItem f_EScrollViewListItem)
            where T : MonoBehaviour
        {
            await UniTask.Delay(0);
            bool isTry = false;
            if (transform.TryFind<T>($"{f_EScrollViewListItem}", out var component))
            {
                isTry = true;
            }
            return (isTry, component);
        }
        public async UniTask<bool> SetSpriteAsync(EScrollViewListItem f_EScrollViewListItem, Sprite f_Sprite)
        {
            var isTry = false;
            var image = await GetValueAsync<Image>(f_EScrollViewListItem);
            if (image.result)
            {
                image.component.sprite = f_Sprite;
                isTry = true;
            }
            return isTry;
        }
        public async UniTask<bool> SetTMPAsync(EScrollViewListItem f_EScrollViewListItem, string f_Content)
        {
            var isTry = false;
            var tmp = await GetValueAsync<TextMeshProUGUI>(f_EScrollViewListItem);
            if (tmp.result)
            {
                tmp.component.text = f_Content;
                isTry = true;
            }
            return isTry;
        }


        public async UniTask OnDestroyAsync()
        {
            
        }
    }
}