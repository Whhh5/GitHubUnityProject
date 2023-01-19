using System.Collections;
using System.Collections.Generic;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UILobbyPage : UIWindowPage
{
    public string NNmae = "adasdasdadas";

    public override async UniTask<List<(EPrefab eWindow, EUIRoot root)>> GetWindowNameAsync()
    {
        await UniTask.Delay(0);
        var windowList = new List<(EPrefab eWindow, EUIRoot root)>()
        {
            (EPrefab.UILobby, EUIRoot.App1),
            (EPrefab.UILobbyItemInfo, EUIRoot.App1),
            (EPrefab.UILobbyNavigationBar, EUIRoot.App1),
        };
        return windowList;
    }
}
