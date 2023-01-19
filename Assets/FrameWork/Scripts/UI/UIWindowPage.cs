using System;
using System.Collections;
using System.Collections.Generic;
using B1.Event;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace B1.UI
{
    public abstract class UIWindowPage : Base, IUIWindowPage
    {
        public abstract UniTask<List<(EPrefab eWindow, EUIRoot root)>> GetWindowNameAsync();

        private Dictionary<EPrefab, UIWindow> m_DicWindow = new();
        public async UniTask InitAsync()
        {
            var fieldInfo = typeof(EUIWindowPage).GetField($"{GetType()}");
            if (fieldInfo == null)
            {
                Debug.Log($"当前配置类型获取失败   请到  EUIWindowPage 中注册");
            }
            var pageType = (EUIWindowPage)fieldInfo.GetValue(null);

            var windowList = await GetWindowNameAsync();
            if (windowList != null && windowList.Count > 0)
            {
                UniTask[] tasks = new UniTask[windowList.Count];
                for (int i = 0; i < windowList.Count; i++)
                {
                    var windowInfo = windowList[i];
                    tasks[i] = UniTask.Create(
                        async () =>
                        {
                            var window = await UIWindowManager.Instance.LoadWindowAsync<UIWindow>(windowInfo.eWindow, windowInfo.root);
                            if (window != null)
                            {
                                window.gameObject.SetActive(false);
                                window.m_CurentPageType = pageType;
                                window.m_CurPage = this;

                                m_DicWindow.Add(windowInfo.eWindow, window);
                            }
                            else
                            {
                                
                            }
                        });
                }
                await UniTask.WhenAll(tasks);
                await ShowAsync(windowList[0].eWindow);
            }
            else
            {
                Log("UIPage 打开失败  未获取到当前 page 的窗口   请重写函数并返回 GetWindowNameAsync()");
            }
        }
        public async UniTask ShowAsync(EPrefab f_Window)
        {
            try
            {
                var window = m_DicWindow[f_Window];
                await window.ShowAsync();
                EventManager.Instance.FireEvent(EEvent.UI_WINDOW_HIDE, window, f_Window.ToString());
            }
            catch (Exception)
            {
                LogError($"窗口不存在当前 page 中     f_Window = {f_Window}");
            }
        }
        public async UniTask HideAsync(EPrefab f_Window)
        {
            var window = m_DicWindow[f_Window];
            await window.HideAsync();
            EventManager.Instance.FireEvent(EEvent.UI_WINDOW_HIDE, window, f_Window.ToString());
        }
        public async UniTask CloseAsync()
        {
            UniTask[] tasks = new UniTask[m_DicWindow.Count];
            uint index = 0;
            foreach (var item in m_DicWindow)
            {
                var window = item;
                tasks[index++] = UniTask.Create(async () =>
                {
                    await UIWindowManager.Instance.UnloadWindowAsync(window.Key, window.Value.gameObject);
                    EventManager.Instance.FireEvent(EEvent.UI_WINDOW_UNLOAD_FINISH, window.Key, window.Key.ToString());
                });
            }
            await UniTask.WhenAll(tasks);

        }
        public async UniTask HideAllAsync()
        {
            UniTask[] tasks = new UniTask[m_DicWindow.Count];
            uint index = 0;
            foreach (var item in m_DicWindow)
            {
                var window = item;
                tasks[index++] = UniTask.Create(async () =>
                {
                    await HideAsync(window.Key);
                });
            }
            await UniTask.WhenAll(tasks);
        }


        public async UniTask<UIWindow> GetWindowAsync(EPrefab f_EWindow)
        {
            await UniTask.Delay(0);
            if (!m_DicWindow.TryGetValue(f_EWindow, out var window))
            {
                Log($"不存在该窗口 window name = {f_EWindow}");
            }
            return window;
        }

        //public async UniTask<UIWindow> GetWindowAsync(EPrefab f_EPrefab)
        //{

        //}

    }
}