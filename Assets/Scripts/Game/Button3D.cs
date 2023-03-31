
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button3D : MonoBehaviour
{
    [SerializeField]
    Vector3 m_OriginScale = Vector3.one;
    [SerializeField]
    Vector3 m_ToScale = new Vector3(1.2f, 1.5f, 1.2f);

    Action m_OnClick = null;
    Action m_OnClick2 = null;

    float m_ClickTime = 0.0f;
    float m_Click2Interval = 1;
    private void OnMouseEnter()
    {
        DOTween.Kill(EUIMapDGIDType.Button3DOnMouseEnter);
        var curSize = transform.localScale;
        var moveTime = Vector3.Distance(curSize, m_ToScale);
        DOTween.To(() => 0.0f, (value) =>
              {
                  transform.localScale = Vector3.Lerp(curSize, m_ToScale, value);

              }, 1.0f, moveTime)
                .SetId(EUIMapDGIDType.Button3DOnMouseEnter);
    }
    private void OnMouseExit()
    {
        DOTween.Kill(EUIMapDGIDType.Button3DOnMouseEnter);
        var curSize = transform.localScale;
        var moveTime = Vector3.Distance(curSize, m_OriginScale);
        DOTween.To(() => 0.0f, (value) =>
        {
            transform.localScale = Vector3.Lerp(curSize, m_OriginScale, value);

        }, 1.0f, moveTime)
                .SetId(EUIMapDGIDType.Button3DOnMouseEnter);
    }
    private void OnMouseUpAsButton()
    {
        var curTime = Time.deltaTime;
        if (Mathf.Abs(curTime - m_ClickTime) > m_Click2Interval)
        {
            m_OnClick?.Invoke();
            m_Click2Interval = Time.deltaTime;
            Debug.Log("1");

        }
        else
        {
            m_OnClick2?.Invoke();
            m_Click2Interval = 0;
            Debug.Log("2");
        }



    }

    public void AddListener(Action f_Func)
    {
        var list = m_OnClick.GetInvocationList();
        if (Array.IndexOf(list, f_Func) > -1)
        {
            return;
        }
        else
        {
            m_OnClick += f_Func;
        }
    }
    public void RemoveListener(Action f_Func)
    {
        var list = m_OnClick.GetInvocationList();
        if (Array.IndexOf(list, f_Func) > -1)
        {
            m_OnClick -= f_Func;
        }
        else
        {
            return;
        }
    }
    public void AddListener2(Action f_Func)
    {
        var list = m_OnClick2.GetInvocationList();
        if (Array.IndexOf(list, f_Func) > -1)
        {
            return;
        }
        else
        {
            m_OnClick2 += f_Func;
        }
    }
    public void RemoveListener2(Action f_Func)
    {
        var list = m_OnClick2.GetInvocationList();
        if (Array.IndexOf(list, f_Func) > -1)
        {
            m_OnClick2 -= f_Func;
        }
        else
        {
            return;
        }
    }
}
