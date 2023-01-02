using System.Collections;
using System.Collections.Generic;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UILobbyPage : UIWindowPage
{
    public override async UniTask<List<(EWindow eWindow, EUIRoot root)>> GetWindowNameAsync()
    {
        await UniTask.Delay(0);
        var windowList = new List<(EWindow eWindow, EUIRoot root)>()
        {
            (EWindow.UILobby, EUIRoot.App1),
            (EWindow.UILobbyItemInfo, EUIRoot.App1),
            (EWindow.UILobbyNavigationBar, EUIRoot.App1),
        };
        return windowList;
    }
}
