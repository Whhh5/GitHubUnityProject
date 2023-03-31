using System;
using System.Collections;
using System.Collections.Generic;
using B1.Event;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace B1.UI
{
    public abstract class  UIWindowPage : Base, IUIWindowPage
    {

        protected abstract List<EAssetName> GetWindowNameAsync();
        protected abstract EAssetName SpriteAltas { get; }

        protected ListStack<EAssetName> m_WindowStack = new("window page stack", 5);
        private Dictionary<EAssetName, UIWindow> m_DicWindow = new();
        private SpriteAtlas m_SpriteAtlas = null;
        public EUIWindowPage CurPage { get; private set; }
        public async UniTask InitAsync()
        {
            var fieldInfo = typeof(EUIWindowPage).GetField($"{GetType()}");
            if (fieldInfo == null)
            {
                Debug.Log($"当前配置类型获取失败   请到  EUIWindowPage 中注册");
            }
            //CurPage = (EUIWindowPage)fieldInfo.GetValue(null);

            // 加载 UI window 窗口
            var windowList = GetWindowNameAsync();
            if (windowList != null && windowList.Count > 0)
            {
                UniTask[] tasks = new UniTask[windowList.Count];
                for (int i = 0; i < windowList.Count; i++)
                {
                    var windowName = windowList[i];
                    tasks[i] = UniTask.Create(
                        async () =>
                        {
                            var window = await UIWindowManager.Instance.LoadWindowAsync<UIWindow>(windowName);
                            if (window != null)
                            {
                                window.gameObject.SetActive(false);
                                await window.SetPage(this);

                                m_DicWindow.Add(windowName, window);
                            }
                            else
                            {
                                LogError($"加载当前窗口失败  window name = {windowName}");
                            }
                        });
                }
                await UniTask.WhenAll(tasks);

                var firstWindowName = windowList[0];
                await ShowStackAsync(firstWindowName);


                // 加载图集
                if (!Enum.Equals(SpriteAltas, EAssetName.None))
                {
                    var result = await AssetsManager.Instance.LoadAssetAsync<SpriteAtlas>(SpriteAltas, EAssetLable.spriteAtlas, (int)SpriteAltas);
                    if (result.tResult)
                    {
                        m_SpriteAtlas = result.tObj;
                    }
                    else
                    {
                        LogError($"加载图集失败，如需要加载图片请创建图集 ");
                    }
                }
            }
            else
            {
                Log("UIPage 打开失败  未获取到当前 page 的窗口   请重写函数并返回 GetWindowNameAsync()");
            }

        }
        /// <summary>
        /// 先是一个可以入栈的窗口
        /// </summary>
        /// <param name="f_Window"></param>
        /// <returns></returns>
        public async UniTask ShowStackAsync(EAssetName f_Window)
        {
            if (m_WindowStack.TryValue(out var value))
            {
                await HideAsync(value);
            }
            await ShowAsync(f_Window);
            m_WindowStack.Push(f_Window);
        }
        /// <summary>
        /// 关闭栈顶窗口
        /// </summary>
        /// <returns></returns>
        public async UniTask HideStackAsync()
        {
            if(m_WindowStack.Count > 1 && m_WindowStack.TryPop(out var window))
            {
                await HideAsync(window);
                m_WindowStack.TryValue(out var value);
                await ShowAsync(value);
            }
            else
            {
                Log($"window stack no have element    count = {m_WindowStack.Count}");
            }
        }
        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <param name="f_Window"></param>
        /// <returns></returns>
        public async UniTask ShowAsync(EAssetName f_Window)
        {
            if (m_DicWindow.TryGetValue(f_Window, out var value))
            {
                await value.ShowAsync();
                EventManager.Instance.FireEvent(EEvent.UI_WINDOW_HIDE, value, f_Window.ToString());
            }
            else
            {
                LogError($"窗口不存在当前 page 中     f_Window = {f_Window}");
            }
        }
        /// <summary>
        /// 隐藏一个窗口
        /// </summary>
        /// <param name="f_Window"></param>
        /// <returns></returns>
        public async UniTask HideAsync(EAssetName f_Window)
        {
            if (m_DicWindow.TryGetValue(f_Window, out var value))
            {
                await value.HideAsync();
                EventManager.Instance.FireEvent(EEvent.UI_WINDOW_HIDE, value, f_Window.ToString());
            }
            else
            {
                LogError($"窗口不存在当前 page 中     f_Window = {f_Window}");
            }
        }
        /// <summary>
        /// 隐藏 page 中所有窗口
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 获取以恶搞当前 page 中的窗口
        /// </summary>
        /// <param name="f_EWindow"></param>
        /// <returns></returns>
        public UIWindow GetWindowAsync(EAssetName f_EWindow)
        {
            if (!m_DicWindow.TryGetValue(f_EWindow, out var window))
            {
                Log($"不存在该窗口 window name = {f_EWindow}");
            }
            return window;
        }

        /// <summary>
        /// 加载当前 page 图集中的图片资源
        /// </summary>
        /// <param name="f_Sprite"></param>
        /// <returns></returns>
        public async UniTask<(bool tResult, Sprite tSprite)> LoadSpriteAsync(ESpriteName f_Sprite)
        {
            await DelayAsync();
            var tryValue = false;
            var sprite = m_SpriteAtlas?.GetSprite(f_Sprite.ToString());
            if (!object.ReferenceEquals(sprite, null))
            {
                tryValue = true;
            }
            return (tryValue, sprite);
        }


        /// <summary>
        /// 关闭当前 page
        /// </summary>
        /// <returns></returns>
        public async UniTask CloseAsync()
        {
            UniTask[] tasks = new UniTask[m_DicWindow.Count + 1];
            uint index = 0;

            // 卸载当前 page 窗口
            foreach (var item in m_DicWindow)
            {
                var window = item;
                tasks[index++] = UniTask.Create(async () =>
                {
                    await UIWindowManager.Instance.UnloadWindowAsync(window.Key, window.Value);
                    EventManager.Instance.FireEvent(EEvent.UI_WINDOW_UNLOAD_FINISH, window.Key, window.Key.ToString());
                });
            }

            // 卸载图集
            if (!object.ReferenceEquals(m_SpriteAtlas, null))
            {
                tasks[index++] = UniTask.Create(async () =>
                {
                    await AssetsManager.Instance.UnloadAsync(SpriteAltas, EAssetLable.spriteAtlas, (int)SpriteAltas);
                });
            }

            await UniTask.WhenAll(tasks);
            m_DicWindow.Clear();
        }
    }
}