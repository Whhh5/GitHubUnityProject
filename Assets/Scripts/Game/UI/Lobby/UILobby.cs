using System.Collections;
using System.Collections.Generic;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UILobby : UIWindow
{
    public override async UniTask OnEnableAsync()
    {
        await base.OnEnableAsync();

        var page = await GetPage<UILobbyPage>();
        Log($"{page.NNmae}");
    }
}
