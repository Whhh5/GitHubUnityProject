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
        private Dictionary<Type, UIWindowPage> m_DicPage = new();

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
            if (!PageIsExistAsync(key))
            {
                T window = new();
                m_DicPage.Add(key, window);
                Log($"开始加载 UI Window Page    page name = {typeof(T)}");
                await window.InitAsync();
            }
            else
            {
                Log($"重复开启 UI Window Page 已经被打开  key = {key} ");
            }

            return m_DicPage[key] as T;
        }
        /// <summary>
        /// 打开一个 page, 该方式可加入 page 栈中
        /// </summary>
        /// <param name="f_Type"></param>
        /// <returns></returns>
        private async UniTask<UIWindowPage> OpenPageAsync(Type f_Type)
        {
            await UniTask.Delay(0);
            UIWindowPage page = null;
            if (f_Type == null)
            {
                LogError($"传入参数为空    f_Type = {f_Type}");
                return page;
            }

            if (!PageIsExistAsync(f_Type))
            {
                var window = Activator.CreateInstance(f_Type);
                m_PageStack.Push(f_Type, window as UIWindowPage);
                Log($"开始加载 UI Window Page    page name = {f_Type}");
                var method_InitAsync = f_Type.GetMethod("InitAsync", BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public);
                method_InitAsync.Invoke(window, new object[] { });
                page = window as UIWindowPage;
            }
            else
            {
                Log($"重复开启 UI Window Page 已经被打开  key = {f_Type}   value = {f_Type}");
            }
            return page;
        }
        /// <summary>
        /// 打开一个 page, 该方式不会加入到 page 栈中
        /// </summary>
        /// <param name="f_EUIWindowPage"></param>
        /// <returns></returns>
        public async UniTask<UIWindowPage> OpenPageAsync(EUIWindowPage f_EUIWindowPage)
        {
            var type = Type.GetType(f_EUIWindowPage.ToString());
            if (type == null)
            {
                LogError($"打开 page 失败    类型不存在    f_EUIWindowPage = {f_EUIWindowPage}     type = {type}");
                return null;
            }
            return await OpenPageAsync(type);
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
        public async UniTask ClosePageAsync<T>() where T : UIWindowPage
        {
            var type = typeof(T);
            if (m_DicPage.TryGetValue(type, out var page))
            {
                await page.CloseAsync();
                m_DicPage.Remove(type);
            }
            else
            {
                Log($"当前 page 未打开 type = {type}");
            }
        }
        /// <summary>
        /// 获取一个 page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async UniTask<T> GetPageAsync<T>(EUIWindowPage f_Page) where T : UIWindowPage
        {
            await UniTask.Delay(0);
            Type type = Type.GetType(f_Page.ToString());
            T retPage = null;
            if (m_PageStack.TryGetValue(type, out var page) && page != null && page as T != null)
            {
                retPage = page as T;
            }
            else
            {
                Log($"获取 UI Window Page 失败 type = {typeof(T)}");
            }
            return retPage;
        }
        /// <summary>
        /// 获取一个 page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async UniTask<T> GetPageAsync<T>() where T : UIWindowPage
        {
            await UniTask.Delay(0);
            Type type = typeof(T);
            T retPage = null;
            if (m_DicPage.TryGetValue(type, out var page) && page != null && page as T != null)
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
        private bool PageIsExistAsync(Type f_Type)
        {
            return m_PageStack.TryGetValue(f_Type, out var value) || m_DicPage.ContainsKey(f_Type);
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

                MessagingSystem.Instance.SendEvent(EEvent.UI_WINDOW_LOAD_FINISH, window, f_EWindow.ToString());

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