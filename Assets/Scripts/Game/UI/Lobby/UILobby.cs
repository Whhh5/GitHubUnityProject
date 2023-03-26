using System.Collections;
using System.Collections.Generic;
using B1.UI;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using B1;

public class UILobby : UIWindow
{
    public UILobbyPage CurPage => GetPage<UILobbyPage>();


    public Button m_Button;
    public ESpriteName m_SpriteName;
    public Image m_TestImage = null;



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
        await CurPage.ShowStackAsync(EAssetName.UILobbyItemInfo);


        var result = await CurPage.LoadSpriteAsync(m_SpriteName);
        if (result.tResult)
        {
            m_TestImage.sprite = result.tSprite;
        }
    }
}
