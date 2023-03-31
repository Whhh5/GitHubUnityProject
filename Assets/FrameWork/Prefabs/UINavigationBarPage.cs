using B1;
using B1.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINavigationBarPage : UIWindowPage
{
    protected override EAssetName SpriteAltas => EAssetName.None;

    protected override List<EAssetName> GetWindowNameAsync()
    {
        return new List<EAssetName>()
        {
            EAssetName.UINavigationBar
        };
    }
}
