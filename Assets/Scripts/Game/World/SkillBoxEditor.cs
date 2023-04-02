using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkillBox))]
public class SkillBoxEditor : Editor
{
    private SkillBox m_Target;
    private void OnEnable()
    {
        m_Target ??= target as SkillBox;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("StartExecute"))
        {
            //m_Target.StartExecute();
        }



        base.OnInspectorGUI();
    }
}
