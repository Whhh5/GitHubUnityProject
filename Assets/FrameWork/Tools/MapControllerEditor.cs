using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cysharp.Threading.Tasks;

[CustomEditor(typeof(MapController)) ]
public class MapControllerEditor : Editor
{
    MapController m_Target = null;
    private void OnEnable()
    {
        m_Target ??= target as MapController;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Point"))
        {
            UniTask.Void(m_Target.CreateTargetPoint);
        }
        if (GUILayout.Button("Close Point"))
        {
            while (m_Target.m_RoomTarget.parent.childCount > 1)
            {
                GameObject.DestroyImmediate(m_Target.m_RoomTarget.parent.GetChild(1).gameObject);
            }
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Create Wall"))
        {
            UniTask.Void(m_Target.CreateWall);
        }
        if (GUILayout.Button("Create Terrain"))
        {
            UniTask.Void(m_Target.CreateTerrain);
        }
        if (GUILayout.Button("Create Step"))
        {
            UniTask.Void(m_Target.CreateStep);
        }
        if (GUILayout.Button("Create Indestructible Obstacle"))
        {
            UniTask.Void(m_Target.CreateIndestructibleObstacle);
        }
        if (GUILayout.Button("Create Destructible Obstacle"))
        {
            UniTask.Void(m_Target.CreateDestructibleObstacle);
        }
        if (GUILayout.Button("Create Effect Property"))
        {
            UniTask.Void(m_Target.CreateEffectProperty);
        }
        if (GUILayout.Button("Create Function Property"))
        {
            UniTask.Void(m_Target.CreateFunctionProperty);
        }

        base.OnInspectorGUI();
    }
}
