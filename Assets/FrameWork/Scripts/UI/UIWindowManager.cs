using System;
using System.Collections;
using System.Collections.Generic;
using B1;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using B1.Event;

namespace B1.UI
{
    public sealed class UIWindowManager : MonoSingleton<UIWindowManager>
    {
        private DicStack<Type, UIWindowPage> m_PageStack = new("UI Window Stack Info");

        protected override void Awake()
        {
            base.Awake();
            // 初始化 UI 层级
            for (int i = 0; i < (int)EUIAppRoot.EnumCount; i++)
            {
                if (transform.Find($"{(EUIAppRoot)i}") == null)
                {
                    var obj = new GameObject($"{(EUIAppRoot)i}");
                    var rect = obj.AddComponent<RectTransform>();
                    rect.SetParent(transform);
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.pivot = Vector2.one * 0.5f;
                    rect.anchoredPosition3D = Vector3.zero;
                    rect.sizeDelta = Vector2.zero;
                    rect.localScale = Vector3.one;
                }
            }
        }

        #region Page
        /// <summary>
        /// 打开一个 page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async UniTask<T> OpenPageAsync<T>() where T : UIWindowPage, new()
        {
            var key = typeof(T);
            if (!m_PageStack.TryGetValue(key, out var value))
            {
                T window = new();
                m_PageStack.Push(key, window);
                Log($"开始加载 UI Window Page    page name = {typeof(T)}");
                await window.InitAsync();
            }
            else
            {
                Log($"重复开启 UI Window Page 已经被打开  key = {key}   value = {value}");
            }
            return m_PageStack[key] as T;
        }
        /// <summary>
        /// 打开一个 page
        /// </summary>
        /// <param name="f_Type"></param>
        /// <returns></returns>
        private async UniTask OpenPageAsync(Type f_Type)
        {
            await UniTask.Delay(0);
            if (f_Type == null)
            {
                LogError($"传入参数为空    f_Type = {f_Type}");
                return;
            }

            if (!m_PageStack.TryGetValue(f_Type, out var value))
            {
                var window = Activator.CreateInstance(f_Type);
                m_PageStack.Push(f_Type, window as UIWindowPage);
                Log($"开始加载 UI Window Page    page name = {f_Type}");
                var method_InitAsync = f_Type.GetMethod("InitAsync", BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public);
                method_InitAsync.Invoke(window, new object[] { });
            }
            else
            {
                Log($"重复开启 UI Window Page 已经被打开  key = {f_Type}   value = {value}");
            }
        }
        /// <summary>
        /// 打开一个 page
        /// </summary>
        /// <param name="f_EUIWindowPage"></param>
        /// <returns></returns>
        public async UniTask OpenPageAsync(EUIWindowPage f_EUIWindowPage)
        {
            var type = Type.GetType(f_EUIWindowPage.ToString());
            if (type == null)
            {
                LogError($"打开 page 失败    类型不存在    f_EUIWindowPage = {f_EUIWindowPage}     type = {type}");
                return;
            }
            await OpenPageAsync(type);
        }
        /// <summary>
        /// 关闭栈顶 page
        /// </summary>
        /// <returns></returns>
        public async UniTask ClosePageAsync()
        {
            if (m_PageStack.TryPop(out var value))
            {
                await value.CloseAsync();
            }
        }
        /// <summary>
        /// 获取一个 page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async UniTask<T> GetPageAsync<T>() where T : UIWindowPage
        {
            await UniTask.Delay(0);
            T retPage = default(T);
            if (m_PageStack.TryGetValue(typeof(T), out var page) && page != null && page as T != null)
            {
                retPage = page as T;
            }
            else
            {
                Log($"获取 UI Window Page 失败 type = {typeof(T)}");
            }
            return retPage;
        }
        #endregion

        #region 工具 tool
        /// <summary>
        /// 获取 ui 层级父对象
        /// </summary>
        /// <param name="f_EUIRoot"></param>
        /// <returns></returns>
        public async UniTask<Transform> GetUIRootAsync(EUIAppRoot f_EUIRoot)
        {
            await UniTask.Delay(0);
            return transform.Find($"{f_EUIRoot}");
        }
        #endregion

        #region 加载 load
        /// <summary>
        /// 加载一个 UI window 窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f_EWindow"></param>
        /// <returns></returns>
        public async UniTask<T> LoadWindowAsync<T>(EAssetName f_EWindow)
            where T : UIWindow
        {
            T window = default(T);
            var result = await AssetsManager.Instance.LoadPrefabAsync<T>(f_EWindow, null);
            if (result.result == true && result.obj != null)
            {
                window = result.obj;
                var parent = await GetUIRootAsync(window.AppRoot);
                window.transform.SetParent(parent);
                window.Rect.NormalFullScene();
                await window.AwakeAsync();

                EventManager.Instance.FireEvent(EEvent.UI_WINDOW_LOAD_FINISH, window, f_EWindow.ToString());

                LogWarning($"加载一个窗口    window name = {f_EWindow}     window = {window}");
            }
            else
            {
                LogError($"UI Window 加载失败  请检查资源是否存在   或者资源被重复加载       window name = {f_EWindow}");
            }

            return window;
        }
        /// <summary>
        /// 写在一个 UI window 窗口
        /// </summary>
        /// <param name="f_EWindow"></param>
        /// <param name="f_Obj"></param>
        /// <returns></returns>
        public async UniTask UnloadWindowAsync(EAssetName f_EWindow, UIWindow f_Obj)
        {
            await AssetsManager.Instance.UnLoadPrefabAsync(f_EWindow, EAssetLable.Prefab, f_Obj);

            LogWarning($"卸载窗口    f_EWindow = {f_EWindow} ");

        }
        #endregion


    }
}