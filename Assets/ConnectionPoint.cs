using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//连接点的种类：进、出
public enum ConnectionPointType { In, Out }

//连接点，是节点两侧用于连线的接点
public class ConnectionPoint
{
    public Rect rect;   //矩形范围

    public ConnectionPointType type;    //种类：是进还是出

    public MyNode OwnerNode;            //所归属的节点
    public MyNodeEditor OwnerWindow;    //所归属的节点编辑器窗口

    private GUIStyle style; //绘制连接点的风格

    //构造函数
    public ConnectionPoint(MyNode owner, ConnectionPointType type, MyNodeEditor ownerWindow, GUIStyle PointStyle)
    {
        this.OwnerNode = owner;
        this.type = type;
        this.OwnerWindow = ownerWindow;
        this.style = PointStyle;

        //矩形范围：
        rect = new Rect
            (0,     //X：之后会根据归属节点重新计算
            0,      //Y：之后会根据归属节点重新计算
            16f,    //宽度
            16f);   //高度
    }

    //绘制图形
    public void Draw()
    {
        //连接点的Y值应在所归属的节点的中间：
        rect.y = OwnerNode.rect.y + (OwnerNode.rect.height * 0.5f) - rect.height * 0.5f;

        //连接点的X值根据种类区分：
        switch (type)
        {
            case ConnectionPointType.In:    //对于进的连接点，绘制在节点的左侧：
                rect.x = OwnerNode.rect.x - rect.width;
                break;

            case ConnectionPointType.Out:   //对于出的连接点，绘制在节点的右侧：
                rect.x = OwnerNode.rect.x + OwnerNode.rect.width;
                break;
        }

        //绘制按钮
        if (GUI.Button(rect, "", style))//按钮被按下后：
        {
            //若窗口没有任何选择点，则表示这是选择的第一个点
            if (OwnerWindow.SelectingPoint == null)
                OwnerWindow.SelectingPoint = this;
            else    //否则，窗口中已经选择了一个连接点了
            {
                //若先前所选的连接点的方向和现在的点不一样，则可以创建连接
                if (OwnerWindow.SelectingPoint.type != this.type)
                {
                    //根据自己的类型来决定创建连接时参数的顺序
                    if (this.type == ConnectionPointType.In)
                        OwnerWindow.connections.Add(new Connection(this, OwnerWindow.SelectingPoint, OwnerWindow));
                    else
                        OwnerWindow.connections.Add(new Connection(OwnerWindow.SelectingPoint, this, OwnerWindow));

                    //连接创建结束后将SelectingPoint置为空
                    OwnerWindow.SelectingPoint = null;
                }
            }
        }
    }
}
