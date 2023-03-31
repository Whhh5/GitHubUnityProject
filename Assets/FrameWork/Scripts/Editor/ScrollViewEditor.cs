using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using B1.UI;

[CustomEditor(typeof(ScrollView))]
public class ScrollViewEditor : Editor
{
    public ScrollView m_Target = null;
    private void OnEnable()
    {
        m_Target = m_Target == null ? target as ScrollView : m_Target;
    }




    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Init"))
        {
            //m_Target.Scroll.viewport = m_Target.Rect.Find("Viewport") != null ?
            //    m_Target.Rect.Find("Viewport").GetComponent<RectTransform>() :
            //    new GameObject("Viewport").AddComponent<RectTransform>();
            //m_Target.Scroll.content = m_Target.Scroll.viewport.Find("Content") != null ?
            //    m_Target.Rect.Find("Content").GetComponent<RectTransform>() :
            //    new GameObject("Content").AddComponent<RectTransform>();
            //var Item = m_Target.Scroll.content.Find("ListItem") != null ?
            //    m_Target.Rect.Find("ListItem").GetComponent<RectTransform>() :
            //    new GameObject("ListItem").AddComponent<RectTransform>();
            //m_Target.Scroll.viewport.SetParent(m_Target.transform);
            //m_Target.Scroll.content.SetParent(m_Target.Scroll.viewport);
            //m_Target.m_Item.transform.SetParent(m_Target.Scroll.content);
            //m_Target.Scroll.viewport.gameObject.AddComponent<Image>();
            //m_Target.Scroll.viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            //m_Target.m_Item = Item.gameObject.AddComponent<ScrollViewListItem>();
            //m_Target.m_Item.gameObject.AddComponent<Image>();
            //m_Target.Scroll.viewport.NormalFullScene();
            //m_Target.m_Item.transform.Normalize();

            //EditorUtility.SetDirty(m_Target);
            //AssetDatabase.SaveAssets();
        }
        base.OnInspectorGUI();
    }
}
