using B1.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINavigationBarPage : UIWindowPage
{
    public override async UniTask<List<(EPrefab eWindow, EUIRoot root)>> GetWindowNameAsync()
    {
        return null;
    }
}
