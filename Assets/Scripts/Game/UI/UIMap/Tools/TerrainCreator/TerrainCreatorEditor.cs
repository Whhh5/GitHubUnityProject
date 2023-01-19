using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(TerrainCreator))]
public class TerrainCreatorEditor : Editor
{
    public TerrainCreator originalTarget = null;
    private void OnEnable()
    {
        if (originalTarget == null) originalTarget = target as TerrainCreator;
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        originalTarget.m_Height = EditorGUILayout.FloatField("Terrain Max Height", originalTarget.m_Height);

        EditorGUILayout.BeginHorizontal();
        GUILayout.TextField("Density X");
        originalTarget.m_DensityX = EditorGUILayout.Slider(originalTarget.m_DensityX, 0, 1);
        GUILayout.TextField("Density Y");
        originalTarget.m_DensityY = EditorGUILayout.Slider(originalTarget.m_DensityY, 0, 1);
        originalTarget.m_Random = EditorGUILayout.Slider(originalTarget.m_Random, -20, 20);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create Terrain Vertices And Normal")) 
        {
            originalTarget.PerlinNoise();
        }
        if (GUILayout.Button("Update Weight"))
        {
            originalTarget.UpdateWeight();
        }
        if (GUILayout.Button("Save To Sprite"))
        {
            originalTarget.SaveToSprite();
        }

        originalTarget.chunkX = EditorGUILayout.IntField("ChunkX", originalTarget.chunkX);
        originalTarget.chunkY = EditorGUILayout.IntField("ChunkY", originalTarget.chunkY);
        originalTarget.subVerNum = EditorGUILayout.IntField("SubVerNum", originalTarget.subVerNum);
        originalTarget.target = EditorGUILayout.Vector3Field("SubVerNum", originalTarget.target);

        //EditorGUILayout.BeginHorizontal();
        //Vector2 m_Target = originalTarget.target;
        //m_Target.x = EditorGUILayout.Slider(m_Target.x, 0, 10.0f);
        //m_Target.y = EditorGUILayout.Slider(m_Target.y, 0, 10.0f);
        //originalTarget.target = m_Target;
        //EditorGUILayout.EndHorizontal();
    }
}
