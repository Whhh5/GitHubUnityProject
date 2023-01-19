using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using B1;
using B1.UI;
using System;

[CustomEditor(typeof(UIWindowManagerTest))]
public class UIWindowManagerTestEditor : Editor
{
    private UIWindowManagerTest m_Target = null;
    private void OnEnable()
    {
        m_Target = m_Target == null ? target as UIWindowManagerTest : m_Target;
    }




    EUIWindowPage m_Page = EUIWindowPage.None;
    UnityEngine.Object m_UIWindow = null;
    public override async void OnInspectorGUI()
    {
        m_Page = (EUIWindowPage)EditorGUILayout.EnumPopup("Page", m_Page);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Open Page"))
        {
            UIWindowManager.Instance.OpenPageAsync(m_Page);
        }
        if (GUILayout.Button("CLose Page"))
        {
            UIWindowManager.Instance.ClosePageAsync();
        }
        EditorGUILayout.EndHorizontal();

        m_UIWindow = EditorGUILayout.ObjectField(m_UIWindow, typeof(UIWindow));
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Show Window") && m_UIWindow != null)
        {
            (m_UIWindow as UIWindow).ShowAsync();
        }
        if (GUILayout.Button("Hide Window") && m_UIWindow != null)
        {
            (m_UIWindow as UIWindow).HideAsync();
        }
        if (GUILayout.Button("Close Window") && m_UIWindow != null)
        {
            var window = m_UIWindow as UIWindow;
            window.OnDestroyAsync();
            GameObject.Destroy(window.gameObject);
        }
        EditorGUILayout.EndHorizontal();


        base.OnInspectorGUI();
    }

}
