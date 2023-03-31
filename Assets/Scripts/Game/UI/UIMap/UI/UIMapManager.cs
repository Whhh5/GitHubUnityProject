using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMapManager : MonoBehaviour
{
    private void Awake()
    {
        Instance ??= this;
        OnAwake();
    }
    public static UIMapManager Instance = null;

    #region enum
    enum EMapShape
    {
        Box,
        Sphere,
        EnumCount,
    }
    enum EScaleModule
    {
        GUICentre,
        MouseAndTouch,
        EnumCount,
    }
    #endregion

    #region Var
    /// <summary>
    /// UI地图快行数 列数
    /// </summary>
    [Header("基础参数")]
    [SerializeField]
    private Vector2Int m_RowCol;
    /// <summary>
    /// 整个UI地图的长宽
    /// </summary>
    [SerializeField]
    private Vector2Int m_UIMapOriginalWH;
    /// <summary>
    /// 世界地图 -> UI 地图比例
    /// </summary>
    [SerializeField]
    private float m_RatioOriginalWH;
    /// <summary>
    /// 资源目录 (最后不要加斜杠) 例：Assets/Resources/Images
    /// </summary>
    [SerializeField]
    private string m_AssetsMenu;

    /// <summary>
    /// （0， 0） 点 对应世家中的位置 左下角
    /// </summary>
    [SerializeField]
    private Vector2 m_StartingPoint;

    /// <summary>
    /// 视图左下块  根据这个属性来判断地图位置是否有变化 来刷新地图快加载
    /// </summary>
    [Space(50)]
    [Header("根据基础属性动态改变的")]
    [SerializeField]
    private Vector2Int m_ViewLeftBottomPointRC;
    /// <summary>
    /// 地图资源名字 例：ImageName_0   则此处应该填入 ImageName
    /// </summary>
    [SerializeField]
    private string m_AssetsName;

    private Vector2Int m_UIMapWH => new Vector2Int((int)(m_UIMapOriginalWH.x * m_MapCurScale.value), (int)(m_UIMapOriginalWH.y * m_MapCurScale.value));
    private float m_RatopWH => m_WorldMapWH.x / m_UIMapWH.x;
    /// <summary>
    /// 世界地图的长宽
    /// </summary>
    [SerializeField]
    private Vector2 m_WorldMapWH => (Vector2)m_UIMapOriginalWH * m_RatioOriginalWH;
    /// <summary>
    /// 地图快的长宽
    /// </summary>
    [SerializeField]
    public Vector2Int m_ChunkUnitWH => new Vector2Int(m_UIMapOriginalWH.x / m_RowCol.y, m_UIMapOriginalWH.y / m_RowCol.x);
    private Vector2Int m_ChunkWorldUnitWH => new Vector2Int((int)(m_ChunkUnitWH.x * m_MapCurScale.value), (int)(m_ChunkUnitWH.y * m_MapCurScale.value));
    /// <summary>
    /// 整个屏幕最大能放下多少块
    /// </summary>
    [SerializeField]
    private Vector2Int m_ViewMaxChunkRC => new Vector2Int(Mathf.CeilToInt(m_CustomViewWH.y / m_ChunkWorldUnitWH.y), Mathf.CeilToInt(m_CustomViewWH.x / m_ChunkWorldUnitWH.x));

    /// <summary>
    /// 玩家在世界中的位置
    /// </summary>
    [Space(50)]
    [Header(" 临时属性 ")]
    [SerializeField]
    private Vector2 m_HeroPosToWorld;
    [SerializeField]
    private Camera m_MainCamera;
    [SerializeField]
    private Vector2Int m_TempData;
    [SerializeField]
    private RectTransform m_TempTarget;
    [SerializeField]
    private Image m_TempChunk;
    public RectTransform m_Target2;
    public RectTransform m_ViewCentre;
    public RectTransform m_ContentRoot;

    /// <summary>
    /// 地图快 Scroll 的 Content
    /// </summary>
    [Space(50)]
    [Header("对象")]
    [SerializeField]
    private RectTransform m_MapContent;
    /// <summary>
    /// 地图快
    /// </summary>
    [SerializeField]
    private RectTransform m_ChunkParent;
    /// <summary>
    /// 地图Scroll层
    /// </summary>
    [SerializeField]
    private RectTransform m_RectScroll;
    private ScrollRect2 m_MapScroll => m_RectScroll.GetComponent<ScrollRect2>();
    /// <summary>
    /// 地图快遮罩层
    /// </summary>
    [SerializeField]
    private RectTransform m_RectMask;
    /// <summary>
    /// 标记点根节点
    /// </summary>
    [SerializeField]
    private RectTransform m_FlagPointRoot;
    /// <summary>
    /// 地图缩放对象
    /// </summary>
    [SerializeField]
    private RectTransform m_ScaleRect;
    /// <summary>
    /// 地图缩放对象
    /// </summary>
    private RectTransform m_MapScaleRect => m_ScaleRect;

    /// <summary>
    /// 存放地图快图片
    /// </summary>
    [Space(50)]
    [Header("容器")]
    Dictionary<uint, UIMapChunk> m_Index_Chunk = new();


    [Header("功能变量")]
    /// <summary>
    /// 地图快遮罩层 sizeDelta 缩放
    /// </summary>
    [Range(0, 1)]
    [SerializeField]
    private float m_MaskScale = 1;
    /// <summary>
    /// 屏幕最大显示扩展
    /// </summary>
    [SerializeField]
    private int m_ExtendViewMaxRC;
    /// <summary>
    /// 地图缩放进度
    /// </summary>
    [SerializeField]
    private Slider m_MapSlider;
    /// <summary>
    /// 固定屏幕中心或者鼠标位置
    /// </summary>
    private EScaleModule m_ScaleModule;
    /// <summary>
    /// 获取当前地图缩放 <C>  </C>
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see langword="slider" /> = 总缩放进度(0, 1) ， <see langword="value" /> = localScale
    ///     </para>
    /// </remarks>
    /// <returns></returns>
    [SerializeField]
    public (float slider, float value) m_MapCurScale => ((m_MapScaleRect.localScale.x - m_MapSlider.minValue) / (m_MapSlider.maxValue - m_MapSlider.minValue), m_MapScaleRect.localScale.x);
    /// <summary>
    /// 自定义屏幕大小
    // </summary>
    [SerializeField]
    public Vector2 m_ViewWH => new Vector2(m_MainCamera.pixelWidth, m_MainCamera.pixelHeight);
    /// <summary>
    /// 自定义屏幕大小
    /// </summary>
    [SerializeField]
    public Vector2 m_CustomViewWH => m_ViewWH * m_MaskScale;
    /// <summary>
    /// 自定义屏幕世界大小
    /// </summary>
    [SerializeField]
    public Vector2 m_CustomViewWorldWH => m_CustomViewWH / m_MapCurScale.value;

    /// <summary>
    /// 当前地图状态  大地图 or 小地图
    /// </summary>
    [Header("只读变量  请不要在面板上更改   只是方便调试")]
    private EUIMapStatus m_MapState;
    /// <summary>
    /// 当前小地图的样式
    /// </summary>
    [SerializeField]
    private EMapShape m_MapShap = EMapShape.Box;
    /// <summary>
    /// 是否有手指正在触摸屏幕
    /// </summary>
    [SerializeField]
    private bool m_IsHaveTouchDown = false;
    /// <summary>
    /// 两个手指之间的距离
    /// </summary>
    [SerializeField]
    private float m_TouchDistance = 0.0f;
    [SerializeField]
    private Vector2 m_MousePosition => false ? (m_MapScroll.m_EventData?.position ?? Vector2.zero) : Input.mousePosition;

    private Dictionary<ushort, Action> m_MapAction = new Dictionary<ushort, Action>
    {
        //(1, ()=>{ }),

    };

    /// <summary>
    /// 其他脚本
    /// </summary>
    public UIMapLifeCycle m_LiftCycle => GetComponent<UIMapLifeCycle>();

    #endregion

    #region 周期函数
    private void OnAwake()
    {
        m_AssetsName = m_AssetsMenu = default(string);

        // 注册事件
        m_MapContent.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (m_MapState == EUIMapStatus.MiniMap)
                {
                    ChangeUIMapState(EUIMapStatus.BigMap);
                }
                else
                {
                    UIMapFlagPointManager.Instance.ShowFlagPointTypeWindow(GetUIMapPosByViewPos(m_MousePosition));
                }
            });

        #region 地图缩放条
        var mapScale = new Vector3(0.5f, 2.0f, 1.0f);
        var sliderMinValue = m_CustomViewWH / m_UIMapOriginalWH;
        sliderMinValue = new(Mathf.Max(sliderMinValue.x, mapScale.x), Mathf.Max(sliderMinValue.y, mapScale.x));
        m_MapSlider.minValue = MathF.Max(sliderMinValue.x, sliderMinValue.y);
        m_MapSlider.maxValue = mapScale.y;
        m_MapSlider.value = mapScale.z;
        m_MapSlider.onValueChanged.AddListener(OnMapScaleChangeClick);
        #endregion
    }

    private void Start()
    {
        UpdateMapInfo(0);
        //Create(Vector2Int.zero);

    }

    void OnScrollValueChangeClick(Vector2 f_Value)
    {
        UpdateViewInfo();
    } 
    private void OnEnable()
    {
        m_LiftCycle.UpdateFrameRate00 += OnScrollWheel;
        m_LiftCycle.UpdateFrameRate00 += OnTouchClick;
        //m_LiftCycle.UpdateFrameRate05 += UpdateViewInfo;

        m_MapScroll.onValueChanged.AddListener(OnScrollValueChangeClick);
    }
    private void OnDisable()
    {
        m_LiftCycle.UpdateFrameRate00 -= OnScrollWheel;
        m_LiftCycle.UpdateFrameRate00 -= OnTouchClick;
        //m_LiftCycle.UpdateFrameRate05 -= UpdateViewInfo;

        m_MapScroll.onValueChanged.RemoveListener(OnScrollValueChangeClick);
    }

    private void Update()
    {
        //UpdateViewInfo();

        //OnTouchClick();

        TestUpdate();
    }

    private void LateUpdate()
    {
    }
    private void OnDestroy()
    {
        ClearMapInfoData();
        Instance = null;
        m_MapContent.GetComponent<Button>().onClick.RemoveAllListeners();
        m_MapSlider.onValueChanged.RemoveAllListeners();
        ClearPool();
    }
    #endregion

    #region 更新信息   update info
    /// <summary>
    /// 根据地图 ID 更新地图属性
    /// </summary>
    /// <param name="nMapID"></param>
    public void UpdateMapInfo(uint nMapID)
    {
        // 先清空以前的数据
        ClearMapInfoData();

        #region table data
        m_RowCol = new Vector2Int(21, 21);
        m_UIMapOriginalWH = new Vector2Int(21504, 21504);
        m_RatioOriginalWH = 3;
        var chunkNamePrefix = "newworld";
        #endregion


        m_AssetsMenu = $"{UIMapPath.m_MapChunkPath}{chunkNamePrefix}";
        var menuArr = m_AssetsMenu.Split("/");
        m_AssetsName = false ? menuArr[menuArr.Length - 1] : chunkNamePrefix;

        m_MapContent.sizeDelta = m_UIMapOriginalWH;

        ChangeUIMapState(EUIMapStatus.MiniMap);

        UpdateMapChunk();
    }
    /// <summary>
    /// 更新视图信息  根据中心点是否变化来刷新地图快加载
    /// </summary>
    public void UpdateViewInfo()
    {
        var LB = GetUIMapPosByViewPos(Vector3.zero);
        var curMapChunk = new Vector2Int((int)LB.y / m_ChunkUnitWH.y, (int)LB.x / m_ChunkUnitWH.x);

        if (curMapChunk != m_ViewLeftBottomPointRC)
        {
            m_ViewLeftBottomPointRC = curMapChunk;
            UpdateMapChunk();
        }
    }
    #endregion



    #region 创建地图  create
    /// <summary>
    /// 创建所有地图 根据传入的行列 广度遍历 生成地图快 null = Vector2Int.Zero
    /// </summary>
    /// <param name="point"></param>
    public void Create(Vector2Int? point = null)
    {
        var points = point ?? Vector2Int.zero;
        PushAllMapChunk();
        StartCoroutine(IECreate(points));

    }
    /// <summary>
    /// 更新当前屏幕显示的地图块
    /// </summary>
    public void UpdateMapChunk()
    {
        StartCoroutine(IEUpdateChunk(Vector2Int.zero));
    }
    public IEnumerator IECreate(Vector2Int point)
    {
        yield return 0;
        int unit = 1;
        for (int i = -unit; i < unit + 1; i++)
        {
            for (int j = -unit; j < unit + 1; j++)
            {
                var temp_Point = point + new Vector2Int(i, j);
                var index = (uint)((temp_Point.x * m_RowCol.y) + temp_Point.y);
                if (temp_Point.x >= 0 && temp_Point.y >= 0
                    && temp_Point.x < m_RowCol.x
                    && temp_Point.y < m_RowCol.y
                    && !m_Index_Chunk.ContainsKey(index))
                {
                    m_Index_Chunk.Add(index, null);
                    StartCoroutine(IECreate(temp_Point));
                    Pop(temp_Point, (chunk) => m_Index_Chunk[index] = chunk);
                }
            }
        }
    }
    public IEnumerator IEUpdateChunk(Vector2Int point)
    {
        yield return 0;
        var startingPoint = m_ViewLeftBottomPointRC - Vector2Int.one * m_ExtendViewMaxRC;
        var maxChunkRC = m_ViewMaxChunkRC + Vector2Int.one * m_ExtendViewMaxRC * 2 + new Vector2Int(1, 1);
        Dictionary<uint, UIMapChunk> newDic = new();
        for (int i = 0; i < maxChunkRC.x; i++)
        {
            for (int j = 0; j < maxChunkRC.y; j++)
            {
                var temp_Point = startingPoint + new Vector2Int(i, j);
                var index = (uint)((temp_Point.x * m_RowCol.y) + temp_Point.y);
                // 判断是否超出地图最大最小值
                if (temp_Point.x >= 0 && temp_Point.y >= 0
                    && temp_Point.x < m_RowCol.x
                    && temp_Point.y < m_RowCol.y
                    )
                {
                    // 判断是否在当前屏幕范围内
                    if (temp_Point.x >= startingPoint.x && temp_Point.x < maxChunkRC.x + startingPoint.x
                        && temp_Point.y >= startingPoint.y && temp_Point.y < maxChunkRC.y + startingPoint.y)
                    {
                        if (m_Index_Chunk.TryGetValue(index, out var value) && value != null)
                        {
                            newDic.TryAdd(index, value);
                        }
                        else
                        {
                            newDic.TryAdd(index, null);
                            Pop(temp_Point, (chunk) => newDic[index] = chunk);
                        }

                        // 更新当前地图快上面的标记信息  other class
                        UIMapFlagPointManager.Instance.UpdateFlagPointByUIMapChunk(index, true);
                    }
                }
            }
        }
        foreach (var item in m_Index_Chunk)
        {
            if (!newDic.ContainsKey(item.Key) && item.Value != null)
            {
                Push(item.Value);

                // 更新当前地图快上面的标记信息  other class
                UIMapFlagPointManager.Instance.UpdateFlagPointByUIMapChunk(item.Key, false);
            }
        }
        m_Index_Chunk = newDic;
    }

    /// <summary>
    /// 释放单个图片资源
    /// </summary>
    /// <param name="image"></param>
    private void UnLoadSprite(UIMapChunk f_Chunk)
    {
        if (f_Chunk.Image.sprite != null)
        {
            UIMapAssetsManager.Instance.UnLoadSpriteAssets(f_Chunk.GetInstanceID().ToString(), f_Chunk.m_AssetPath);
        }
    }
    /// <summary>
    /// 将所有的块放到池中
    /// </summary>
    private void PushAllMapChunk()
    {
        foreach (var item in m_Index_Chunk)
            if (item.Value != null)
            {
                Push(item.Value);
            }
        m_Index_Chunk = new();
    }
    /// <summary>
    /// 清空地图配置信息数据
    /// </summary>
    private void ClearMapInfoData()
    {
        ChangeMapStatusClearMapData();
        PushAllMapChunk();
        m_AssetsName = m_AssetsMenu = default(string);
    }
    /// <summary>
    /// 全部清空地图数据
    /// </summary>
    /// <param name="state"></param>
    public void ChangeMapStatusClearMapData(EUIMapStatus? state = null)
    {
        // 遍历标记点 判断现实条件  根据标记点类型 显示隐藏 一类标记点
        if (state == null)
        {
            ChangeMapStatusClearMapData(EUIMapStatus.MiniMap);
            ChangeMapStatusClearMapData(EUIMapStatus.BigMap);
        }
        else
        {
            switch (state)
            {
                case EUIMapStatus.BigMap:
                    UIMapFlagPointManager.Instance.HideFlagPointInfoTypeWindow();
                    UIMapFlagPointManager.Instance.HideFlagPointSelectWindow();
                    break;
                case EUIMapStatus.MiniMap:
                    break;
                default:
                    break;
            }
        }
    }
    #endregion

    #region 转换  change
    /// <summary>
    /// 根据 row col 获取索引    读取突变资源时候用的
    /// </summary>
    /// <param name="rowCol">行 列</param>
    /// <returns></returns>
    private uint GetChunkIndex(Vector2Int rowCol)
    {
        int retData = default;
        if (false)
        {
            retData = rowCol.x * m_RowCol.y + rowCol.y;
        }
        else
        {
            retData = (m_RowCol.x - rowCol.x - 1) * m_RowCol.y + rowCol.y;
        }
        return (uint)retData;
    }
    /// <summary>
    /// 块索引 -> 行列
    /// </summary>
    /// <param name="f_Index"></param>
    /// <returns></returns>
    private Vector2Int GetChunkRowCol(int f_Index)
    {
        return new Vector2Int
            (
                f_Index / m_RowCol.y,
                f_Index % m_RowCol.y
            );
    }
    /// <summary>
    /// 传入世界空间中一个点转换到UI地图上面的坐标 左下 0，0
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public Vector2 WorldToUIMapPos(Vector3 worldPos)
    {
        var worldDistance = new Vector2(Mathf.Abs(worldPos.x - m_StartingPoint.x), Mathf.Abs(worldPos.z - m_StartingPoint.y));
        var pixelDistance = worldDistance * 100 / m_RatopWH / m_MapCurScale.value;
        return pixelDistance;
    }
    /// <summary>
    /// ui map position => world position
    /// </summary>
    /// <param name="f_uimapPos"></param>
    /// <returns></returns>
    public Vector3 UIMapToWorldPos(Vector2 f_UIMapPos)
    {
        var worldDistance = (f_UIMapPos * m_RatopWH * m_MapCurScale.value) / 100;
        return new Vector3(worldDistance.x, 0, worldDistance.y);
    }
    /// <summary>
    /// 判断是否在屏幕内    传入 ui 地图上面的坐标左下（0，0） 返回 该点在屏幕上面的坐标
    /// </summary>
    /// <param name="mapPos"></param>
    /// <returns></returns>
    public (bool isViewShow, Vector2 viewPosToMap, Vector2 viewWorldPos, Vector2 viewPos, Vector2 canvasPos) IsInViewShow(Vector2 mapPos)
    {
        var customWorldView = m_CustomViewWorldWH;

        bool isViewShow = false;

        var curMapCentrePos = GetCurMapCentrePos();
        var viewWorldPos = mapPos - curMapCentrePos;
        var canvasPos = viewWorldPos * m_MapCurScale.value;


        var line1K = viewWorldPos.y / (viewWorldPos.x == 0 ? 0.001f : viewWorldPos.x);
        line1K = line1K == 0 ? 0.001f : line1K;

        var angle = Mathf.Atan(line1K) * Mathf.Rad2Deg;

        switch (m_MapState)
        {
            case EUIMapStatus.BigMap:
                Shape_Box();
                break;
            case EUIMapStatus.MiniMap:
                switch (m_MapShap)
                {
                    case EMapShape.Box:
                        Shape_Box();
                        break;
                    case EMapShape.Sphere:
                        Shape_Sphere();
                        break;
                    case EMapShape.EnumCount:
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        void Shape_Box()
        {
            var tangle = Mathf.Atan(customWorldView.y / customWorldView.x) * Mathf.Rad2Deg;

            if (Mathf.Abs(angle) - tangle > 0)
            {
                var UB = viewWorldPos.y > 0 ? 1 : -1;
                var y = viewWorldPos.x * line1K;
                isViewShow = Mathf.Abs(y) < Mathf.Abs(customWorldView.y * 0.5f);
                if (!isViewShow)// 不在屏幕内
                {
                    viewWorldPos = 0.5f * customWorldView.y * UB * new Vector2(1 / line1K, 1);
                }
            }
            else
            {
                var UB = viewWorldPos.x > 0 ? 1 : -1;
                var x = viewWorldPos.y / line1K;
                isViewShow = Mathf.Abs(x) < Mathf.Abs(customWorldView.x * 0.5f);
                if (!isViewShow) // 不在屏幕内
                {
                    viewWorldPos = 0.5f * customWorldView.x * UB * new Vector2(1, line1K);
                }
            }
        }
        void Shape_Sphere()
        {
            //
        }


        var viewPos = viewWorldPos * m_MapCurScale.value;


        return (isViewShow, viewWorldPos + curMapCentrePos, viewWorldPos, viewPos , canvasPos);
    }
    /// <summary>
    /// 获取一个有效的地图点
    /// </summary>
    /// <param name="f_Anchored2D"></param>
    /// <returns></returns>
    public Vector2 GetValidMapPoint(Vector2 f_Anchored2D)
    {
        var tempPara = f_Anchored2D;

        tempPara.x = tempPara.x > m_UIMapOriginalWH.x ? m_UIMapOriginalWH.x : tempPara.x;
        tempPara.y = tempPara.y > m_UIMapOriginalWH.y ? m_UIMapOriginalWH.y : tempPara.y;
        tempPara.x = tempPara.x < 0 ? 0 : tempPara.x;
        tempPara.y = tempPara.y < 0 ? 0 : tempPara.y;

        return tempPara;
    }
    /// <summary>
    /// 获取一个有效的镜头左下角可移动到的的地图点
    /// </summary>
    /// <param name="f_Anchored2D"></param>
    /// <returns></returns>
    public Vector2 GetValidViewMoveMapPoint(Vector2 f_Anchored2D)
    {
        var value = new Vector2
            (
                Mathf.Clamp(f_Anchored2D.x, 0, m_UIMapOriginalWH.x - m_CustomViewWorldWH.x),
                Mathf.Clamp(f_Anchored2D.y, 0, m_UIMapOriginalWH.y - m_CustomViewWorldWH.y)
            );

        //var maxMoveDis = m_UIMapOriginalWH - m_MapContent.sizeDelta;
        //value.x = Mathf.Clamp(f_Anchored2D.x, maxMoveDis.x + m_MapScaleRect.rect.x / 2 * (m_MapCurScale.value - 1), -m_MapScaleRect.rect.x / 2 * (m_MapCurScale.value - 1));
        //value.y = Mathf.Clamp(f_Anchored2D.y, maxMoveDis.y + m_MapScaleRect.rect.y / 2 * (m_MapCurScale.value - 1), -m_MapScaleRect.rect.y / 2 * (m_MapCurScale.value - 1));

        return value;
    }
    /// <summary>
    /// 获取一个有效的地图中心点位置
    /// </summary>
    /// <param name="f_Anchored2D"></param>
    /// <returns></returns>
    public Vector2 GetValidMapCentrePos(Vector2 f_Anchored2D)
    {
        var point = GetValidViewMoveMapPoint(f_Anchored2D - m_CustomViewWorldWH * 0.5f) + m_CustomViewWorldWH * 0.5f;
        return point;
    }
    /// <summary>
    /// 传入屏幕位置 获取 ui 地图位置
    /// </summary>
    /// <param name="f_ViewPos"></param>
    /// <returns></returns>
    public Vector2 GetUIMapPosByViewPos(Vector2 f_ViewPos)
    {
        var mapPos = (f_ViewPos - m_CustomViewWH * 0.5f) / m_MapCurScale.value + m_CustomViewWH * 0.5f - m_MapContent.anchoredPosition;
        return mapPos;
    }
    /// <summary>
    /// 获取当前地图中心点的位置
    /// </summary>
    /// <returns></returns>
    public Vector2 GetCurMapCentrePos()
    {
        return GetUIMapPosByViewPos(m_CustomViewWH * 0.5f);
    }
    /// <summary>
    /// 设置当前地图的中心点
    /// </summary>
    /// <param name="f_Anchored2D"></param>
    public void SetCurMapCentrePos(Vector2 f_Anchored2D)
    {
        var validCentre = GetValidMapCentrePos(f_Anchored2D);

        var value = m_CustomViewWH * 0.5f - validCentre;

        var increment = m_CustomViewWH * (m_MapCurScale.value - 1);
        value.x = Mathf.Clamp(value.x, -(m_UIMapOriginalWH.x - m_CustomViewWorldWH.x + increment.x), increment.x);
        value.y = Mathf.Clamp(value.y, -(m_UIMapOriginalWH.y - m_CustomViewWorldWH.y + increment.y), increment.y);


        m_MapScroll.SetContentAnchoredPosition(value);
        //m_MapContent.anchoredPosition = -value;
    }
    /// <summary>
    /// 传入 ui 地图位置 获取当前所在块
    /// </summary>
    /// <param name="f_UIMapPos"></param>
    /// <returns></returns>
    public (int index, int row, int col) GetChunkIndexByUIMapPos(Vector2 f_UIMapPos)
    {
        var row = Mathf.FloorToInt(f_UIMapPos.y / m_ChunkUnitWH.y);
        var col = Mathf.FloorToInt(f_UIMapPos.x / m_ChunkUnitWH.x);
        var index = row * m_RowCol.y + col;
        return (index, row, col);
    }
    /// <summary>
    /// 传入 世界 地图位置 ，获取当前所在地图快 , ui 地图上面的位置
    /// </summary>
    /// <param name="f_UIMapPos"></param>
    /// <returns></returns>
    public (int index, int row, int col, Vector2 uimapPos) GetChunkIndexByWorldPos(Vector3 f_WorldPos)
    {
        var uiMapPos = WorldToUIMapPos(f_WorldPos);
        var chunkInfo = GetChunkIndexByUIMapPos(uiMapPos);
        return (chunkInfo.index, chunkInfo.row, chunkInfo.col, uiMapPos);
    }
    public List<uint> GetRangeChunk(int f_Index, int f_Range = 1)
    {
        var curRowCol = GetChunkRowCol(f_Index);
        List<uint> list = new();
        for (int i = curRowCol.x - f_Range; i <= curRowCol.x + f_Range; i++)
        {
            if (i < 0 || i >= m_RowCol.x) continue;
            for (int j = curRowCol.y - f_Range; j < curRowCol.y + f_Range; j++)
            {
                if (j < 0 || j >= m_RowCol.y) continue;

                list.Add((uint)(i * m_RowCol.y + j));

            }
        }
        return list;
    }
    #endregion

    #region 对象池方法   pool
    private void Push(UIMapChunk f_Chunk)
    {
        UnLoadSprite(f_Chunk);
        UIMapPoolManager.Instance.PoolPush(EUIMapAssetType.UIMapChunk, f_Chunk.gameObject);
    }
    private void Pop(Vector2Int f_RawCol, Action<UIMapChunk> f_Callback)
    {
        if (string.IsNullOrEmpty(m_AssetsName)) return;
        var index = GetChunkIndex(f_RawCol);
        var assetsPath = $"{m_AssetsMenu}/{m_AssetsName}_{index}";

        UIMapPoolManager.Instance.PoolPop<UIMapChunk>(EUIMapAssetType.UIMapChunk, m_ChunkParent, (chunk) =>
            {
                var instanceID = chunk.GetInstanceID().ToString();
                chunk.name = $"Chunk_{index}_{f_RawCol.x}_{f_RawCol.y} --- {instanceID} --- {assetsPath}";
                var rect = chunk.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.anchoredPosition3D = new Vector3(f_RawCol.y * m_ChunkUnitWH.x, f_RawCol.x * m_ChunkUnitWH.y, 0) + new Vector3(m_ChunkUnitWH.x, m_ChunkUnitWH.y, 0) / 2;
                UIMapAssetsManager.Instance.LoadSpriteAssets(chunk.Image, instanceID, assetsPath);
                chunk.SetSettings(index, f_RawCol, assetsPath);
                chunk.Image.SetNativeSize();
                f_Callback.Invoke(chunk);
            });
    }
    private void ClearPool()
    {
        UIMapPoolManager.Instance.ReleasePoolAssets(EUIMapAssetType.UIMapChunk);
    }
    #endregion

    #region 功能函数   function
    /// <summary>
    /// 移动地图中心点到某个位置
    /// </summary>
    /// <param name="f_CentrePoint"></param>
    public void MoveMapCentreToAnchored2D(Vector2 f_CentrePoint)
    {
        DOTween.Kill(EUIMapDGIDType.MoveMapCentreToAnchored2D);

        var endPos = GetValidMapCentrePos(f_CentrePoint);
        var curMapCentrePoint = GetCurMapCentrePos();

        var moveTime = false ? Vector2.Distance(endPos, curMapCentrePoint) / 1000.0f : 1.0f;
        DOTween.To(() => curMapCentrePoint, (value) =>
            {
                SetCurMapCentrePos(value);
                if (m_IsHaveTouchDown)
                {
                    DOTween.Kill(EUIMapDGIDType.MoveMapCentreToAnchored2D);
                }
            }, endPos, moveTime)
                .SetId(EUIMapDGIDType.MoveMapCentreToAnchored2D);
    }

    /// <summary>
    /// 设置当前地图的状态 大/小 地图
    /// </summary>
    /// <param name="state"></param>
    public void ChangeUIMapState(EUIMapStatus? state = null)
    {
        state = m_MapState = state ?? (EUIMapStatus)Mathf.Abs((int)m_MapState - 1);
        DOTween.Kill(EUIMapDGIDType.SetUIMapState);

        #region 需要配置的数据 setting data
        // 小地图大小
        var endValue = 0.5f;
        #endregion

        // 移动时间
        var maxMoveTime = 1.0f;
        var moveTime = m_MaskScale / maxMoveTime;
        // 小地图大小
        var miniMapSize = m_ViewWH * endValue;
        // 小地图边距
        var miniMapDege = new Vector2(100, 100);
        // 小地图缩放
        var miniMapScale = 0.3f;
        // 目标点
        Vector2 target = (m_ViewWH - miniMapSize) / 2 - miniMapDege;
        target = new Vector2(-target.x, target.y);
        // 地图边缘遮罩
        int mapDegeMask = 100;
        // slider alpha value
        bool isShowSlider = false;
        // rect mask
        RectMask2D rectMask = m_RectMask.GetComponent<RectMask2D>();
        // 获取当前地图中心点
        var curCentre = GetCurMapCentrePos();
        switch (state)
        {
            case EUIMapStatus.BigMap:
                endValue = 1;
                target = Vector2.zero;
                mapDegeMask = 0;
                miniMapScale = 1;
                isShowSlider = true;
                break;
            case EUIMapStatus.MiniMap:
                break;
            default:
                break;
        }
        // 清空地图数据
        ChangeMapStatusClearMapData(state: (EUIMapStatus)MathF.Abs((int)state - 1));

        // 缩小视图
        DOTween.To(() => m_MaskScale, (value) =>
            {
                m_MaskScale = value;
                m_RectScroll.sizeDelta = -m_ViewWH * (1 - value);
            }, endValue, moveTime)
                .SetId(EUIMapDGIDType.SetUIMapState)
                .OnUpdate(() => SetCurMapCentrePos(curCentre));

        // 移动地图
        m_RectScroll.DOAnchorPos(target, moveTime)
                .SetId(EUIMapDGIDType.SetUIMapState);

        // 地图边缘遮罩
        DOTween.To(() => rectMask.softness.x, (value) =>
            {
                rectMask.softness = Vector2Int.one * value;
            }, mapDegeMask, moveTime)
                .SetId(EUIMapDGIDType.SetUIMapState);

        // 隐藏进度条
        var canvasGroup = m_MapSlider.GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = canvasGroup.interactable = isShowSlider;
        DOTween.To(() => canvasGroup.alpha, (value) =>
         {
             canvasGroup.alpha = value;
         }, isShowSlider ? 1.0f : 0.0f, moveTime)
            .SetId(EUIMapDGIDType.SetUIMapState)
            .OnStart(() =>
                {
                    if (isShowSlider) canvasGroup.gameObject.SetActive(isShowSlider);
                    canvasGroup.blocksRaycasts = canvasGroup.interactable = false;
                })
            .OnComplete(() =>
                {
                    if (!isShowSlider) canvasGroup.gameObject.SetActive(isShowSlider);
                    canvasGroup.blocksRaycasts = canvasGroup.interactable = true;
                });

        // 不需要动画的
        DOTween.To(() => 0.0f, (value) => { }, 1.0f, moveTime)
            .SetId(EUIMapDGIDType.SetUIMapState)
            .OnStart(() =>
                {

                })
            .OnComplete(() =>
                {
                    // 释放资源
                    UIMapAssetsManager.Instance.ReleaseUnuseSpritedAssets();
                    UIMapPoolManager.Instance.ReleasePoolAssets();
                    // 地图缩放
                    m_MapSlider.value = miniMapScale;
                });

        #region 外部函数调用
        UIMapFlagPointManager.Instance.UpdateFlagStateByUIMapSatus();
        #endregion
    }

    private void SetScaleRootPivotTo(Vector2 f_ViewPos)
    {
        var centrePivot = m_ScaleRect.pivot;

        var localPivot = f_ViewPos / m_CustomViewWH - centrePivot;

        var scalePivot = localPivot / m_MapCurScale.value;

        m_ScaleRect.anchoredPosition = (scalePivot * 2) * (m_CustomViewWH * centrePivot) * (m_MapCurScale.value - 1);

        m_ScaleRect.pivot = scalePivot + centrePivot;
    }

    #endregion

    #region 按钮事件 button
    private void OnMapScaleChangeClick(float f_Value)
    {
        Vector2 guiPos = m_MousePosition;
        switch (m_ScaleModule)
        {
            case EScaleModule.GUICentre:
                guiPos = m_CustomViewWH / 2;
                break;
            case EScaleModule.MouseAndTouch:

                break;
            case EScaleModule.EnumCount:
                break;
            default:
                break;
        }
        ChangMapScaleTo(guiPos, f_Value);
    }
    /// <summary>
    /// 获取滚轮滚动
    /// </summary>
    public void OnScrollWheel()
    {
        var scrollWheelIncrement = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheelIncrement != 0) OnMouseScrollWheelClick(scrollWheelIncrement);// sheng + xia -
    }
    /// <summary>
    /// 滚轮滚动事件
    /// </summary>
    /// <param name="f_Increment"></param>
    private void OnMouseScrollWheelClick(float f_Increment)
    {
        if (m_MapState == EUIMapStatus.MiniMap) return;
        var value = m_MapSlider.value + f_Increment;
        value = MathF.Min(Mathf.Max(value, m_MapSlider.minValue), m_MapSlider.maxValue);
        // 缩放地图
        m_MapSlider.value = value;
    }
    /// <summary>
    /// 改变地图大小到某个值
    /// </summary>
    /// <param name="f_OnGUIPos"></param>
    /// <param name="f_ToValue"></param>
    private void ChangMapScaleTo(Vector2 f_OnGUIPos, float f_ToValue)
    {
        DOTween.Kill(EUIMapDGIDType.ChangMapScaleTo);

        var pivot = f_OnGUIPos / m_CustomViewWH;
        var curMOusePos = m_MousePosition;
        var moveTime = Mathf.Min(Mathf.Max(Mathf.Abs(m_MapCurScale.value - f_ToValue), 0.5f), 1.0f);
        //var curMapContre = GetCurMapCentrePos();
        DOTween.To(() => m_MapCurScale.value, (value) =>
        {
            m_MapScaleRect.localScale = Vector3.one * value;
            //SetCurMapCentrePos(curMapContre);
            UIMapFlagPointManager.Instance.UpdateFlagPointScale(1 / value);

        }, f_ToValue, moveTime)
                    .SetId(EUIMapDGIDType.ChangMapScaleTo);
    }

    /// <summary>
    /// 手指按键事件
    /// </summary>
    public void OnTouchClick()
    {
        m_IsHaveTouchDown = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0);
        if (Input.touchCount < 2 || m_MapState != EUIMapStatus.BigMap)
        {
            m_MapScroll.horizontal = m_MapScroll.vertical = true;
            return;
        }
        else
        {
            m_MapScroll.horizontal = m_MapScroll.vertical = false;
        }
        var dis = Vector2.Distance(Input.GetTouch(0).deltaPosition, Input.GetTouch(1).deltaPosition);

        switch (Input.GetTouch(1).phase)
        {
            case TouchPhase.Began:
                break;
            case TouchPhase.Moved:
                #region 手指移动
                if (m_TouchDistance <= 0.01f) m_TouchDistance = dis;

                var inrcementDis = dis - m_TouchDistance;
                inrcementDis /= 100;
                OnMouseScrollWheelClick(inrcementDis);
                m_TouchDistance = dis;

                #endregion
                break;
            case TouchPhase.Stationary:
                break;
            case TouchPhase.Ended:
                m_TouchDistance = 0.0f;
                break;
            case TouchPhase.Canceled:
                break;
            default:
                break;
        }
    }

    float m_BegionDis;

    public void OnTouchClickTest()
    {
        return;
        if (Input.GetMouseButtonDown(0))
        {
            m_BegionDis = 0.0f;
            m_MapScroll.horizontal = m_MapScroll.vertical = false;
        }

        Vector2 mouse1 = Vector2.zero;
        Vector2 mouse2 = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            var dis = Vector2.Distance(mouse2, mouse1);
            if (m_BegionDis <= 0.01f) m_BegionDis = dis;

            var inrcementDis = dis - m_BegionDis;
            inrcementDis /= 100;
            OnMouseScrollWheelClick(inrcementDis);
            m_BegionDis = dis;
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_BegionDis = 0.0f;
            m_MapScroll.horizontal = m_MapScroll.vertical = true;
        }
    }

    #endregion

    #region 外部接口    external
    /// <summary>
    /// 获取当前是小地图还是大地图
    /// </summary>
    /// <returns></returns>
    public EUIMapStatus GetUIMapState()
    {
        return m_MapState;
    }
    public bool GetChunkEnable(uint f_Index)
    {
        return m_Index_Chunk.ContainsKey(f_Index);
    }
    #endregion




    private void TestUpdate()
    {
        var rect = m_TempChunk.GetComponent<RectTransform>();
        var retData = IsInViewShow(rect.anchoredPosition);
        if (retData.isViewShow)
        {
            m_TempChunk.color = Color.green;
        }
        else
        {
            m_TempChunk.color = Color.red;
        }
        m_TempTarget.anchoredPosition = retData.canvasPos;



        var curCentre = GetCurMapCentrePos();
        m_ViewCentre.anchoredPosition = curCentre;


        #region yes
        var mousPosToMapPos = GetUIMapPosByViewPos(m_MousePosition);
        m_Target2.anchoredPosition = mousPosToMapPos;
        #endregion

        //SetScaleRootPivotTo(m_MousePosition);

        OnTouchClickTest();
    }


    /// <example>
    /// <code lang="xaml">
    /// <![CDATA[
    /// <Window>
    ///     <Window.Resources>
    ///         <local:BoolToVisibiltyConverter x:Key="converter" />
    ///     </WindwoResource>
    ///     <CheckBox Name="Check" />
    ///     <Image Visibility="{Binding ElementName=Check, Path=IsChecked, Converter={StaticResource converter}}" />
    /// </Window>
    /// ]]>
    /// </example>
    /// 


}
