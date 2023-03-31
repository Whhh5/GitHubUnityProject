using System;
using UnityEngine;

#region 枚举类型
public enum EUIMapFlagPointType
{
    None,
    /// <summary>
    /// 普通 NPC
    /// </summary>
    NPC,
    /// <summary>
    /// 怪物
    /// </summary>
    Monster,
    /// <summary>
    /// 传送 NPC
    /// </summary>
    TransferNPC,
    /// <summary>
    /// 采集物
    /// </summary>
    Collection,
    /// <summary>
    /// 传送门
    /// </summary>
    TransferPortal,
    /// <summary>
    /// 马车点
    /// </summary>
    CarriagePoint,
    /// <summary>
    /// 可接任务
    /// </summary>
    AvailableTask,
    /// <summary>
    /// 可交付任务
    /// </summary>
    DeliverableTask,
    /// <summary>
    /// 事件
    /// </summary>
    Incident,
    /// <summary>
    /// 野外锚点
    /// </summary>
    FieldAnchor,
    /// <summary>
    /// 城镇锚点
    /// </summary>
    CitiesAndTownsAnchor,
    /// <summary>
    /// 制造台
    /// </summary>
    ManufacturingStation,
    /// <summary>
    /// 标记点
    /// </summary>
    FlagPoint,
    /// <summary>
    /// 队友
    /// </summary>
    Teammate,
    /// <summary>
    /// 营地
    /// </summary>
    Campsite,
    /// <summary>
    /// 进行中任务
    /// </summary>
    UnderwayTask,
    /// <summary>
    /// 事件范围
    /// </summary>
    IncidentRange,
    /// <summary>
    /// 英雄
    /// </summary>
    Hero,
    /// <summary>
    /// 副本入口
    /// </summary>
    CopyEntrance,
    /// <summary>
    /// BOSS
    /// </summary>
    Boss,
    /// <summary>
    /// 小区域文本
    /// </summary>
    MinAreaText,
    /// <summary>
    /// 大区域文本
    /// </summary>
    MaxAreaText,
    /// <summary>
    /// 城镇板子
    /// </summary>
    TownBoard,
    /// <summary>
    /// 类型总数
    /// </summary>
    EnumCount,
}

public enum EUIMapTracePointType
{
    None,
    Type1,
    Type2,
    Type3,
    EnumCount,
}

public enum EUIMapFlagPointInfoType
{
    None,
    Type1,
    Type2,
    Type3,
    Type4,
    Type5,
    Type6,
    Type7,
    Type8,
    Type9,
    Type10,
    Type11,
    Type12,
    Type13,
    Type14,
    EnumCount,
}
public enum EUIMapFlagMoveType
{
    Static,
    Move,
    EnumCount,
}

public enum EUIMapStatus
{
    BigMap = 1 << 0,
    MiniMap = 1 << 1,
    All = int.MaxValue,
}

public enum EUIMapAssetType : ushort
{
    None,
    UIMapChunk,
    UIMapFlagPoint,
    EnumCount,
}


public enum EUIMapDGIDType
{
    SetUIMapState,
    MoveMapCentreToAnchored2D,
    ShowFlagPointInfoTypeWindow,
    ChangMapScaleTo,
    UpdateFlagStateByUIMapSatus,
    Button3DOnMouseEnter,
    EnumCount,
}

public enum EUIMapClick : ushort
{
    ChangeMapStatus,
    ChangeMapChunkStatus,
    EnumCount,
}
#endregion


#region 接口

#endregion


public class UIMapPath
{
    public const string m_PrefabPath = "Prefabs/UIMap/";
    public const string m_MapChunkPath = "Art/UIMap/";
}


#region 标记点类型
[Serializable]
public class UIMapFlagPointBase
{
    /// <summary>
    /// 世界中 标记点 实体的 世界位置
    /// </summary>
    [SerializeField]
    public Vector3 m_WorldPos = default(Vector3);
    /// <summary>
    /// 标记
    /// </summary>
    [SerializeField]
    public string m_Name = default(string);
    /// <summary>
    /// 当前标记点类型
    /// </summary>
    public EUIMapFlagPointType m_FlagType = EUIMapFlagPointType.None;
    /// <summary>
    /// 当前标记点数据
    /// </summary>
    public dynamic m_Dynamic = null;
    /// <summary>
    /// 当前点 移动类型
    /// </summary>
    [SerializeField]
    public EUIMapFlagMoveType m_MovementType = EUIMapFlagMoveType.Static;
    /// <summary>
    /// 当前标记点显示地图
    /// </summary>
    [SerializeField]
    public EUIMapStatus m_ShowMapType = EUIMapStatus.All;
    /// <summary>
    /// update 事件 0.5s / 1
    /// </summary>
    private Func<Vector3> m_OnClickUpdate05 = null;
    public Func<Vector3> OnClickUpdate05
    {
        private get => m_OnClickUpdate05;
        set => m_OnClickUpdate05 = value;
    }
    /// <summary>
    /// 获取当前标记点再 ui 地图上面 块索引信息
    /// </summary>
    public (int index, int row, int col, Vector2 uimapPos) m_MapPositionInfo =>
        UIMapManager.Instance.GetChunkIndexByWorldPos(m_WorldPos);
    /// <summary>
    /// 当前点在 ui 地图上面的位置
    /// </summary>
    public Vector2 m_LocalPosition => UIMapManager.Instance.WorldToUIMapPos(m_WorldPos);

    public UIMapFlagPointBase(EUIMapFlagMoveType f_MovementType = EUIMapFlagMoveType.Static)
    {
        m_MovementType = f_MovementType;
        if (f_MovementType == EUIMapFlagMoveType.Move)
            UIMapManager.Instance.m_LiftCycle.UpdateFrameRate05 += Update05;
    }

    ~UIMapFlagPointBase()
    {
        if (m_MovementType == EUIMapFlagMoveType.Move)
            UIMapManager.Instance.m_LiftCycle.UpdateFrameRate05 -= Update05;
    }

    void Update05()
    {
        m_WorldPos = m_OnClickUpdate05?.Invoke() ?? m_WorldPos;
    }
}
[Serializable]
public sealed class FlagPointInfo : UIMapFlagPointBase
{
    public FlagPointInfo() : base(EUIMapFlagMoveType.Static) { }
    public EUIMapFlagPointInfoType? m_Type => m_Dynamic as EUIMapFlagPointInfoType?;
    //public EUIMapFlagPointInfoType m_Type = EUIMapFlagPointInfoType.Type1;
}
public sealed class FlagPointMonsterInfo : UIMapFlagPointBase
{
    private FlagPointMonsterInfo() : base(EUIMapFlagMoveType.Move) { }
    public Transform m_Target => m_Dynamic as Transform;
}


public enum EFlagPointInfo
{
    m_Type,
}
#endregion

