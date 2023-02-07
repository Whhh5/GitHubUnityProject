using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMapClickManager : MonoBehaviour
{
    private void Awake()
    {
        Instance ??= this;
        OnAwake();
    }
    public static UIMapClickManager Instance = null;


    private Dictionary<ushort, Action<dynamic>> m_Click = new();
    private void OnAwake()
    {

    }

    private void OnDestroy()
    {
        
    }



    public void AddClick(ushort f_ID, Action<dynamic> f_Action)
    {
        if (!m_Click.ContainsKey(f_ID))
        {
            m_Click.Add(f_ID, f_Action);
        }
        else if(m_Click[f_ID] == null)
        {
            m_Click[f_ID] = f_Action;
        }
        else
        {
            m_Click[f_ID] += f_Action;
        }
    }
    public void RemoveClick(ushort f_ID, Action<dynamic> f_Action = null)
    {
        if (f_Action == null)
        {
            m_Click.Remove(f_ID);
            return;
        }

        if (m_Click.TryGetValue(f_ID, out var value) && value != null)
        {
            var list = new List<Delegate>(value.GetInvocationList());
            if (list.Contains(f_Action))
            {
                if (list.Count > 1)
                {
                    m_Click[f_ID] -= f_Action;
                }
                else
                {
                    m_Click.Remove(f_ID);
                }
            }
            else
            {
                Debug.Log($"不存在该 事件   id = {f_ID}");
            }
        }
        else
        {
            Debug.Log($"不存在该 ID   id = {f_ID}");
        }
    }

}
