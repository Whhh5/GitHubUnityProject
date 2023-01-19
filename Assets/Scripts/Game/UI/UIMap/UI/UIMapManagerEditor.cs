using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIMapManager))]
public class UIMapManagerEditor : Editor
{
    UIMapManager m_Target;
    private void OnEnable()
    {
        m_Target = m_Target == null ? target as UIMapManager : m_Target;
    }

    Vector2 m_Centre;
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Initialization"))
        {
            m_Target.UpdateMapInfo(0);
        }
        if (GUILayout.Button("Create"))
        {
            m_Target.Create();
        }
        if (GUILayout.Button("Update Map Chunk"))
        {
            m_Target.UpdateMapChunk();
        }
        if (GUILayout.Button("Update Scene Info"))
        {
            m_Target.UpdateViewInfo();
        }
        m_Centre = EditorGUILayout.Vector2Field("Centre", m_Centre);
        if (GUILayout.Button("Move Map Centre To"))
        {
            m_Target.MoveMapCentreToAnchored2D(m_Centre);
        }
        base.OnInspectorGUI();
    }
}
