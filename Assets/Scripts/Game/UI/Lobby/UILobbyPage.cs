using System.Collections;
using System.Collections.Generic;
using B1;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UILobbyPage : UIWindowPage
{
    public string NNmae = "adasdasdadas";


    protected override EAssetName SpriteAltas => EAssetName.UILobbySpriteAltas;

    protected override List<EAssetName> GetWindowNameAsync()
    {
        var windowList = new List<EAssetName >()
        {
            EAssetName.UILobby,
            EAssetName.UILobbyItemInfo,
            EAssetName.UILobbyNavigationBar,
        };
        return windowList;
    }
}
