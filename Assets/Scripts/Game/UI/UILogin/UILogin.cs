using System.Collections;
using System.Collections.Generic;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UILogin : UIWindow
{


    public override async UniTask AwakeAsync()
    {
        await DelayAsync();

    }

    public override async UniTask OnShowAsync()
    {
        await DelayAsync();

    }
    public override async UniTask ShowAsync()
    {
        await base.ShowAsync();

    }
}