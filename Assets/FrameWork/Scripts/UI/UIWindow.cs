using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace B1.UI
{
    [RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
    public abstract class UIWindow : MonoBase, IAppRoot, IOnDestroyAsync
    {
        #region 变量
        public CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();
        public RectTransform Rect => GetComponent<RectTransform>();

        private UIWindowPage m_Page = null;

        [SerializeField]
        protected EUIAppRoot m_AppRoot = EUIAppRoot.None;
        public EUIAppRoot AppRoot => m_AppRoot;
        #endregion

        private float m_DGTime = 0.5f;
        private float m_StartValue = 0.7f;

        #region 生命周期函数
        /// <summary>
        /// 将会在被加载出来首先调用
        /// </summary>
        /// <returns></returns>
        public async UniTask OnLoadAsync()
        {
            await DelayAsync();
            CanvasGroup.blocksRaycasts = CanvasGroup.interactable = false;
        }
        /// <summary>
        /// 将会在实体被创建时初始化 首先被调用
        /// </summary>
        /// <returns></returns>
        public abstract UniTask AwakeAsync();
        /// <summary>
        /// 在 ShowAsync 之前被调用
        /// </summary>
        /// <returns></returns>
        public abstract UniTask OnShowAsync();
        /// <summary>
        /// 将会在显示的时候被调用 <see langword="OnEnable" /> ֮
        /// </summary>
        /// <returns></returns>
        public virtual async UniTask ShowAsync()
        {
            await OnShowAsync();
            await DOTween.To(() => 0.0f, (value) =>
            {
                CanvasGroup.alpha = value;
                Rect.localScale = Vector3.one * value + Vector3.one * m_StartValue * (1 - value);
            }, 1, m_DGTime)
                .SetEase(Ease.InOutBack)
                .OnStart(()=>
                {
                    gameObject.SetActive(true);
                })
                .OnComplete(()=>
                {
                    CanvasGroup.blocksRaycasts = CanvasGroup.interactable = true;
                });
        }
        /// <summary>
        /// 将会在窗口被隐藏的时候调用 <see langword="OnDisable"/> ֮
        /// </summary>
        /// <returns></returns>
        public virtual async UniTask HideAsync()
        {
            await DOTween.To(() => 0.0f, (value) =>
            {
                CanvasGroup.alpha = value;
                Rect.localScale = Vector3.one * m_StartValue * value + Vector3.one * m_StartValue * value;
            }, 1, m_DGTime)
                .SetEase(Ease.InOutBack)
                .OnStart(() =>
                {
                    CanvasGroup.blocksRaycasts = CanvasGroup.interactable = false;
                })
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }
        /// <summary>
        /// 将会在实体被销毁的时候调用
        /// </summary>
        /// <returns></returns>
        public async UniTask OnDestroyAsync()
        {
            await DelayAsync();
        }
        #endregion






        /// <summary>
        /// 设置当前 page
        /// </summary>
        /// <param name="f_Page"></param>
        /// <returns></returns>
        public async UniTask SetPage(UIWindowPage f_Page)
        {
            await DelayAsync();
            m_Page = f_Page;
        }
        /// <summary>
        /// 获取当前窗口配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPage<T>() where T : UIWindowPage
        {
            T page = null;
            if (m_Page != null && m_Page is T)
            {
                page = m_Page as T;
            }
            else
            {
                Log($"page 获取失败   当前想要获取 page = {typeof(T)}      当前窗口 page  m_Page = {m_Page}");
            }
            return page;
        }

    }
}
