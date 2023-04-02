using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MyNodeEditor : EditorWindow
{
    private List<MyNode> nodes;             //节点列表
    public List<Connection> connections;    //连接列表

    public ConnectionPoint SelectingPoint;  //记录正在选择的点，用于判断之后是否会发生连接
    public bool isRemoveConnectionMode;     //标志着是否进入移除连线模式

    private Vector2 GridOffset;         //画布网格的偏移

    //记录一些GUIStyle
    GUIStyle style_Node;            //节点的GUI风格
    GUIStyle style_Node_select;     //节点在选择情况下的GUI风格
    GUIStyle style_Point;           //连接点的GUI风格

    //在Window菜单下添加一栏 My Node Editor
    [MenuItem("Window/My Node Editor")]
    private static void OpenWindow()
    {
        //获得一个已存在的MyNodeEditor窗口，若没有则创建一个新的：
        MyNodeEditor window = GetWindow<MyNodeEditor>();
    }

    //EditorWindow的接口OnEnable：当对象加载时调用此函数
    private void OnEnable()
    {
        //设置窗口的标题：
        titleContent = new GUIContent("My Node Editor");

        //创建节点列表对象
        nodes = new List<MyNode>();
        //创建连接列表对象
        connections = new List<Connection>();

        //临时加些测试：
        /*{
            //临时加一个测试用节点
            MyNode n1 = new MyNode(new Vector2(0, 0));
            nodes.Add(n1);
            //临时加第二个测试用节点
            MyNode n2 = new MyNode(new Vector2(200, 200));
            nodes.Add(n2);

            //临时加一个测试的连线
            connections.Add(new Connection(n2.inPoint, n1.outPoint));
        }*/

        //初始化一些GUIStyle
        {
            //节点的风格：
            style_Node = new GUIStyle();
            //normal：	    正常显示组件时的渲染设置
            style_Node.normal.background = EditorGUIUtility.Load("Assets/node.png") as Texture2D;
            //alignment：	文本对齐方式
            style_Node.alignment = TextAnchor.MiddleCenter;//中心

            //节点在选择状态下的风格：
            style_Node_select = new GUIStyle();
            //normal：	    正常显示组件时的渲染设置
            style_Node_select.normal.background = EditorGUIUtility.Load("Assets/node_select.png") as Texture2D;
            //alignment：	文本对齐方式
            style_Node_select.alignment = TextAnchor.MiddleCenter;//中心
            //fontStyle：   要使用的字体样式
            style_Node_select.fontStyle = FontStyle.Bold;//粗体

            //连接点的风格：
            style_Point = new GUIStyle();
            //normal：       正常显示组件时的渲染设置
            style_Point.normal.background = EditorGUIUtility.Load("Assets/point_normal.png") as Texture2D;
            //active：       按下控件时的渲染设置。
            style_Point.active.background = EditorGUIUtility.Load("Assets/point_active.png") as Texture2D;
            //hover：        鼠标悬停在控件上时的渲染设置。
            style_Point.hover.background = EditorGUIUtility.Load("Assets/point_hover.png") as Texture2D;
        }
    }

    //EditorWindow的接口OnGUI：绘制控件调用的接口
    private void OnGUI()
    {
        //绘制背景画布网格
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        //绘制节点和连线
        DrawNodes();
        DrawConnections();

        //处理事件
        ProcessNodeEvents(Event.current);   //先处理节点的
        ProcessEvents(Event.current);       //再处理自身的

        //绘制待连接线
        DrawPendingConnection(Event.current);

        if (GUI.changed)  //若GUI发生变化则重新绘制  
            Repaint();
    }

    //绘制所有节点
    private void DrawNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
            nodes[i].Draw();
    }
    //绘制所有的连线
    private void DrawConnections()
    {
        for (int i = 0; i < connections.Count; i++)
            connections[i].Draw();
    }

    //绘制待连接线
    private void DrawPendingConnection(Event e)
    {
        if (SelectingPoint != null)//如果已经选择了一个连接点，则画出待连接的线
        {
            //贝塞尔曲线的起点，根据已选则点的方向做判断：
            Vector3 startPosition = (SelectingPoint.type == ConnectionPointType.In) ? SelectingPoint.rect.center : e.mousePosition;
            Vector3 endPosition = (SelectingPoint.type == ConnectionPointType.In) ? e.mousePosition : SelectingPoint.rect.center;

            Handles.DrawBezier(     //绘制通过给定切线的起点和终点的纹理化贝塞尔曲线
            startPosition,
            endPosition,
            startPosition + Vector3.left * 50f, //startTangent	贝塞尔曲线的起始切线。
            endPosition - Vector3.left * 50f,   //endTangent	贝塞尔曲线的终点切线。
            Color.white,        //color	    要用于贝塞尔曲线的颜色。
            null,               //texture	要用于绘制贝塞尔曲线的纹理。
            2f                  //width	    贝塞尔曲线的宽度。
            );

            GUI.changed = true;
        }
    }

    //绘制画布网格
    //gridSpacing：  格子间距
    //gridOpacity：  网格线不透明度
    //gridColor：    网格线颜色
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        //宽度分段
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        //高度分段
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();//在 3D Handle GUI 内开始一个 2D GUI 块。
        {
            //设置颜色：
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            //单格的偏移，算是GridOffset的除余
            Vector3 gridOffset = new Vector3(GridOffset.x % gridSpacing, GridOffset.y % gridSpacing, 0);

            //绘制所有的竖线
            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(
                    new Vector3(gridSpacing * i, 0 - gridSpacing, 0) + gridOffset,                  //起点
                    new Vector3(gridSpacing * i, position.height + gridSpacing, 0f) + gridOffset);  //终点
            }
            //绘制所有的横线
            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(
                    new Vector3(0 - gridSpacing, gridSpacing * j, 0) + gridOffset,                  //起点
                    new Vector3(position.width + gridSpacing, gridSpacing * j, 0f) + gridOffset);   //终点
            }

            //重设颜色
            Handles.color = Color.white;
        }
        Handles.EndGUI(); //结束一个 2D GUI 块并返回到 3D Handle GUI。
    }

    //处理事件
    private void ProcessEvents(Event e)
    {
        switch (e.type)//根据事件类型做判断
        {
            case EventType.MouseDown:   //按下鼠标键
                if (e.button == 1)      //鼠标右键
                {
                    //触发菜单
                    RightMouseMenu(e.mousePosition);
                }
                if (e.button == 0)  //按下鼠标左键
                {
                    SelectingPoint = null;//清空当前所选的连接点
                }
                break;
            case EventType.KeyDown://按下键盘
                if (e.keyCode == KeyCode.Y)//是Y键
                {
                    isRemoveConnectionMode = true;  //进入移除连线模式
                    GUI.changed = true;             //提示需要刷新GUI
                }
                break;
            case EventType.KeyUp://松开键盘
                if (e.keyCode == KeyCode.Y)//是Y键
                {
                    isRemoveConnectionMode = false; //离开移除连线模式
                    GUI.changed = true;             //提示需要刷新GUI
                }
                break;
            case EventType.MouseDrag:   //鼠标拖拽
                if (e.button == 0)       //鼠标左键
                {
                    DragAllNodes(e.delta);  //拖拽所有节点（拖拽画布）
                    GridOffset += e.delta;  //增加画布网格的偏移
                    GUI.changed = true;     //提示需要刷新GUI
                }
                break;
        }
    }

    //处理所有节点的事件
    private void ProcessNodeEvents(Event e)
    {
        //降序处理所有节点的事件（之所以降序是因为后画的节点将显示在更上层）
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            //处理每个节点的事件并看是否发生了拖拽
            bool DragHappend = nodes[i].ProcessEvents(e);
            //若发生了拖拽则提示GUI发生变化
            if (DragHappend)
                GUI.changed = true;
        }
    }

    //鼠标右键菜单：
    private void RightMouseMenu(Vector2 mousePosition)
    {
        //创建菜单对象
        GenericMenu genericMenu = new GenericMenu();
        //菜单加一项 Add node
        genericMenu.AddItem(new GUIContent("Add node"), false, () => ProcessAddNode(mousePosition));
        //显示菜单
        genericMenu.ShowAsContext();
    }
    //处理添加节点
    private void ProcessAddNode(Vector2 nodePosition)
    {
        nodes.Add(new MyNode(nodePosition, this, style_Node, style_Node_select, style_Point));
    }
    //处理移除节点
    public void ProcessRemoveNode(MyNode node)
    {
        //收集“待删除连接列表”
        List<Connection> connectionsToRemove = new List<Connection>();

        //遍历所有的连接，若连接的入点或出点是属于要删除的节点的，则将其添加到“待删除连接列表”中
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                connectionsToRemove.Add(connections[i]);
        }

        //删除“待删除连接列表”中所有连接
        for (int i = 0; i < connectionsToRemove.Count; i++)
            connections.Remove(connectionsToRemove[i]);

        connectionsToRemove = null;

        //移除节点
        nodes.Remove(node);
    }

    //拖拽所有节点（拖拽画布）
    private void DragAllNodes(Vector2 delta)
    {
        for (int i = 0; i < nodes.Count; i++)
            nodes[i].ProcessDrag(delta);
    }
}
