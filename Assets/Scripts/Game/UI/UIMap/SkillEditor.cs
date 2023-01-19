using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Skill))]
public class SkillEditor : Editor
{
    private Skill m_Target;
    void OnEnable()
    {
        if (m_Target == null) m_Target = target as Skill;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Play"))
        {
            m_Target.Action();
        }

        if (GUILayout.Button("Stop"))
        {
            m_Target.Stop();
        }
        EditorGUILayout.EndHorizontal();


        if (GUILayout.Button("Clear"))
        {
            m_Target.Clear();
        }


    }
}
