using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph
{
    public class NodeCreateWindowContent : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow m_Window;
        private NodeGraphView m_NodeGraphView;

        private Texture2D _indentationIcon;
        private Type[] nodeTypes;
        
        public void Configure(EditorWindow window, NodeGraphView graphView)
        {
            m_Window = window;
            m_NodeGraphView = graphView;

            var editorAssembly = typeof(NodeBase<,>).Assembly;
            editorAssembly.GetTypes();
            
            nodeTypes = editorAssembly.GetTypes()
                .Where(a => a.GetInterfaces().Contains(typeof(INode)))
                .ToArray();
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
                new SearchTreeGroupEntry(new GUIContent("Nodes"), 1),
            };

            if (nodeTypes != null && nodeTypes.Length > 0)
            {
                for (int i = 0; i < nodeTypes.Length; i++)
                {
                    var entry = new SearchTreeEntry(new GUIContent(nodeTypes[i].FullName))
                    {
                        level = 2, userData = nodeTypes[i].FullName
                    };
                    tree.Add(entry);
                }
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            //Editor window-based mouse position
            var mousePosition = m_Window.rootVisualElement.ChangeCoordinatesTo(m_Window.rootVisualElement.parent,
                context.screenMousePosition - m_Window.position.position);
            var graphMousePosition = m_NodeGraphView.contentViewContainer.WorldToLocal(mousePosition);
            
            var editorAssembly = typeof(NodeBase<,>).Assembly;
            var typeName = (string) SearchTreeEntry.userData;
            var newNode = editorAssembly.CreateInstance(typeName);
            var nodeType = newNode.GetType();
            var onCreated = nodeType.GetMethod("OnCreated");
            onCreated?.Invoke(newNode, new[] {m_NodeGraphView});
            var node = newNode as Node;
            node.SetPosition(new Rect(graphMousePosition, node.GetPosition().size));
            m_NodeGraphView.AddElement(node);
            
            return true;
        }
    }
}