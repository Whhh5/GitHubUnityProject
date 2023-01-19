using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;

[CustomEditor(typeof(UIAlphaMask))]
public class UIAlphaMaskEditor : Editor
{
    private UIAlphaMask originalTarget = null;
    private void OnEnable()
    {
        if (originalTarget == null) originalTarget = target as UIAlphaMask;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create"))
        {
            var texture2d = originalTarget?.Create();
            if (texture2d != null && originalTarget.m_IsCacheImg)
            {
                var txtPngByte = texture2d.EncodeToPNG();
                var direction = $"{Application.dataPath}/{originalTarget.m_CachePath}";
                var fileName = $"{originalTarget.m_NamePrefix}{originalTarget.m_CacheImageName}.png";
                var assetPath = $"Assets/{originalTarget.m_CachePath}/{fileName}";
                var path = $"{direction}/{fileName}";

                if (originalTarget.m_NewObjIsDesOldObj)
                {

                }


                if (!Directory.Exists(direction))
                {
                    Directory.CreateDirectory(direction);
                }

                var indexImg = 0;
                while (File.Exists(path))
                {
                    if (originalTarget.m_NewObjIsDesOldObj)
                    {
                        File.Delete(path);
                    }
                    else
                    {
                        fileName = $"{originalTarget.m_NamePrefix}{originalTarget.m_CacheImageName}_{indexImg}.png";
                        assetPath = $"Assets/{originalTarget.m_CachePath}/{fileName}";
                        path = $"{direction}/{fileName}";
                    }
                    indexImg++;
                }
                var fileStream = File.Create(path);
                fileStream.Write(txtPngByte);
                fileStream.Close();

                //List<string> assetPaths = new List<string>();
                //assetPaths.Add(assetPath);
                AssetDatabase.Refresh(ImportAssetOptions.Default);

                var loadAsset = AssetImporter.GetAtPath(assetPath);
                if(loadAsset != null)
                {
                    TextureImporter texImporter = loadAsset as TextureImporter;
                    texImporter.textureType = TextureImporterType.Sprite; 
                    texImporter.isReadable = true;
                    texImporter.SaveAndReimport();
                    AssetDatabase.ImportAsset(assetPath);

                    var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    originalTarget.SetCreateObjSprite(sprite);
                }
                else
                {
                    Debug.Log($"��Դδ�ҵ�  assetPath: {assetPath}");
                }
            }
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Show All"))
        {
            originalTarget?.SetActiveAll(true);
        }
        if (GUILayout.Button("Hide All"))
        {
            originalTarget?.SetActiveAll(false);
        }
        EditorGUILayout.EndHorizontal();

        originalTarget.m_IsCacheImg = EditorGUILayout.Toggle("Is Cache Image", originalTarget.m_IsCacheImg);
        if (originalTarget.m_IsCacheImg)
        {
            EditorGUILayout.BeginHorizontal();
            originalTarget.m_CachePath = EditorGUILayout.TextField("Cache Path", originalTarget.m_CachePath);
            if (GUILayout.Button("Reset Path"))
            {
                originalTarget.ResetCcachePath();
            }
            EditorGUILayout.EndHorizontal();
            originalTarget.m_CacheImageName = EditorGUILayout.TextField("Cache Name", originalTarget.m_CacheImageName);

        }
        originalTarget.m_NewObjIsDesOldObj = EditorGUILayout.Toggle("Create Obj Is Des Old Obj", originalTarget.m_NewObjIsDesOldObj);
        if(GUILayout.Button("Clear All Create Object"))
        {
            originalTarget.ClearAllCreateObject();
        }
    }
    public void Refresh()
    {

    }
}
