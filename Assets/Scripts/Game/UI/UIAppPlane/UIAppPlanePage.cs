using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using B1.UI;
using Cysharp.Threading.Tasks;

public class UIAppPlanePage : UIWindowPage
{
    protected override EAssetName SpriteAltas => EAssetName.None;

    protected override List<EAssetName> GetWindowNameAsync()
    {
        return new List<EAssetName>()
        {
            EAssetName.UIAppPlane,
        };
    }
}
