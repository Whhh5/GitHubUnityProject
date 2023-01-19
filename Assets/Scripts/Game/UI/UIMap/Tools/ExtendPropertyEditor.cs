using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ExtendProperty))]
public class ExtendPropertyEditor : Editor
{
    private ExtendProperty m_Target = null;
    private void OnEnable()
    {
        m_Target = m_Target == null ? target as ExtendProperty : m_Target;
    }
    public override void OnInspectorGUI()
    {
        GUILayout.Label($"World Position : {m_Target.transform.position}");
    }
}