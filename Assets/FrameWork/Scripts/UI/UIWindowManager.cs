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
        ListStack<Type, UIWindowPage> m_PageStack = new("UI Window Stack Info");

        public override void Awake()
        {
            base.Awake();

            for (int i = 0; i < (int)EUIRoot.EnumCount; i++)
            {
                if (transform.Find($"{(EUIRoot)i}") == null)
                {
                    var obj = new GameObject($"{(EUIRoot)i}");
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
            await UpdateUINavigationBarAsync();
            return m_PageStack[key] as T;
        }
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
            await UpdateUINavigationBarAsync();
        }
        public async UniTask OpenPageAsync(EUIWindowPage f_EUIWindowPage)
        {
            var type = Type.GetType(f_EUIWindowPage.ToString());
            if (type == null)
            {
                LogError($"打开 page 失败    类型不存在    f_EUIWindowPage = {f_EUIWindowPage}     type = {type}");
                return;
            }
            await OpenPageAsync(type);
            await UpdateUINavigationBarAsync();
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
                await UpdateUINavigationBarAsync();
            }
        }
        public async UniTask<T> GetPageAsyn<T>() where T : UIWindowPage
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
        public async UniTask<Transform> GetUIRootAsync(EUIRoot f_EUIRoot)
        {
            await UniTask.Delay(0);
            return transform.Find($"{f_EUIRoot}");
        }
        #endregion

        #region 加载 load
        public async UniTask<T> LoadWindowAsync<T>(EPrefab f_EWindow, EUIRoot f_EUIRoot)
            where T : UIWindow
        {
            T window = default(T);
            var parent = await GetUIRootAsync(f_EUIRoot);
            var result = await AssetsManager.Instance.LoadPrefabAsync<T>(f_EWindow, parent);
            if (result.result == true && result.obj != null)
            {
                window = result.obj;
                window.m_AppRoot = f_EUIRoot;
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

        public async UniTask UnloadWindowAsync(EPrefab f_EWindow, GameObject f_Obj)
        {
            await AssetsManager.Instance.UnloadAsync(f_EWindow, EAssetLable.Prefab, f_Obj);

            LogWarning($"卸载窗口    f_EWindow = {f_EWindow} ");

        }
        #endregion






        #region 特殊 other
        public UINavigationBar m_Navigation = null;
        /// <summary>
        /// 更新 page 返回按钮
        /// </summary>
        /// <returns></returns>
        public async UniTask UpdateUINavigationBarAsync()
        {
            if (m_PageStack.Count > 0)
            {
                m_Navigation ??= await LoadWindowAsync<UINavigationBar>(EPrefab.UINavigationBar, EUIRoot.System);
            }
            else if (m_Navigation != null)
            {
                await UnloadWindowAsync(EPrefab.UINavigationBar, m_Navigation.gameObject);
                m_Navigation = null;
            }
        }
        #endregion
    }
}