using System.Collections;
using System.Collections.Generic;
using B1;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyNavigationBar : UIWindow
{
    private UILobbyPage CurPage => GetPage<UILobbyPage>();
    public Button m_Button;


    public override async UniTask AwakeAsync()
    {
        await DelayAsync();
        m_Button.onClick.AddListener(OnClickAsync);
    }

    public override async UniTask OnShowAsync()
    {
        await DelayAsync();
    }

    public override async UniTask ShowAsync()
    {
        await base.ShowAsync();
        
        Log($"{CurPage.NNmae}");
    }

    public async void OnClickAsync()
    {
        //await CurPage.ShowAsync(EAssetName.UILobby);
        await UIWindowManager.Instance.OpenPageAsync<UIAppPlanePage>();
    }
}
