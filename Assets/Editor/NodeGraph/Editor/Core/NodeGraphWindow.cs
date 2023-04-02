using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph
{
    public class NodeGraphWindow : EditorWindow
    {
        private static NodeGraphWindow m_Instance = null;
        private static readonly Rect rect = new Rect(200, 150, 800, 600);
        private static void ShowWindow(string title)
        {
            if (m_Instance == null)
            {
                m_Instance = GetWindow<NodeGraphWindow>(title, true);
                m_Instance.position = rect;
            }
            else
            {
                m_Instance.titleContent = new GUIContent(title);
                m_Instance.rootVisualElement.Clear();
                m_Instance.CreateGUI();
                m_Instance.Show();
            }
        }
        
        public static void OpenNodeGraphWindow(string title, NodeGraph data)
        {
            ShowWindow(title);
            m_Instance.m_data = data;
            NodeGraphSaveTools.Load(m_Instance, data);
        }

        private NodeGraphView m_NodeGraphView = null;
        public NodeGraphView NodeGraphView => m_NodeGraphView;
        
        private NodeGraph m_data = null;
        public NodeGraph Data => m_data;

        private void CreateGUI()
        {
            CreateView();
            CreateToolBar();
        }

        private void CreateToolBar()
        {
            var _toolBar = new Toolbar();
            _toolBar.Add(new Button(() =>
            {
                NodeGraphSaveTools.Save(m_Instance);
            }) { text = "Save Data" });
            rootVisualElement.Add(_toolBar);
        }

        private void CreateView()
        {
            var styleSheet = EditorGUIUtility.Load("NodeGraphView.uss") as StyleSheet;
            m_NodeGraphView = new NodeGraphView(this, styleSheet);
            m_NodeGraphView.StretchToParentSize();
            rootVisualElement.Add(m_NodeGraphView);
        }
        
        private void OnDisable()
        {
            m_NodeGraphView = null;
            m_data = null;
            m_Instance = null;
        }
    }
}