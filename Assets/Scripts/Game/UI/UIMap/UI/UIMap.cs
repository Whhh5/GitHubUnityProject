using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMap : MonoBehaviour
{
    [SerializeField]
    private Button m_BtnMiniMap = null;
    [SerializeField]
    private Button m_BtnBigMap = null;
    [SerializeField, Range(0, 2)]
    private float m_ChangeTime = 1.0f;

    [SerializeField]
    Dictionary<string,string> m_Dic = new();

    private void Awake()
    {
        m_BtnMiniMap.onClick.AddListener(OnBtnMiniMap);
        m_BtnBigMap.onClick.AddListener(OnBtnBigMap);
    }

    private void OnBtnBigMap()
    {
        UIMapManager.Instance.ChangeUIMapState(EUIMapStatus.MiniMap);
    }

    private void OnBtnMiniMap()
    {
        UIMapManager.Instance.ChangeUIMapState(EUIMapStatus.BigMap);
    }


    [Space(30), Header("手机信息 天气")]
    public Button m_BtnWeather = null;


    /// <summary>
    /// hero 标识
    /// </summary>
    private RectTransform m_HeroTarget;
    private RectTransform m_HeroDirection;
    private void Start()
    {
        
    }

    private void UpdateHeroPos()
    {

    }
}
