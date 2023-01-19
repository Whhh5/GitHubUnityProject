using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace B1.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIWindow : MonoBase, IOnDestroyAsync
    {
        enum EDGType
        {
            ShowUIWindow,
            EnumCount,
        }
        private string GetDGID(EDGType eDGType)
        {
            return $"{GetType()}_{eDGType}";
        }



        CanvasGroup m_CanvasGroup => GetComponent<CanvasGroup>();
        RectTransform m_Rect => GetComponent<RectTransform>();

        [HideInInspector]
        public EUIWindowPage m_CurentPageType = EUIWindowPage.None;
        public UIWindowPage m_CurPage = null;

        [HideInInspector]
        public EUIRoot m_AppRoot = EUIRoot.None;

        public async UniTask<T> GetPage<T>() where T : UIWindowPage
        {
            await UniTask.Delay(0);
            T page = null;
            if (m_CurPage != null && m_CurPage is T)
            {
                page = m_CurPage as T;
            }
            else
            {
                Log($"page 获取失败   想要获取的 page = {typeof(T)}      当前 page  m_CurPage = {m_CurPage}");
            }
            return page;
        }
        public virtual async UniTask AwakeAsync()
        {
            DOTween.To(() => 0.0f, (value) =>
              {
                  m_CanvasGroup.alpha = value;
                  m_Rect.localScale = Vector3.one * value;
                  m_Rect.anchoredPosition = new Vector2(0, -UnityEngine.Screen.height * (1 - value));
                  gameObject.SetActive(value > 0 ? true : false);
              }, 1, 1)
                .SetId(GetDGID(EDGType.ShowUIWindow))
                .SetAutoKill(false)
                .Pause();
        }
        /// <summary>
        /// 在 <see langword="OnEnable" /> 之后调用
        /// </summary>
        /// <returns></returns>
        public virtual async UniTask ShowAsync()
        {
            gameObject.SetActive(true);
            await OnEnableAsync();
            DOTween.PlayForward(GetDGID(EDGType.ShowUIWindow));
        }
        /// <summary>
        /// 在 <see langword="OnDisable"/> 之后调用
        /// </summary>
        /// <returns></returns>
        public virtual async UniTask HideAsync()
        {
            gameObject.SetActive(false);
            DOTween.PlayBackwards(GetDGID(EDGType.ShowUIWindow));
            await OnDisableAsync();
        }

        public virtual async UniTask OnEnableAsync()
        {

        }
        public virtual async UniTask OnDisableAsync()
        {

        }

        public async UniTask OnDestroyAsync()
        {
            DOTween.Kill(EDGType.ShowUIWindow);
        }
    }
}
