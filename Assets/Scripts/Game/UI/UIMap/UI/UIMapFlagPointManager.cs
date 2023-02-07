using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;

public class UIMapFlagPointManager : MonoBehaviour
{
    public static UIMapFlagPointManager Instance = default(UIMapFlagPointManager);
    private void Awake()
    {
        Instance ??= this;
        OnAwake();
    }
    #region Tools
    public void Log(object message)
    {
        Debug.Log($"【 {GetType()} 】:\n {message}");
    }
    #endregion

    /// <summary>
    /// 标记点根节点
    /// </summary>
    [Space(50), Header("标记点"), SerializeField]
    private RectTransform m_FlagPointRoot;
    /// <summary>
    /// 追踪点到地图边缘的距离
    /// </summary>
    [SerializeField, Range(0, 200)]
    private float m_TraceToEdgeDistance = 0.0f;
    /// <summary>
    /// 追踪点父对象
    /// </summary>
    [SerializeField]
    private RectTransform m_TraceFlagPointRoot;
    /// <summary>
    /// 当前被追踪点列表
    /// </summary>
    [SerializeField]
    public Dictionary<EUIMapTracePointType, (int count, List<UIMapFlagPoint> list)> m_ListTrace = new();
    /// <summary>
    /// 地图快 -> 标记点列表
    /// </summary>
    [SerializeField]
    public Dictionary<uint, (bool? state, Dictionary<UIMapFlagPointBase, UIMapFlagPoint> list)> m_DicChunk_Flag = new();

    [Space(50), Header("多标记点选择界面"), SerializeField]
    private int m_;

    [Space(50)]
    [Header("地图用户标记点   flag point info type")]
    /// <summary>
    /// 标记点根对象
    /// </summary>
    [SerializeField]
    private RectTransform m_FPITRoot;
    /// <summary>
    /// 标记点物体
    /// </summary>
    [SerializeField]
    private RectTransform m_FPITItemParent;
    /// <summary>
    /// 标记点物体
    /// </summary>
    [SerializeField]
    private RectTransform m_FPITItem;
    /// <summary>
    /// 标记点名称输入
    /// </summary>
    [SerializeField]
    private TMP_InputField m_FPITInput;
    /// <summary>
    /// 标记点名称提交按钮
    /// </summary>
    [SerializeField]
    private Button m_FPITSetNameButton;
    /// <summary>
    /// 标记点列表
    /// </summary>
    [SerializeField]
    List<RectTransform> m_ListFPIT = new();
    /// <summary>
    /// 标记点半径
    /// </summary>
    [SerializeField]
    float m_FPITRadius = 100;
    /// <summary>
    /// 标记点显示最大角度
    /// </summary>
    [SerializeField, Range(0, 1)]
    float m_FPITMaxAngle = 0.5f;
    /// <summary>
    /// 标记点偏移角度
    /// </summary>
    [SerializeField, Range(-180, 180)]
    float m_FPITMoveAngle = 00.0f;
    /// <summary>
    /// 标记点整体旋转角度
    /// </summary>
    [SerializeField, Range(0, 1)]
    float m_FPITRotation = 00.0f;
    public float FPITRotate
    {
        get => m_FPITRotation;
        set
        {
            if (m_FPITRotation != value)
            {
                m_FPITRotation = value;
                UpdateFlagPointInfoType();
            }
        }
    }
    /// <summary>
    /// 标记点显示动画曲线
    /// </summary>
    [SerializeField]
    private AnimationCurve m_FPITEase;
    float m_TempAngle = 0.0f;

    /// <summary>
    /// 存放所有的标记点 表里的 数据
    /// </summary>
    public Dictionary<EUIMapFlagPointType, object> m_Data_Flag = new Dictionary<EUIMapFlagPointType, object>();

    #region 周期函数
    private void OnAwake()
    {
        // 初始化列表
        m_DicChunk_Flag = new();

        // 初始化标记点类型 以及列表
        for (int i = 0; i < (int)EUIMapFlagPointType.EnumCount; i++)
        {
            // 初始化标记数据 value = List<T>
            m_Data_Flag.Add((EUIMapFlagPointType)i, null);
            // 创建根节点
            var obj = new GameObject(((EUIMapFlagPointType)i).ToString());
            var rect = obj.AddComponent<RectTransform>();
            rect.SetParent(m_FlagPointRoot);
            rect.anchoredPosition3D = Vector3.zero;
            rect.localScale = Vector3.one;

            var traceParent = GameObject.Instantiate<RectTransform>(rect, m_TraceFlagPointRoot);
            traceParent.name = ((EUIMapFlagPointType)i).ToString();
            traceParent.anchoredPosition3D = Vector3.zero;

            //m_FlagType_Instance.Add((EUIMapFlagPointType)i, (EUIMapStatus.EnumCount, new()));
            // 获取表数据 根据优先级对根节点进行排序
        }

        // 初始化追踪类型列表
        for (int i = 0; i < (int)EUIMapTracePointType.EnumCount; i++)
        {
            m_ListTrace.Add((EUIMapTracePointType)i, (999, new()));
        }

        // 初始化用户标记点类型列表
        InitFlagPointInfoTypeWindow();
    }

    private void Start()
    {

    }
    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }
    private void Update()
    {
        //UpdateFlagPointInfoType();
    }

    private void OnDestroy()
    {
        m_DicChunk_Flag = new();
        m_ListTrace = new();
        m_Data_Flag = new();
        CloseFlagPointInfoTypeWindow();
        ClearPool();
    }
    #endregion

    #region 创建 更新
    public void CreateFlagPoint()
    {
        // 获取标记点数据

        // 遍历创建所有标记点

    }
    public void UpdateFlagPointInfo(EUIMapFlagPointType flagType)
    {
        //m_Data_Flag.TryGetValue(flagType, out var nowFlagData);
        switch (flagType)
        {
            case EUIMapFlagPointType.None:
                break;
            case EUIMapFlagPointType.NPC:
                break;
            case EUIMapFlagPointType.Monster:
                break;
            case EUIMapFlagPointType.TransferNPC:
                break;
            case EUIMapFlagPointType.Collection:
                break;
            case EUIMapFlagPointType.TransferPortal:
                break;
            case EUIMapFlagPointType.CarriagePoint:
                break;
            case EUIMapFlagPointType.AvailableTask:
                break;
            case EUIMapFlagPointType.DeliverableTask:
                break;
            case EUIMapFlagPointType.Incident:
                break;
            case EUIMapFlagPointType.FieldAnchor:
                break;
            case EUIMapFlagPointType.CitiesAndTownsAnchor:
                break;
            case EUIMapFlagPointType.ManufacturingStation:
                break;
            case EUIMapFlagPointType.FlagPoint:
                break;
            case EUIMapFlagPointType.Teammate:
                break;
            case EUIMapFlagPointType.Campsite:
                break;
            case EUIMapFlagPointType.UnderwayTask:
                break;
            case EUIMapFlagPointType.IncidentRange:
                break;
            case EUIMapFlagPointType.Hero:
                break;
            case EUIMapFlagPointType.CopyEntrance:
                break;
            case EUIMapFlagPointType.Boss:
                break;
            case EUIMapFlagPointType.MinAreaText:
                break;
            case EUIMapFlagPointType.MaxAreaText:
                break;
            case EUIMapFlagPointType.TownBoard:
                break;
            case EUIMapFlagPointType.EnumCount:
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 更新传入块索引上面的标记信息
    /// </summary>
    /// <param name="f_Index"></param>
    /// <param name="f_Active"></param>
    public void UpdateFlagPointByUIMapChunk(uint f_Index, bool f_Active)
    {
        if (m_DicChunk_Flag.TryGetValue(f_Index, out var value))
        {
            if ((value.state ?? false) == f_Active) return;
            m_DicChunk_Flag[f_Index] = (f_Active, value.list);

            // 这两个数组的是因为 pop push 操作 也对 m_DicChunk_Flag 字典进行了操作
            List<UIMapFlagPointBase> popList = new();
            List<UIMapFlagPoint> pushList = new();

            foreach (var item in value.list)
            {
                if (f_Active)
                {
                    if (item.Value != null)
                    {
                        Log("flagpoint 块隐藏后没有被释放");
                    }
                    else
                    {
                        popList.Add(item.Key);
                    }
                }
                else
                {
                    if (item.Value != null && !item.Value.GetTraceState())
                    {
                        pushList.Add(item.Value);
                    }
                }
            }

            foreach (var item in popList)
            {
                Pop(item);
            }
            foreach (var item in pushList)
            {
                Push(item);
            }
        }
        else
        {
            m_DicChunk_Flag.Add(f_Index, (f_Active, new()));
        }
    }
    /// <summary>
    /// 移动 标记点 到 块
    /// </summary>
    /// <param name="f_Flag"></param>
    /// <param name="f_ToIndex"></param>
    private void MoveFlagToChunk(UIMapFlagPoint f_Flag, int f_ToIndex)
    {
        if (f_Flag.m_ChunkIndex != f_ToIndex)
        {
            m_DicChunk_Flag[(uint)f_Flag.m_ChunkIndex].list.Remove(f_Flag.m_Base);
            if (!m_DicChunk_Flag.ContainsKey((uint)f_ToIndex))
            {
                m_DicChunk_Flag.Add((uint)f_ToIndex, (null, new()));
            }
            m_DicChunk_Flag[(uint)f_ToIndex].list.Add(f_Flag.m_Base, f_Flag);
            f_Flag.m_ChunkIndex = f_ToIndex;
            UpdateFlagPointUIMapChunkInfo(f_Flag);
        }
        else Log("index 相同不需要移动");
    }
    /// <summary>
    /// 更新单个地图标记点的块信息
    /// </summary>
    public void UpdateFlagPointUIMapChunkInfo(UIMapFlagPoint f_FlagPoint)
    {
        if (m_DicChunk_Flag.TryGetValue((uint)f_FlagPoint.m_ChunkIndex, out var list) && list.list.ContainsKey(f_FlagPoint.m_Base))
        {
            var curChunkIndex = f_FlagPoint.m_Base.m_MapPositionInfo;

            if (curChunkIndex.index != f_FlagPoint.m_ChunkIndex)
            {
                MoveFlagToChunk(f_FlagPoint, curChunkIndex.index);
            }
            else if (!(list.state ?? false) && !f_FlagPoint.GetTraceState())
            {
                Push(f_FlagPoint);
            }
        }
    }
    #endregion

    #region 加载
    /// <summary>
    /// 加载一个标记点
    /// </summary>
    /// <param name="flagType"></param>
    /// <param name="worldPos"></param>
    /// <param name="dynamicData"></param>
    public void LoadFlagPoint(EUIMapFlagPointType flagType, Vector3 worldPos, object dynamicData = null)
    {
        var data = new UIMapFlagPointBase(EUIMapFlagMoveType.Move)
        {
            m_WorldPos = worldPos,
            m_FlagType = flagType,
            m_Dynamic = dynamicData
        };
        Pop(data);
    }
    /// <summary>
    /// 卸载一个标记点
    /// </summary>
    /// <param name="flagpoint"></param>
    public void UnLoadFlagPoint(UIMapFlagPoint f_FlagPoint)
    {
        var flagBase = f_FlagPoint.m_Base;
        Push(f_FlagPoint);
        m_DicChunk_Flag[(uint)flagBase.m_MapPositionInfo.index].list.Remove(flagBase);
    }


    #endregion

    #region 对象池

    private void Push(UIMapFlagPoint f_Flag)
    {
        var f_BaseData = f_Flag.m_Base;

        UIMapFlagPoint flag = null;
        if (m_DicChunk_Flag.TryGetValue((uint)f_BaseData.m_MapPositionInfo.index, out var value)
            && value.list.TryGetValue(f_BaseData, out flag)
            && flag != null
            && flag == f_Flag)
        {
            ChangeTraceStatus(f_Flag.m_Base, false);
            f_Flag.gameObject.SetActive(false);
            f_Flag.OnDestroy();
            UIMapPoolManager.Instance.PoolPush(EUIMapAssetType.UIMapFlagPoint, f_Flag.gameObject);
            value.list[f_BaseData] = null;
        }
        else
        {
            Log($"标记点 push 失败    当前块信息不存在索引 {value}  or  数据对应的对象不存在 {flag}  or  数据和对象不匹配 {f_Flag}");
            return;
        }


    }
    public void Pop(UIMapFlagPointBase f_BaseData, Action<UIMapFlagPoint> f_Callback = null)
    {
        Action<UIMapFlagPoint> action = (value) =>
            {
                if (!m_DicChunk_Flag.ContainsKey((uint)f_BaseData.m_MapPositionInfo.index))
                {
                    m_DicChunk_Flag.Add((uint)f_BaseData.m_MapPositionInfo.index, (null, new()));
                }

                var dicValue = m_DicChunk_Flag[(uint)f_BaseData.m_MapPositionInfo.index];
                if (!dicValue.list.TryGetValue(f_BaseData, out var obj))
                {
                    dicValue.list.Add(f_BaseData, value);
                }
                else if (obj == null)
                {
                    dicValue.list[f_BaseData] = value;
                }
                else
                {
                    Log($"当前标记点已有数据实体 对象加载失败 放回池中    f_BaseData = {f_BaseData}   obj = {obj}   type = {f_BaseData.m_FlagType}    world position = {f_BaseData.m_WorldPos}");
                    Push(value);
                    return;
                }

                value.Awake();
                value.Start();
                value.SetParameters(f_BaseData);
                ChangeTraceStatus(f_BaseData, false);
                UpdateFlagPointScale(m_FPITRoot.localScale.x);
                f_Callback?.Invoke(value);
            };
        UIMapPoolManager.Instance.PoolPop(EUIMapAssetType.UIMapFlagPoint, GetParent(f_BaseData.m_FlagType), action);
    }
    public void ClearPool()
    {
        UIMapPoolManager.Instance.ReleasePoolAssets(EUIMapAssetType.UIMapFlagPoint);
    }

    #endregion

    #region 功能函数
    /// <summary>
    /// 获取某参考点周围的 点  UIMap 地图坐标 左下角 （0，0）
    /// </summary>
    /// <param name="target"> 地图参考点 </param>
    /// <param name="radius"> 半径 </param>
    /// <returns></returns>
    public List<(float distance, UIMapFlagPoint uiMapFlagPoint)> GetRangePoint(UIMapFlagPoint target, uint radius = 100)
    {
        var chunkUnitWH = UIMapManager.Instance.m_ChunkUnitWH;
        var extendIndex = Mathf.CeilToInt(radius / (Mathf.Min(chunkUnitWH.x, chunkUnitWH.y) / 2.0f));

        var list = UIMapManager.Instance.GetRangeChunk(target.m_ChunkIndex, extendIndex);
        var data = (from keyIndex in list
                    where m_DicChunk_Flag.ContainsKey(keyIndex)
                    from flags in m_DicChunk_Flag[keyIndex].list
                    where flags.Value != null
                    let dis = Vector2.Distance(flags.Key.m_LocalPosition, target.m_Base.m_LocalPosition)
                    where dis < radius
                    orderby dis
                    select (dis, flags.Value)).ToList();
        Log($"获取周围点    count = {data.Count}");
        return data;
    }
    /// <summary>
    /// 改变标记点的追踪状态
    /// </summary>
    /// <param name="f_FlagPoint"></param>
    public void ChangeTraceStatus(UIMapFlagPointBase f_FlagPointBase, bool? f_IsTrace = null)
    {
        if (!m_DicChunk_Flag.ContainsKey((uint)f_FlagPointBase.m_MapPositionInfo.index))
        {
            m_DicChunk_Flag.Add((uint)f_FlagPointBase.m_MapPositionInfo.index, (null, new()));
        }

        if (!m_DicChunk_Flag[(uint)f_FlagPointBase.m_MapPositionInfo.index].list.ContainsKey(f_FlagPointBase))
        {
            m_DicChunk_Flag[(uint)f_FlagPointBase.m_MapPositionInfo.index].list.Add(f_FlagPointBase, null);
        }

        if (m_DicChunk_Flag[(uint)f_FlagPointBase.m_MapPositionInfo.index].list[f_FlagPointBase] == null)
        {
            if (!(f_IsTrace ?? false))
            {
                return;
            }
            Pop(f_FlagPointBase, (flagPoint) => Context(flagPoint));
        }
        else
        {
            Context(m_DicChunk_Flag[(uint)f_FlagPointBase.m_MapPositionInfo.index].list[f_FlagPointBase]);
        }


        void Context(UIMapFlagPoint flagPoint)
        {
            var isTrace = f_IsTrace ?? !flagPoint.GetTraceState();
            flagPoint.transform.SetParent(GetParent(flagPoint.m_Base.m_FlagType, isTrace));
            var (count, list) = m_ListTrace[flagPoint.m_TraceType];
            if (isTrace)
            {
                if (!list.Contains(flagPoint))
                {
                    if (list.Count >= count)
                    {
                        list.RemoveAt(0);
                    }
                    list.Add(flagPoint);
                }
            }
            else
            {
                list.Remove(flagPoint);
                m_ListTrace[flagPoint.m_TraceType].list.Remove(flagPoint);
            }
            flagPoint.ChangeTrace(f_IsTrace);
            UpdateFlagPointUIMapChunkInfo(flagPoint);
        }
    }
    /// <summary>
    /// 获取标记点在屏幕上的信息
    /// </summary>
    /// <param name="f_FlagPoint"></param>
    /// <returns></returns>
    public (bool isViewShow, Vector2 viewPosToMap, Vector2 viewWorldPos, Vector2 viewPos, Vector2 canvasPos) GetFlagPointPosInView(Vector2 f_LocalPosition)
    {
        var isViewShow = UIMapManager.Instance.IsInViewShow(f_LocalPosition);

        var disToEdge = Mathf.Min(Vector2.Distance(isViewShow.canvasPos, isViewShow.viewPos), m_TraceToEdgeDistance);

        var customView = UIMapManager.Instance.m_CustomViewWH - Vector2.one * disToEdge;

        if (!isViewShow.isViewShow)
        {
            Vector2 centreLocalPos = isViewShow.viewPos;

            var line1K = centreLocalPos.y / centreLocalPos.x;
            var angle = Mathf.Atan(line1K) * Mathf.Rad2Deg;
            var tangle = Mathf.Atan(customView.y / customView.x) * Mathf.Rad2Deg;

            if (Mathf.Abs(angle) - tangle > 0)
            {
                var UB = centreLocalPos.y > 0 ? 1 : -1;
                isViewShow.canvasPos = 0.5f * customView.y * UB * new Vector2(1 / line1K, 1);
            }
            else
            {
                var UB = centreLocalPos.x > 0 ? 1 : -1;
                isViewShow.canvasPos = 0.5f * customView.x * UB * new Vector2(1, line1K);
            }
        }
        return isViewShow;
    }
    /// <summary>
    /// 更新非追踪标记点缩放的到某个值
    /// </summary>
    /// <param name="f_ToScale"></param>
    public void UpdateFlagPointScale(float f_ToScale)
    {
        var list = from item in m_DicChunk_Flag
                   where item.Value.state ?? false
                   from para in item.Value.list
                   where para.Value?.GetTraceState() == false
                   select para.Value;

        foreach (var item in list)
        {
            item.GetComponent<RectTransform>().localScale = Vector3.one * f_ToScale;
        }
        m_FPITRoot.localScale = Vector3.one * f_ToScale;
    }

    /// <summary>
    /// 更新当前图标重叠
    /// </summary>
    public void UpdateIconOverlap(float f_Distance)
    {
        // 取周围点的中间值
    }

    /// <summary>
    /// 更新地图标记点的缩放
    /// </summary>
    public void UpdateFlagStateByUIMapSatus()
    {
        DOTween.Kill(EUIMapDGIDType.UpdateFlagStateByUIMapSatus);

        var curMapState = UIMapManager.Instance.GetUIMapState();
        var lists = from item in m_DicChunk_Flag
                    where item.Value.state ?? false
                    let dic = item.Value.list
                    from para in dic
                    where para.Value != null
                    select para.Value;

        var moveTiime = 1.0f;
        DOTween.To(() => 0.0f, (value) =>
          {
              foreach (var item in lists)
              {
                  var isShow = (item.m_Base.m_ShowMapType & curMapState) != 0;

                  var curAlphaValue = item.GetComponent<CanvasGroup>().alpha + moveTiime / 10.0f * (isShow ? 1 : -1);

                  var alpha = isShow ? Mathf.Min(curAlphaValue, 1.0f) : Mathf.Max(curAlphaValue, 0.0f);
                  item.GetComponent<CanvasGroup>().alpha = alpha;
              }
          }, 1.0f, moveTiime)
            .SetId(EUIMapDGIDType.UpdateFlagStateByUIMapSatus);
    }
    #endregion

    #region 辅助函数
    /// <summary>
    /// 获取当前类型标记点的父对象
    /// </summary>
    /// <param name="f_FlagType"></param>
    /// <param name="m_FlagTraceState"></param>
    /// <returns></returns>
    public Transform GetParent(EUIMapFlagPointType f_FlagType, bool m_FlagTraceState = false)
    {
        return m_FlagTraceState ?
            m_TraceFlagPointRoot.Find(f_FlagType.ToString()) :
            m_FlagPointRoot.Find(f_FlagType.ToString());
    }
    #endregion

    #region 其他窗口
    /// <summary>
    /// 显示 标记点选择窗口  当点击的标记点周围 有多个标记点的时候调用
    /// </summary>
    /// <param name="flagPoints"></param>
    public void ShowFlagPointSelectWindow(List<(float distance, UIMapFlagPoint uiMapFlagPoint)> flagPoints)
    {
        Log($"显示 标记点选择窗口     Flag Point Count = {flagPoints.Count}");
        // 排序
    }
    public void HideFlagPointSelectWindow()
    {

    }
    public void CloseFlagPointSelectWindow()
    {

    }


    public void InitFlagPointInfoTypeWindow()
    {
        CloseFlagPointInfoTypeWindow();


        for (int i = 1; i < (int)EUIMapFlagPointInfoType.EnumCount; i++)
        {
            var index = i;

            var obj = GameObject.Instantiate<RectTransform>(m_FPITItem, m_FPITItemParent);

            obj.gameObject.SetActive(false);

            obj.anchoredPosition3D = Vector3.zero;

            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                Log($"当前点击标记点  {index}");
                UIMapFlagPointBase flagdata = new FlagPointInfo()
                {
                    m_WorldPos = UIMapManager.Instance.UIMapToWorldPos(m_FPITRoot.anchoredPosition),
                    m_Name = $"{index}",
                    m_FlagType = EUIMapFlagPointType.FlagPoint,
                    m_Dynamic = EUIMapFlagPointInfoType.Type1
                };
                Pop(flagdata, (flag) => { });
                HideFlagPointInfoTypeWindow();
            });

            obj.Find("Text").GetComponent<TextMeshProUGUI>().text = $"{i}";

            m_ListFPIT.Add(obj);
        }

        #region 初始化 DoTween 动画
        var localScale = m_FPITItem.localScale;
        //m_FPITItem.GetComponent<Image>().maskable = false;
        DOTween.To(() => 0.0f, (value) =>
            {
                m_FPITRadius = value * 100.0f;
                UpdateFlagPointInfoType();

                foreach (var item in m_ListFPIT)
                {
                    m_FPITItem.GetComponent<CanvasGroup>().alpha =
                        item.gameObject.GetComponent<CanvasGroup>().alpha = value;
                    m_FPITItem.localScale = localScale * Mathf.Min(value * 2, 1);
                    item.localScale = Vector3.one * value;
                }

            }, 1, m_FPITEase.keys[m_FPITEase.length - 1].time)
                    .SetId(EUIMapDGIDType.ShowFlagPointInfoTypeWindow)
                    .SetEase(m_FPITEase)
                    .SetAutoKill(false)
                    .Pause()
                    .isBackwards = false;
        #endregion

        HideFlagPointInfoTypeWindow();

    }
    public void CloseFlagPointInfoTypeWindow()
    {
        DOTween.Kill(EUIMapDGIDType.ShowFlagPointInfoTypeWindow);
        foreach (var item in m_ListFPIT)
        {
            GameObject.Destroy(item.gameObject);
        }
        m_ListFPIT = new();
    }

    public void ShowFlagPointTypeWindow(Vector2 f_UIMapFlagPoint)
    {
        var list = DOTween.TweensById(EUIMapDGIDType.ShowFlagPointInfoTypeWindow);
        if (list != null && !list[0].isBackwards)
        {
            HideFlagPointInfoTypeWindow();
        }
        else
        {
            m_FPITRoot.anchoredPosition = f_UIMapFlagPoint;
            DOTween.PlayForward(EUIMapDGIDType.ShowFlagPointInfoTypeWindow);
        }
    }

    /// <summary>
    /// 显示标记点选择窗口
    /// </summary>
    /// <param name="f_UIMapFlagPoint"></param>
    public void UpdateFlagPointInfoType()
    {
        // 间隔
        float pointInterval = 30.0f;
        // 当前旋转
        float curRotation;
        // 开始第一个角度
        float startAngle;
        // 最大显示多少个
        int maxShowNum;
        // 开始显示索引
        int startIndex;
        // 最大角度

        if (false) // 平均分配角度
        {
            startIndex = 0;
            curRotation = 0;
            maxShowNum = m_ListFPIT.Count;
            startAngle = 360 * m_FPITMaxAngle;
            pointInterval = startAngle / (maxShowNum - 1);
        }
        else // 固定分配角度
        {
            var maxAngle = 360 * m_FPITMaxAngle;

            maxShowNum = Mathf.FloorToInt(maxAngle / pointInterval) + 1;

            var maxShowNum2 = maxShowNum > m_ListFPIT.Count ? m_ListFPIT.Count : maxShowNum;

            startIndex = maxShowNum > m_ListFPIT.Count ?
                0 :
                Mathf.FloorToInt((m_ListFPIT.Count - maxShowNum) * m_FPITRotation);

            startAngle = Mathf.Max(Mathf.Min(
                270.0f,
                90 + (maxShowNum2 - 1) * 0.5f * pointInterval
                ), 90.0f);

            var maxRotation = (m_ListFPIT.Count - maxShowNum) * pointInterval;

            curRotation = startIndex + maxShowNum >= m_ListFPIT.Count ? 0 : maxRotation * m_FPITRotation % pointInterval;
        }

        startAngle += m_FPITMoveAngle;

        var viewCount = Mathf.Min(maxShowNum + startIndex + Mathf.Min(Mathf.CeilToInt(m_FPITRotation), 1), m_ListFPIT.Count);
        for (int i = 0; i < m_ListFPIT.Count; i++)
        {
            var listIndex = i;

            if (listIndex < startIndex || listIndex >= viewCount)
            {
                m_ListFPIT[listIndex].gameObject.SetActive(false);
                continue;
            }

            var index = i - startIndex;

            m_ListFPIT[listIndex].gameObject.SetActive(true);

            var curAngle = startAngle - index * pointInterval + curRotation;

            var endPos = new Vector2(1 * Mathf.Cos(curAngle * Mathf.Deg2Rad), 1 * Mathf.Sin(curAngle * Mathf.Deg2Rad)) * m_FPITRadius;

            m_ListFPIT[listIndex].anchoredPosition = endPos;
        }
    }
    /// <summary>
    /// 隐藏标记点选择窗口
    /// </summary>
    public void HideFlagPointInfoTypeWindow()
    {
        DOTween.PlayBackwards(EUIMapDGIDType.ShowFlagPointInfoTypeWindow);
    }
    #endregion





    #region 辅助线条
    [Space(50)]
    public Vector3 m_TargetPoint = default(Vector3);
    public float m_TargetRadius = 100.0f;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_TargetPoint, m_TargetRadius);
    }
    #endregion
}
