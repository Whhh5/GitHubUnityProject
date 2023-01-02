using System.Collections;
using System.Collections.Generic;
using B1.UIWindow;
using UnityEngine;

namespace B1
{

    public class GameManager
    {
        [RuntimeInitializeOnLoadMethod]
        public static async void StartGame()
        {
            var uiManagerPath = $"{PathManager.UIWindow}UGUISystem.prefab";
            await AssetsManager.Instance.LoadPrefabAsync<UGUISystem>(uiManagerPath, null);
            var window = await UIWindowManager.Instance.LoadWindowAsync(EWindow.UILogin, EUIRoot.App1);
        }
    }
}
