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

        public async UniTask OpenPageAsync<T>() where T : UIWindowPage, new()
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
        }
        public void OpenPageAsync(Type f_Type)
        {
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
            //var key = typeof(T);
            //if (!m_WindowStack.TryGetValue(key, out var value))
            //{
            //    T window = new();
            //    m_WindowStack.Push(key, window);
            //    Log($"开始加载 UI Window Page    page name = {typeof(T)}");
            //    await window.InitAsync();
            //}
            //else
            //{
            //    Log($"重复开启 UI Window Page 已经被打开  key = {key}   value = {value}");
            //}
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
        public async UniTask<UIWindow> LoadWindowAsync(EWindow f_EWindow, EUIRoot f_EUIRoot)
        {
            var path = await GetPathAsync(f_EWindow);
            var parent = await GetUIRootAsync(f_EUIRoot);
            var window = await AssetsManager.Instance.LoadPrefabAsync<UIWindow>(path, parent);
            if (window == null)
            {
                Log($"UI Window 加载失败  window name = {f_EWindow}");
            }
            return window;
        }
        public async UniTask UnloadWindowAsync(EWindow f_EWindow)
        {
            var path = await GetPathAsync(f_EWindow);
            await AssetsManager.Instance.UnloadAsync(path);
        }
    }
}