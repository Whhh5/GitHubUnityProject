using System;
using System.Collections;
using System.Collections.Generic;
using B1;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace B1.UI
{
    public sealed class UIWindowManager : MonoSingleton<UIWindowManager>
    {
        ListStack<Type, UIWindowPage> m_WindowStack = new("UI Window Stack Info");
        Dictionary<EWindow, UIWindow> m_Windows = new();

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

        public async UniTask<T> OpenPageAsync<T>() where T : UIWindowPage, new()
        {
            var key = typeof(T);
            if (!m_WindowStack.TryGetValue(key, out var value))
            {
                T window = new();
                m_WindowStack.Push(key, window);
                Log($"开始加载 UI Window Page    page name = {typeof(T)}");
                await window.InitAsync();
            }
            else
            {
                Log($"重复开启 UI Window Page 已经被打开  key = {key}   value = {value}");
            }
            return m_WindowStack[key] as T;
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
        }
        private async UniTask OpenPageAsync(Type f_Type)
        {
            await UniTask.Delay(0);
            if (f_Type == null)
            {
                LogError($"传入参数为空    f_Type = {f_Type}");
                return;
            }

            if (!m_WindowStack.TryGetValue(f_Type, out var value))
            {
                var window = Activator.CreateInstance(f_Type);
                m_WindowStack.Push(f_Type, window as UIWindowPage);
                Log($"开始加载 UI Window Page    page name = {f_Type}");
                var method_InitAsync = f_Type.GetMethod("InitAsync", BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public);
                method_InitAsync.Invoke(window, new object[] { });
            }
            else
            {
                Log($"重复开启 UI Window Page 已经被打开  key = {f_Type}   value = {value}");
            }
        }
        public async UniTask ClosePageAsync()
        {
            if (m_WindowStack.TryPop(out var value))
            {
                await value.CloseAsync();
            }
        }
        public async UniTask<UIWindowPage> GetPageAsyn<T>() where T : UIWindowPage
        {
            await UniTask.Delay(0);
            if (!m_WindowStack.TryGetValue(typeof(T), out var page))
            {
                Log($"获取 UI Window Page 失败 type = {typeof(T)}");
            }
            return page;
        }

        public async UniTask<string> GetPathAsync(EWindow f_EWindow)
        {
            await UniTask.Delay(0);
            return $"{PathManager.UIWindow}{f_EWindow}.prefab";
        }
        public async UniTask<Transform> GetUIRootAsync(EUIRoot f_EUIRoot)
        {
            await UniTask.Delay(0);
            return transform.Find($"{f_EUIRoot}");
        }
        public async UniTask<T> LoadWindowAsync<T>(EWindow f_EWindow, EUIRoot f_EUIRoot)
            where T : UIWindow
        {
            T window;
            if (m_Windows.TryGetValue(f_EWindow, out var value) && value != null)
            {
                window = value as T;
            }
            else
            {
                var path = await GetPathAsync(f_EWindow);
                var parent = await GetUIRootAsync(f_EUIRoot);
                window = await AssetsManager.Instance.LoadPrefabAsync<T>(path, parent);
                if (window != null)
                {
                    m_Windows.Remove(f_EWindow);
                    m_Windows.Add(f_EWindow, window);
                }
                else
                {
                    Log($"UI Window 加载失败  请检查资源是否存在  window name = {f_EWindow}");
                }

            }

            Log($"加载一个窗口    window name = {f_EWindow}     window = {window}");

            return window;
        }

        public async UniTask UnloadWindowAsync(EWindow f_EWindow)
        {
            if (m_Windows.TryGetValue(f_EWindow, out var value) && value != null && value.m_CurentPageType != EUIWindowPage.None)
            {
                var path = await GetPathAsync(f_EWindow);
                await AssetsManager.Instance.UnloadAsync(path);
                m_Windows.Remove(f_EWindow);
            }
            else
            {
                var pageStr = value != null ? value.m_CurentPageType : EUIWindowPage.EnumCount;
                LogError($"卸载窗口失败  ( 在 page 中的窗口不能单独关闭 )   m_Windows = {f_EWindow}    " +
                    $"ContainsKey = {m_Windows.ContainsKey(f_EWindow)}    value = {value}     page = {pageStr}");
            }
        }
    }
}