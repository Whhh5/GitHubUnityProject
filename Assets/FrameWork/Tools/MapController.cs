using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class MapController : MonoBehaviour
{
    /// <summary>
    /// 当前关卡等级
    /// </summary>
    [Range(0, 10)]
    public ushort m_Level;
    /// <summary>
    /// 当前关卡房间数
    /// </summary>
    [Range(1, 10)]
    public ushort m_RoomNum;

    /// <summary>
    /// boss 关卡数量
    /// </summary>
    [Range(1, 5)]
    public ushort m_BossRoomNum;


    /// <summary>
    /// 存储所有的房间
    /// </summary>
    public Dictionary<ushort, RoomBase> m_DicRooms = new();

    #region room target
    public Transform m_RoomTarget;
    public async UniTask CreateRoomTargetAsync(Vector3 f_WorldPosition, Vector3 f_LocalScale, string f_Hint)
    {
        var obj = GameObject.Instantiate(m_RoomTarget, m_RoomTarget.parent);

        obj.localPosition = f_WorldPosition;
        obj.localScale = f_LocalScale / 100;

        obj.Find("Text").GetComponent<TextMeshPro>().text = f_Hint;
    }
    #endregion




    Dictionary<int, WorldPointBase> m_DicWorldPoints = new();
    /// <summary>
    /// 创建房间中心点
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid CreateTargetPoint()
    {
        var rangeLength = new Vector2Int(500, 1000);
        var rangeRoomNum = new Vector2Int(5, 10);
        var roomNum = Random.Range(rangeRoomNum.x, rangeRoomNum.y);

        var position = 0;
        for (int i = 0; i < roomNum; i++)
        {
            var roomLength = new Vector3Int(Random.Range(rangeLength.x, rangeLength.y), 100, Random.Range(rangeLength.x, rangeLength.y));
            position += roomLength.x / 100 / 2 + 1;
            CreateRoomTargetAsync(Vector3.zero + new Vector3Int(position, 0, 0), roomLength, i.ToString());
            position += roomLength.x / 100 / 2 + 1;
        }




    }

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
}


[Serializable]
public class WorldPointBase
{
    [SerializeField]
    private bool m_IsNull = true;
    /// <summary>
    /// 当前点到墙体啊边缘的距离
    /// </summary>
    [SerializeField]
    public Vector4 m_ToEdgeDis;
    [SerializeField]
    private EMapPointType m_PointType = EMapPointType.None;
    public EMapPointType PointType => m_PointType;


    public static EMapPointType operator +(WorldPointBase b1, EMapPointType b2)
    {
        b1.m_PointType |= b2;
        return b1.m_PointType;
    }
    public static EMapPointType operator -(WorldPointBase b1, EMapPointType b2)
    {
        b1.m_PointType &= ~b2;
        return b1.m_PointType;
    }
    public static EMapPointType operator &(WorldPointBase b1, EMapPointType b2)
    {
        return b1.m_PointType & b2;
    }
    public static EMapPointType operator |(WorldPointBase b1, EMapPointType b2)
    {
        return b1.m_PointType | b2;
    }
    public static EMapPointType operator ^(WorldPointBase b1, EMapPointType b2)
    {
        return b1.m_PointType ^ b2;
    }
    public static bool operator ==(WorldPointBase b1, EMapPointType b2)
    {
        return b1.m_PointType == b2;
    }
    public static bool operator !=(WorldPointBase b1, EMapPointType b2)
    {
        return b1.m_PointType != b2;
    }
}