using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ValueToShader))]
public class ValueToShaderEditor : Editor
{
    ValueToShader m_Target = null;

    private void OnEnable()
    {
        m_Target ??= target as ValueToShader;
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Play"))
        {
            m_Target.Play();
        }
        if (GUILayout.Button("Stop"))
        {
            m_Target.Stop();
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Add"))
        {
            m_Target.m_PointList.Add(null);
            m_Target.m_ParamsList.Add(Vector4.zero);
        }
        for (int i = 0; i < m_Target.m_PointList.Count; i++)
        {
            if (i >= m_Target.m_PointList.Count) continue;
            var item = m_Target.m_PointList[i];
            var param = m_Target.m_ParamsList[i];
            EditorGUILayout.BeginHorizontal();


            var obj = EditorGUILayout.ObjectField(item, typeof(RectTransform), GUILayout.Width(100));
            if (obj != null)
            {
                item = obj as RectTransform;
            }
            param.x = EditorGUILayout.FloatField(param.x, GUILayout.Width(50));
            param.y = EditorGUILayout.FloatField(param.y, GUILayout.Width(50));


            m_Target.m_ParamsList[i] = param;
            m_Target.m_PointList[i] = item;
            if (GUILayout.Button("Remove"))
            {
                m_Target.m_PointList.RemoveAt(i);
                m_Target.m_ParamsList.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();

        }

    }
}
