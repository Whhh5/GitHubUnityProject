using System;
using System.Collections;
using System.Collections.Generic;
using B1.UI;
using UnityEngine;

namespace B1
{

    public class GameManager : Base
    {
        [RuntimeInitializeOnLoadMethod]
        public static async void StartGame()
        {
            var uiManagerPath = $"{PathManager.UIWindow}UGUISystem.prefab";
            await AssetsManager.Instance.LoadPrefabAsync<UGUISystem>(uiManagerPath, null);
            await UIWindowManager.Instance.OpenPageAsync<UILobbyPage>();


        }
    }
}
