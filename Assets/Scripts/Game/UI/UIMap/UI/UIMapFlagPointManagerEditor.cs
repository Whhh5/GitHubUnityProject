using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIMapFlagPointManager))]
public class UIMapFlagPointManagerEditor : Editor
{
    UIMapFlagPointManager m_Target;
    private void OnEnable()
    {
        m_Target = m_Target == null ? target as UIMapFlagPointManager : m_Target;
    }


    EUIMapFlagPointType m_flagPointType;
    Vector3 m_FlagPointWorldPos;
    Vector2 m_UIMapPos;
    UnityEngine.Object m_FlagPoint;
    int radius = 100;
    public override void OnInspectorGUI()
    {
        m_flagPointType = (EUIMapFlagPointType)EditorGUILayout.EnumPopup("Flag Type", m_flagPointType);
        m_FlagPointWorldPos = EditorGUILayout.Vector3Field("World Pos", m_FlagPointWorldPos);
        if (GUILayout.Button("Create Flag Point"))
        {
            m_Target.LoadFlagPoint(m_flagPointType, m_FlagPointWorldPos, Random.Range(0, 10));
        }
        m_FlagPoint = EditorGUILayout.ObjectField(m_FlagPoint, typeof(UIMapFlagPoint));
        if (GUILayout.Button("Cahnge Flag Trace") && m_FlagPoint != null)
        {
            m_Target.ChangeTraceStatus((m_FlagPoint as UIMapFlagPoint).m_Base);
        }
        if (GUILayout.Button("Remove Flag Point") && m_FlagPoint != null)
        {
            m_Target.UnLoadFlagPoint(m_FlagPoint as UIMapFlagPoint);
        }
        radius = EditorGUILayout.IntField("¼ì²â°ë¾¶", radius);
        if (GUILayout.Button("Remove Flag Point") && m_FlagPoint != null)
        {
            m_Target.GetRangePoint(m_FlagPoint as UIMapFlagPoint, (uint)radius);
            m_Target.m_TargetPoint = (m_FlagPoint as UIMapFlagPoint).transform.position;
            m_Target.m_TargetRadius = radius;
        }
        m_UIMapPos = EditorGUILayout.Vector2Field("World Pos", m_UIMapPos);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Show Flag Point Type"))
        {
            m_Target.ShowFlagPointTypeWindow(m_UIMapPos);
        }
        if (GUILayout.Button("Hide"))
        {
            m_Target.HideFlagPointInfoTypeWindow();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Dictionary Debug Plane"))
        {
            var window = new UIMapFlagPointManagerWindow(m_Target);
            window.Show();
        }


        base.OnInspectorGUI();
    }
}

public class UIMapFlagPointManagerWindow : EditorWindow
{
    public UIMapFlagPointManager m_Target = null;
    public UIMapFlagPointManagerWindow(UIMapFlagPointManager f_Target)
    {
        m_Target = f_Target;
    }

    Vector2 m_Scroll;
    private void OnGUI()
    {
        m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll, GUILayout.Width(position.width), GUILayout.Height(position.height));
        EditorGUILayout.BeginHorizontal();


        EditorGUILayout.BeginVertical();
        GUILayout.Label("m_ListTrace");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        foreach (var item in m_Target.m_ListTrace)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField($"key = {item.Key}, count = {item.Value.count}, ", GUILayout.Width(150));

            int index = 0;

            EditorGUILayout.BeginVertical();
            foreach (var item2 in item.Value.list)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"[ {index++} ] = ", GUILayout.Width(50));
                EditorGUILayout.ObjectField(item2, typeof(UIMapFlagPoint));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(20);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        //foreach (var item in m_Target.m_ListTrace)
        //{
        //    EditorGUILayout.EnumPopup(item.Value.showMap);
        //}
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();


        EditorGUILayout.Space(50);




        EditorGUILayout.BeginVertical();
        GUILayout.Label("m_DicChunk_Flag");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        foreach (var item in m_Target.m_DicChunk_Flag)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField($"key = {item.Key}, state = {item.Value.state}, ", GUILayout.Width(150));
            int index = 0;

            EditorGUILayout.BeginVertical();
            foreach (var item2 in item.Value.list)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"[ {index++} ] = ", GUILayout.Width(50));
                EditorGUILayout.ObjectField(item2.Value, typeof(UIMapFlagPoint));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(20);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        //foreach (var item in m_Target.m_DicChunk_Flag)
        //{
        //    EditorGUILayout.TextField($"state :{item.Value.state}");
        //}
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();





        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }
}