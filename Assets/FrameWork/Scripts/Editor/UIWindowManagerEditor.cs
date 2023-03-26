using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace B1.UI
{
    [CustomEditor(typeof(UIWindowManager))]
    public class UIWindowManagerEditor : Editor
    {
        UIWindowManager m_Target = null;
        private void OnEnable()
        {
            m_Target ??= target as UIWindowManager;
        }
        public override void OnInspectorGUI()
        {
            var m_PageStack = m_Target.GetType().GetField("m_PageStack", BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance);
            var value = m_PageStack.GetValue(m_Target);
            var pageList = value as DicStack<Type, UIWindowPage>;
            foreach (var item in pageList)
            {
                EditorGUILayout.LabelField($"key: {item.Key} value: {item.Value}");
            }

            base.OnInspectorGUI();
        }
    }
}