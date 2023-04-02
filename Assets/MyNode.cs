using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MyNode
{
    public Rect rect;   //节点的矩形范围

    public MyNodeEditor OwnerWindow;    //所归属的节点编辑器窗口

    private bool isDragged; //标志着是否在拖拽状态
    private bool isSelected;//标志着是否正在被选择

    public ConnectionPoint inPoint;     //连接点：进
    public ConnectionPoint outPoint;    //连接点：出

    private GUIStyle style;         //节点正常情况下的风格
    private GUIStyle style_select;  //节点被选择下的风格

    //构造函数
    public MyNode(Vector2 position, MyNodeEditor owner, GUIStyle NodeStype, GUIStyle NodeSelectStype, GUIStyle PointStyle)
    {
        //自身矩形范围
        rect = new Rect(position.x, position.y, 160, 40);

        OwnerWindow = owner;

        style = NodeStype;
        style_select = NodeSelectStype;

        //创建进和出的连接点
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, owner, PointStyle);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, owner, PointStyle);
    }

    //绘制自身的图形
    public void Draw()
    {
        //绘制自身：
        GUI.Box(rect, "MyNode", isSelected ? style_select : style);
        //绘制两个连接点：
        inPoint.Draw();
        outPoint.Draw();
    }

    //此节点处理事件，返回是否发生拖拽
    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            //鼠标按下：
            case EventType.MouseDown:
                if (e.button == 0)//按下左键
                {
                    if (rect.Contains(e.mousePosition)) //是否按在了节点范围内
                    {
                        isDragged = true;   //标志着进入拖拽状态
                        isSelected = true;
                    }
                    else
                    {
                        isSelected = false;
                    }

                    GUI.changed = true; //提示GUI变化
                }
                if (e.button == 1)//按下右键
                {
                    //被选择且鼠标在节点范围内
                    if (isSelected && rect.Contains(e.mousePosition))
                    {
                        RightMouseMenu();
                        e.Use();
                    }

                }
                break;
            //鼠标松开：
            case EventType.MouseUp:
                isDragged = false;          //标志着离开拖拽状态
                break;
            //鼠标拖拽：
            case EventType.MouseDrag:
                if (e.button == 0 && isDragged) //是否按下鼠标右键且在拖拽状态
                {
                    ProcessDrag(e.delta);   //处理拖拽
                    e.Use();                //标志着这个事件已经被处理过了，其他GUI元素之后将忽略它
                    return true;            //返回true，表示发生了拖拽
                }
                break;
        }
        return false;   //如果最后没有任何拖拽发生，则返回false
    }

    //处理拖拽
    public void ProcessDrag(Vector2 delta)
    {
        //使自身位置增加偏移
        rect.position += delta;
    }

    //鼠标右键菜单：
    private void RightMouseMenu()
    {
        //创建菜单对象
        GenericMenu genericMenu = new GenericMenu();
        //菜单加一项 Remove node，它将调用节点窗口的ProcessRemoveNode函数
        genericMenu.AddItem(new GUIContent("Remove node"), false, () => OwnerWindow.ProcessRemoveNode(this));
        //显示菜单
        genericMenu.ShowAsContext();
    }
}
