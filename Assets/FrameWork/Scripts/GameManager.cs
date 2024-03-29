using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using B1.UI;
using UnityEngine;

namespace B1
{

    public class GameManager : Base
    {
        [RuntimeInitializeOnLoadMethod]
        public static async void StartGame()
        {
            await AssetsManager.Instance.LoadPrefabAsync<UGUISystem>(EAssetName.UGUISystem, null);


            await UIWindowManager.Instance.OpenPageAsync<UIAppPlanePage>();
            await UIWindowManager.Instance.OpenPageAsync<UINavigationBarPage>();
            await UIWindowManager.Instance.OpenPageAsync(EUIWindowPage.UILobbyPage);

        }
    }
}
