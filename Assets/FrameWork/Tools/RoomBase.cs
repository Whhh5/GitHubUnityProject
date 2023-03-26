using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using B1;

[Serializable]
public class RoomBase : MonoBase
{
    /// <summary>
    /// 当前房间难度等级
    /// </summary>
    public ushort m_Level;

    /// <summary>
    /// 当前房间中心点世界坐标
    /// </summary>
    public Vector2Int m_CentrePos;

    /// <summary>
    /// 当前房间大小
    /// </summary>
    public Vector2Int m_Size;

    /// <summary>
    /// 存储所有的门
    /// key = 当前房间门口在房间中心点的位置
    /// value = 当前房间门位于墙面的百分比 下，左 0    上，右 1
    /// </summary>
    public Dictionary<EDoorDirction, float> m_DicDoors = new();




    /// <summary>
    /// 创建四周墙面
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid CreateWall()
    {

    }

    /// <summary>
    /// 创建地形   悬空的一些地形
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid CreateTerrain()
    {

    }

    /// <summary>
    /// 创建辅助攀爬的台阶类工具    弹跳装置 
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid CreateStep()
    {

    }

    /// <summary>
    /// 创建不可破坏的障碍物   墙壁
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid CreateIndestructibleObstacle()
    {

    }

    /// <summary>
    /// 创建可破坏的障碍物  木桶 箱子
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid CreateDestructibleObstacle()
    {

    }

    /// <summary>
    /// 创建效果类道具  一些增益 buff
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid CreateEffectProperty()
    {

    }

    /// <summary>
    /// 创建功能行道具  传送门之类的
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid CreateFunctionProperty()
    {

    }

    public async UniTask CreateEnemy()
    {

    }
}
