using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIMapPoolManager))]
public class UIMapPoolManagerEditor : Editor
{
    UIMapPoolManager m_Target = null;
    private void OnEnable()
    {
        m_Target ??= target as UIMapPoolManager;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Debug Pool"))
        {
            var window = EditorWindow.GetWindow<UIMapPoolManagerWindow>();
            window.Show(m_Target);
        }
        base.OnInspectorGUI();
    }
}

public class UIMapPoolManagerWindow : EditorWindow
{
    UIMapPoolManager m_Target = null;

    public void Show(UIMapPoolManager f_Target)
    {
        m_Target = f_Target;
        Show(true);
    }


    private Vector2 m_Scroll;
    private void OnGUI()
    {
        if (m_Target == null) return;

        m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll, GUILayout.Width(position.width), GUILayout.Height(position.height));



        GUILayout.Label("m_FlagType_Instance");
        EditorGUILayout.BeginHorizontal();
        foreach (var item in m_Target.m_AssetsType_Instance)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField($"key = {item.Key}, count = {item.Value.Count}, ", GUILayout.Width(300));

            GUILayout.Space(20);

            int index = 0;
            EditorGUILayout.BeginVertical();
            foreach (var item2 in item.Value)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"[ {index++} ] = ", GUILayout.Width(50));
                EditorGUILayout.ObjectField(item2, typeof(GameObject));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
            GUILayout.Space(50);
        }
        EditorGUILayout.EndHorizontal();





        EditorGUILayout.EndScrollView();
    }
}
