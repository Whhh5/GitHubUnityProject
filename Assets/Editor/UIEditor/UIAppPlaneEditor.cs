using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using B1.UI;

[CustomEditor(typeof(UIAppPlane))]
public class UIAppPlaneEditor : Editor
{
    UIAppPlane m_Target = null;
    private void OnEnable()
    {
        m_Target = m_Target == null ? target as UIAppPlane : m_Target;
    }




    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Setting"))
        {
            var window = EditorWindow.GetWindow<UIAppPlaneWindow>();
            Debug.Log(m_Target != null);
            window.SetParameters(m_Target);
            window.Show();
        }


        base.OnInspectorGUI();
    }
}
