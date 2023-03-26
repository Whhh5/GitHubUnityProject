using System.Collections;
using System.Collections.Generic;
using B1;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIAppPlane : UIWindow
{
    [SerializeField]
    private Button m_OpenLobbyBtn = null;

    public override async UniTask AwakeAsync()
    {
        await DelayAsync();
        m_OpenLobbyBtn.onClick.AddListener(async () => await UIWindowManager.Instance.OpenPageAsync<UILobbyPage>());
    }

    public override async UniTask OnShowAsync()
    {
        await DelayAsync();

    }
    public override async UniTask ShowAsync()
    {
        await base.ShowAsync();
    }

    public override async UniTask HideAsync()
    {
        await base.HideAsync();
    }

    protected override void OnDestroy()
    {
        m_OpenLobbyBtn.onClick.RemoveAllListeners();


        base.OnDestroy();
    }

    #region Data
    [SerializeField, HideInInspector]
    private List<EUIWindowPage> m_List = new();
    public List<EUIWindowPage> List { get => m_List; }
    [SerializeField, HideInInspector]
    private List<bool> m_ListIsSelect = new();

    public bool TryGetValue(EUIWindowPage f_Key, out bool f_IsSelect)
    {
        var isTry = false;
        f_IsSelect = false;
        if (m_List.Contains(f_Key))
        {
            var index = m_List.IndexOf(f_Key);
            f_IsSelect = m_ListIsSelect[index];
            isTry = true;
        }
        return isTry;
    }
    public void SetValue(EUIWindowPage f_Key, bool f_IsSelect = true)
    {
        if (m_List.Contains(f_Key))
        {
            var index = m_List.IndexOf(f_Key);
            m_ListIsSelect[index] = f_IsSelect;
        }
        else
        {
            Add(f_Key, f_IsSelect);
        }
    }
    public void Add(EUIWindowPage f_Key, bool f_IsSelect = true)
    {
        if (!m_List.Contains(f_Key))
        {
            m_List.Add(f_Key);
            m_ListIsSelect.Add(f_IsSelect);
        }
    }
    public void Remove(EUIWindowPage f_Key)
    {
        if (m_List.Contains(f_Key))
        {
            var index = m_List.IndexOf(f_Key);
            m_List.RemoveAt(index);
            m_ListIsSelect.RemoveAt(index);
        }
    }

    #endregion

}
