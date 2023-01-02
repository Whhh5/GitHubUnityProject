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
        public abstract UniTask<List<(EWindow eWindow, EUIRoot root)>> GetWindowNameAsync();

        private Dictionary<EWindow, UIWindow> m_DicWindow = new();
        public async UniTask InitAsync()
        {
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
                            var window = await UIWindowManager.Instance.LoadWindowAsync(windowInfo.eWindow, windowInfo.root);
                            if (window != null)
                            {
                                await window.InitAsync();
                                m_DicWindow.Add(windowInfo.eWindow, window);
                                EventManager.Instance.FireEvent(EEvent.UI_WINDOW_LOAD_FINISH, window, windowInfo.ToString());
                            }
                            else
                            {
                            }
                        });
                }
                await UniTask.WhenAll(tasks);
                await HideAllAsync();
                await ShowAsync(windowList[0].eWindow);
            }
            else
            {
                Log("UIPage 打开失败");
            }
        }
        public async UniTask ShowAsync(EWindow f_Window)
        {
            var window = m_DicWindow[f_Window];
            await window.ShowAsync();
            EventManager.Instance.FireEvent(EEvent.UI_WINDOW_HIDE, window, f_Window.ToString());
        }
        public async UniTask HideAsync(EWindow f_Window)
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
                    await UIWindowManager.Instance.UnloadWindowAsync(item.Key);
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


        public async UniTask<UIWindow> GetWindowAsync(EWindow f_EWindow)
        {
            await UniTask.Delay(0);
            if (!m_DicWindow.TryGetValue(f_EWindow, out var window))
            {
                Log($"不存在该窗口 window name = {f_EWindow}");
            }
            return window;
        }
    }
}