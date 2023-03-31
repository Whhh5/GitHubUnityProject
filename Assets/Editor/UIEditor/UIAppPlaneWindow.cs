using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using B1.UI;
using System.Reflection;
using UnityEditor.SceneManagement;
using System;
using B1;

public class UIAppPlaneWindow : EditorWindow
{
    UIAppPlane m_Target = null;

    public void SetParameters(UIAppPlane f_UIAppPlane)
    {
        m_Target = f_UIAppPlane;
    }

    Vector2 m_ScrollPos;
    string m_NameKey = "";
    private void OnGUI()
    {
        if (m_Target == null) return;

        int line = 0;
        int elementHeight = 20;
        int elementWidth = 200;
        int lineMaxCount = ((int)position.size.y - 100) / elementHeight;
        if (lineMaxCount <= 0) return;

        List <GUILayoutOption> layoutOptions = new()
        {
            GUILayout.Height(elementHeight),
            GUILayout.Width(elementWidth),
        };


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"{position.size}");
        GUILayout.Label($"{line}");
        GUILayout.Label($"{lineMaxCount}");
        GUILayout.Label($"{m_ScrollPos}");
        if (GUILayout.Button("Clear"))
        {
            m_Target.Clear();
        }
        #region 搜索筛选
        GUILayout.Label("搜索 :",
                        new GUILayoutOption[]
                        {
                        GUILayout.Width(50),
                        });
        m_NameKey = EditorGUILayout.TextField(m_NameKey,
                        new GUILayoutOption[]
                        {
                        GUILayout.Width(300),
                        });
        #endregion
        EditorGUILayout.EndHorizontal();

        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
        EditorGUILayout.BeginHorizontal();
        var maxNum = (int)EUIWindowPage.EnumCount;
        for (int i = 0; i < maxNum; i += lineMaxCount)
        {
            EditorGUILayout.BeginVertical();
            var count = i + lineMaxCount > maxNum ? maxNum - i : lineMaxCount;
            for (int j = 0; j < count; j++)
            {
                var key = (EUIWindowPage)(i + j);

                #region 搜索筛选
                if (!key.ToString().Contains(m_NameKey, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
                #endregion

                m_Target.Add(key);
                EditorGUILayout.BeginHorizontal();
                if (m_Target.TryGetValue(key, out var value))
                {
                    m_Target.SetValue(key, EditorGUILayout.Toggle(value,
                        new GUILayoutOption[]
                        {
                        GUILayout.Height(elementHeight),
                        GUILayout.Width(20),
                        }));
                }
                EditorGUILayout.SelectableLabel($"{key}", layoutOptions.ToArray());
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }

    private void OnEnable()
    {
        Debug.Log($"OnEnable   {m_Target != null}");
    }
    private void OnDisable()
    {
        EditorUtility.SetDirty(m_Target);
        AssetDatabase.SaveAssets();
    }
    private void OnDestroy()
    {
        Debug.Log($"OnDestroy   {m_Target != null}");
    }
}
