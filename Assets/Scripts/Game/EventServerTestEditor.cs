using System.Collections;
using System.Collections.Generic;
using B1.Event;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventServerTest))]
public class EventServerTestEditor : Editor
{
    private EventServerTest m_Target = null;
    private void OnEnable()
    {
        m_Target = m_Target == null ? target as EventServerTest : m_Target;
    }


    string m_Para;
    string m_Des;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Event");
        m_Target.m_Layer = EditorGUILayout.TextField(m_Target.m_Layer);
        m_Target.m_Event = (EEvent)EditorGUILayout.EnumPopup(m_Target.m_Event);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Subscribe Event"))
        {
            EventManager.Instance.Subscribe(EEvent.SCENE_LOAD_START, TestAction, m_Target.m_Layer);
        }
        if (GUILayout.Button("Unsubscribe Event"))
        {
            EventManager.Instance.Unsubscribe(EEvent.SCENE_LOAD_START, TestAction, m_Target.m_Layer);
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Des");
        m_Des = EditorGUILayout.TextField(m_Des);
        GUILayout.Label("Para");
        m_Para = EditorGUILayout.TextField(m_Para);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Fire Event"))
        {
            EventManager.Instance.FireEvent(EEvent.SCENE_LOAD_START, m_Para, m_Des);
        }
        if (GUILayout.Button("Console Event"))
        {
            EventManager.Instance.LogEvent();
        }

        base.OnInspectorGUI();
    }
    void TestAction(EEvent f_EEvent, object f_Parameter, (string layer, string des) des)
    {
        Debug.Log($"触发事件  event name = {f_EEvent}     layer = {des.layer}     des = {des.des}");
    }
}
