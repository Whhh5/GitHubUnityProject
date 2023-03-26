using B1.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINavigationBar : UIWindow
{
    [SerializeField]
    private Button m_BtnRetreat = null;
    [SerializeField]
    private Button m_BtnAdvace = null;

    public override async UniTask AwakeAsync()
    {
        await DelayAsync();
        m_BtnRetreat.onClick.AddListener(async () =>
            {
                await UIWindowManager.Instance.ClosePageAsync();
            });


    }

    public override async UniTask OnShowAsync()
    {
        await DelayAsync();

    }
}
