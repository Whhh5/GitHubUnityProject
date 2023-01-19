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
            await AssetsManager.Instance.LoadPrefabAsync<UGUISystem>(EPrefab.UGUISystem, null);
            await UIWindowManager.Instance.OpenPageAsync<UILobbyPage>();


        }
    }
}
