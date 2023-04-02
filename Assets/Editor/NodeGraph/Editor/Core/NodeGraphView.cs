using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph
{
    public class NodeGraphView : GraphView
    {
        private NodeCreateWindowContent m_NodeCreateWindow;
        public NodeGraphView(EditorWindow window, StyleSheet styleSheet)
        {
            if (styleSheet != null) styleSheets.Add(styleSheet);
            SetupZoom(ContentZoomer.DefaultMinScale,ContentZoomer.DefaultMaxScale);//zoom     
        
            //拖拽背景
            this.AddManipulator(new ContentDragger());
            //拖拽节点
            this.AddManipulator(new SelectionDragger());
            
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());
        
            //背景 这个需要在 uss/styleSheet 中定义GridBackground类来描述类型
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddNodeCreateWindow(window);
        }
        
        private void AddNodeCreateWindow(EditorWindow window)
        {
            m_NodeCreateWindow = ScriptableObject.CreateInstance<NodeCreateWindowContent>();
            m_NodeCreateWindow.Configure(window, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), m_NodeCreateWindow);
        }
    }
}