using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIMapAssetsManager))]
public class UIMapAssetsManagerEditor : Editor
{
    UIMapAssetsManager m_Target = null;

    private void OnEnable()
    {
        m_Target ??= target as UIMapAssetsManager;
    }
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Console Plane"))
        {
            var window = EditorWindow.CreateInstance<UIMapAssetsManagerWindow>();
            window.Show();
            window.SetInputParameter(m_Target);
        }


        base.OnInspectorGUI();
    }
}



public class UIMapAssetsManagerWindow : EditorWindow
{
    UIMapAssetsManager m_Target = null;
    public void SetInputParameter(UIMapAssetsManager f_Target)
    {
        m_Target = f_Target;
    }


    Vector2 m_ScrollV2 = Vector2.zero;
    private void OnGUI()
    {
        if (m_Target == null) return;


        m_ScrollV2 = EditorGUILayout.BeginScrollView(m_ScrollV2, GUILayout.Width(position.width), GUILayout.Height(position.height));

        EditorGUILayout.BeginVertical();
        int unitlWidth = 300;
        int maxHorNum = (int)(position.width / unitlWidth);
        int index = 0;

        Dictionary<int, List<Action>> dic = new();
        foreach (var item in m_Target.m_SpriteAssets)
        {
            var data = item;
            var key = index++ / maxHorNum;
            if (!dic.ContainsKey(key)) dic.Add(key, new());

            dic[key].Add(() =>
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField($"Assets Path: {data.Key}");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Sprite Asset: ", GUILayout.Width(70));
                EditorGUILayout.ObjectField(data.Value.sprite, typeof(Sprite));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField($"Reference List: ", GUILayout.Width(70));
                foreach (var element in data.Value.list)
                {
                    EditorGUILayout.LabelField($"       {element}");
                }
                EditorGUILayout.EndVertical();
            });
        }

        foreach (var item in dic)
        {
            EditorGUILayout.BeginHorizontal();

            foreach (var element in item.Value)
            {
                element.Invoke();
                EditorGUILayout.Space(20);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20);
        }
        EditorGUILayout.EndVertical();





        EditorGUILayout.EndScrollView();
    }
}