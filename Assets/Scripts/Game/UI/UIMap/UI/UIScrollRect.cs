using DG.Tweening;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [SelectionBase]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class UIScrollRect : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement
    {
        #region Var
        [Serializable]
        public class ScrollRectEvent : UnityEvent<Vector2> { }

        [SerializeField]
        private RectTransform m_Content;
        public RectTransform content { get { return m_Content; } set { m_Content = value; } }


        [SerializeField]
        private float m_DecelerationRate = 0.135f;
        public float decelerationRate { get { return m_DecelerationRate; } set { m_DecelerationRate = value; } }


        [SerializeField]
        private RectTransform m_Viewport;
        public RectTransform viewport { get { return m_Viewport; } set { m_Viewport = value; SetDirtyCaching(); } }


        [SerializeField]
        private ScrollRectEvent m_OnValueChanged = new ScrollRectEvent();
        public ScrollRectEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }


        private Vector2 m_PointerStartLocalCursor = Vector2.zero;
        protected Vector2 m_ContentStartPosition = Vector2.zero;

        private RectTransform m_ViewRect;

        protected RectTransform viewRect
        {
            get
            {
                if (m_ViewRect == null)
                    m_ViewRect = m_Viewport;
                if (m_ViewRect == null)
                    m_ViewRect = (RectTransform)transform;
                return m_ViewRect;
            }
        }

        protected Bounds m_ContentBounds;
        private Bounds m_ViewBounds;

        private Vector2 m_Velocity;
        public Vector2 velocity { get { return m_Velocity; } set { m_Velocity = value; } }


        private bool m_Dragging;

        private Vector2 m_PrevPosition = Vector2.zero;
        private Bounds m_PrevContentBounds;
        private Bounds m_PrevViewBounds;
        [NonSerialized]
        private bool m_HasRebuiltLayout = false;

        [System.NonSerialized] private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private RectTransform m_HorizontalScrollbarRect;
        private RectTransform m_VerticalScrollbarRect;

        // field is never assigned warning
#pragma warning disable 649
        private DrivenRectTransformTracker m_Tracker;
#pragma warning restore 649

        protected UIScrollRect()
        { }

        #endregion

        #region Function
        /// <summary>
        /// Rebuilds the scroll rect data after initialization.
        /// </summary>
        /// <param name="executing">The current step in the rendering CanvasUpdate cycle.</param>
        public virtual void Rebuild(CanvasUpdate executing)
        {
            if (executing == CanvasUpdate.Prelayout)
            {
                UpdateCachedData();
            }

            if (executing == CanvasUpdate.PostLayout)
            {
                UpdateBounds();
                UpdatePrevData();

                m_HasRebuiltLayout = true;
            }
        }

        public virtual void LayoutComplete()
        { }

        public virtual void GraphicUpdateComplete()
        { }

        void UpdateCachedData()
        {
            Transform transform = this.transform;

            // These are true if either the elements are children, or they don't exist at all.
            bool viewIsChild = (viewRect.parent == transform);
            bool hScrollbarIsChild = (!m_HorizontalScrollbarRect || m_HorizontalScrollbarRect.parent == transform);
            bool vScrollbarIsChild = (!m_VerticalScrollbarRect || m_VerticalScrollbarRect.parent == transform);
            bool allAreChildren = (viewIsChild && hScrollbarIsChild && vScrollbarIsChild);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            SetDirty();
        }

        protected override void OnDisable()
        {
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

            m_Dragging = false;
            m_HasRebuiltLayout = false;
            m_Tracker.Clear();
            m_Velocity = Vector2.zero;
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        public override bool IsActive()
        {
            return base.IsActive() && m_Content != null;
        }

        private void EnsureLayoutHasRebuilt()
        {
            if (!m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
                Canvas.ForceUpdateCanvases();
        }

        public virtual void StopMovement()
        {
            m_Velocity = Vector2.zero;
        }

        public virtual void OnScroll(PointerEventData data)
        {
            if (!IsActive())
                return;

            EnsureLayoutHasRebuilt();
            UpdateBounds();

            Vector2 delta = data.scrollDelta;
            // Down is positive for scroll events, while in UI system up is positive.
            delta.y *= -1;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                delta.y = delta.x;
            delta.x = 0;

            if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                delta.x = delta.y;
            delta.y = 0;

            Vector2 position = m_Content.anchoredPosition;
            position += delta * 0;

            position += CalculateOffset(position - m_Content.anchoredPosition);

            SetContentAnchoredPosition(position);
            UpdateBounds();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Velocity = Vector2.zero;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive())
                return;

            UpdateBounds();

            m_PointerStartLocalCursor = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out m_PointerStartLocalCursor);
            m_ContentStartPosition = m_Content.anchoredPosition;
            m_Dragging = true;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Dragging = false;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!m_Dragging)
                return;

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive())
                return;

            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out localCursor))
                return;

            UpdateBounds();

            var pointerDelta = localCursor - m_PointerStartLocalCursor;
            Vector2 position = m_ContentStartPosition + pointerDelta;

            // Offset to get content into place in the view.
            Vector2 offset = CalculateOffset(position - (m_Content.anchoredPosition));
            position += offset;


            SetContentAnchoredPosition(position);
        }

        [SerializeField]
        private RectTransform m_ScaleObject = null;
        private Vector2 m_ContentPos = Vector2.zero;
        public virtual void SetContentAnchoredPosition(Vector2 position)
        {
            Vector3 scale = m_ScaleObject?.localScale ?? Vector3.one;
            var maxMoveDis = (Vector2)m_ViewBounds.size - m_Content.sizeDelta;
            position.x = Mathf.Clamp(position.x, maxMoveDis.x + m_ScaleObject.rect.x / 2 * (scale.x - 1), -m_ScaleObject.rect.x / 2 * (scale.x - 1));
            position.y = Mathf.Clamp(position.y, maxMoveDis.y + m_ScaleObject.rect.y / 2 * (scale.x - 1), -m_ScaleObject.rect.y / 2 * (scale.y - 1));

            if (position != m_Content.anchoredPosition)
            {
                m_ContentPos = position - m_Content.anchoredPosition;
            }
        }

        protected virtual void LateUpdate()
        {
            if (!m_Content)return;
            m_Content.anchoredPosition += m_ContentPos;
            m_ContentPos = Vector3.zero;

            EnsureLayoutHasRebuilt();
            UpdateBounds();
            float deltaTime = Time.unscaledDeltaTime;
            Vector2 offset = CalculateOffset(Vector2.zero);
            if (deltaTime > 0.0f)
            {
                if (!m_Dragging && (offset != Vector2.zero || m_Velocity != Vector2.zero))
                {
                    Vector2 position = m_Content.anchoredPosition;
                    for (int axis = 0; axis < 2; axis++)
                    {
                        m_Velocity[axis] *= Mathf.Pow(m_DecelerationRate, deltaTime);
                        if (Mathf.Abs(m_Velocity[axis]) < 1)
                            m_Velocity[axis] = 0;
                        position[axis] += m_Velocity[axis] * deltaTime;
                    }
                    offset = CalculateOffset(position - m_Content.anchoredPosition);
                    position += offset;

                    SetContentAnchoredPosition(position);
                }
                else
                {
                    Vector3 newVelocity = (m_Content.anchoredPosition - m_PrevPosition) / deltaTime;
                    m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, deltaTime * 10);
                }
            }

            if (m_ViewBounds != m_PrevViewBounds || m_ContentBounds != m_PrevContentBounds || m_Content.anchoredPosition != m_PrevPosition)
            {
                UISystemProfilerApi.AddMarker("ScrollRect.value", this);
                m_OnValueChanged.Invoke(normalizedPosition);
                UpdatePrevData();
            }
        }

        protected void UpdatePrevData()
        {
            if (m_Content == null)
                m_PrevPosition = Vector2.zero;
            else
                m_PrevPosition = m_Content.anchoredPosition;
            m_PrevViewBounds = m_ViewBounds;
            m_PrevContentBounds = m_ContentBounds;
        }

        #region normalized
        public Vector2 normalizedPosition
        {
            get
            {
                return new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition);
            }
            set
            {
                SetNormalizedPosition(value.x, 0);
                SetNormalizedPosition(value.y, 1);
            }
        }

        public float horizontalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                if ((m_ContentBounds.size.x <= m_ViewBounds.size.x) || Mathf.Approximately(m_ContentBounds.size.x, m_ViewBounds.size.x))
                    return (m_ViewBounds.min.x > m_ContentBounds.min.x) ? 1 : 0;
                return (m_ViewBounds.min.x - m_ContentBounds.min.x) / (m_ContentBounds.size.x - m_ViewBounds.size.x);
            }
            set
            {
                SetNormalizedPosition(value, 0);
            }
        }

        public float verticalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                if ((m_ContentBounds.size.y <= m_ViewBounds.size.y) || Mathf.Approximately(m_ContentBounds.size.y, m_ViewBounds.size.y))
                    return (m_ViewBounds.min.y > m_ContentBounds.min.y) ? 1 : 0;

                return (m_ViewBounds.min.y - m_ContentBounds.min.y) / (m_ContentBounds.size.y - m_ViewBounds.size.y);
            }
            set
            {
                SetNormalizedPosition(value, 1);
            }
        }

        /// <summary>
        /// >Set the horizontal or vertical scroll position as a value between 0 and 1, with 0 being at the left or at the bottom.
        /// </summary>
        /// <param name="value">The position to set, between 0 and 1.</param>
        /// <param name="axis">The axis to set: 0 for horizontal, 1 for vertical.</param>
        protected virtual void SetNormalizedPosition(float value, int axis)
        {
            EnsureLayoutHasRebuilt();
            UpdateBounds();
            // How much the content is larger than the view.
            float hiddenLength = m_ContentBounds.size[axis] - m_ViewBounds.size[axis];
            // Where the position of the lower left corner of the content bounds should be, in the space of the view.
            float contentBoundsMinPosition = m_ViewBounds.min[axis] - value * hiddenLength;
            // The new content localPosition, in the space of the view.
            float newAnchoredPosition = m_Content.anchoredPosition[axis] + contentBoundsMinPosition - m_ContentBounds.min[axis];

            Vector3 anchoredPosition = m_Content.anchoredPosition;
            if (Mathf.Abs(anchoredPosition[axis] - newAnchoredPosition) > 0.01f)
            {
                anchoredPosition[axis] = newAnchoredPosition;
                m_Content.anchoredPosition = anchoredPosition;
                m_Velocity[axis] = 0;
                UpdateBounds();
            }
        }
        #endregion

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        protected void UpdateBounds()
        {
            m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            m_ContentBounds = GetBounds();

            if (m_Content == null)
                return;

            Vector3 contentSize = m_ContentBounds.size;
            Vector3 contentPos = m_ContentBounds.center;
            var contentPivot = m_Content.pivot;
            AdjustBounds(ref m_ViewBounds, ref contentPivot, ref contentSize, ref contentPos);
            m_ContentBounds.size = contentSize;
            m_ContentBounds.center = contentPos;

            Vector2 delta = Vector2.zero;
            if (m_ViewBounds.max.x > m_ContentBounds.max.x)
            {
                delta.x = Math.Min(m_ViewBounds.min.x - m_ContentBounds.min.x, m_ViewBounds.max.x - m_ContentBounds.max.x);
            }
            else if (m_ViewBounds.min.x < m_ContentBounds.min.x)
            {
                delta.x = Math.Max(m_ViewBounds.min.x - m_ContentBounds.min.x, m_ViewBounds.max.x - m_ContentBounds.max.x);
            }

            if (m_ViewBounds.min.y < m_ContentBounds.min.y)
            {
                delta.y = Math.Max(m_ViewBounds.min.y - m_ContentBounds.min.y, m_ViewBounds.max.y - m_ContentBounds.max.y);
            }
            else if (m_ViewBounds.max.y > m_ContentBounds.max.y)
            {
                delta.y = Math.Min(m_ViewBounds.min.y - m_ContentBounds.min.y, m_ViewBounds.max.y - m_ContentBounds.max.y);
            }
            if (delta.sqrMagnitude > float.Epsilon)
            {
                contentPos = m_Content.anchoredPosition + delta;

                AdjustBounds(ref m_ViewBounds, ref contentPivot, ref contentSize, ref contentPos);
            }

        }

        internal static void AdjustBounds(ref Bounds viewBounds, ref Vector2 contentPivot, ref Vector3 contentSize, ref Vector3 contentPos)
        {
            Vector3 excess = viewBounds.size - contentSize;
            if (excess.x > 0)
            {
                contentPos.x -= excess.x * (contentPivot.x - 0.5f);
                contentSize.x = viewBounds.size.x;
            }
            if (excess.y > 0)
            {
                contentPos.y -= excess.y * (contentPivot.y - 0.5f);
                contentSize.y = viewBounds.size.y;
            }
        }

        private readonly Vector3[] m_Corners = new Vector3[4];
        private Bounds GetBounds()
        {
            if (m_Content == null)
                return new Bounds();
            m_Content.GetWorldCorners(m_Corners);
            var viewWorldToLocalMatrix = viewRect.worldToLocalMatrix;
            return InternalGetBounds(m_Corners, ref viewWorldToLocalMatrix);
        }

        internal static Bounds InternalGetBounds(Vector3[] corners, ref Matrix4x4 viewWorldToLocalMatrix)
        {
            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int j = 0; j < 4; j++)
            {
                Vector3 v = viewWorldToLocalMatrix.MultiplyPoint3x4(corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }

        private Vector2 CalculateOffset(Vector2 delta)
        {
            return InternalCalculateOffset(ref m_ViewBounds, ref m_ContentBounds, ref delta);
        }

        internal static Vector2 InternalCalculateOffset(ref Bounds viewBounds, ref Bounds contentBounds, ref Vector2 delta)
        {
            Vector2 offset = Vector2.zero;
            Vector2 min = contentBounds.min;
            Vector2 max = contentBounds.max;

            min.x += delta.x;
            max.x += delta.x;
            float maxOffset = viewBounds.max.x - max.x;
            float minOffset = viewBounds.min.x - min.x;
            if (minOffset < -0.001f)
                offset.x = minOffset;
            else if (maxOffset > 0.001f)
                offset.x = maxOffset;

            min.y += delta.y;
            max.y += delta.y;
            maxOffset = viewBounds.max.y - max.y;
            minOffset = viewBounds.min.y - min.y;
            if (maxOffset > 0.001f)
                offset.y = maxOffset;
            else if (minOffset < -0.001f)
                offset.y = minOffset;


            return offset;
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }
        protected void SetDirtyCaching()
        {
            if (!IsActive())
                return;

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);

            m_ViewRect = null;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirtyCaching();
        }

#endif
        #endregion




        #region 缩放
        public Slider m_ScaleSlider;
        public RectTransform m_ScaleRect => content;
        public Action<float> m_ScaleCallback;
        private void OnMouseScrollWheelClick(float f_Increment)
        {
            var value = m_ScaleSlider.value + f_Increment;
            value = MathF.Min(Mathf.Max(value, m_ScaleSlider.minValue), m_ScaleSlider.maxValue);
            // 缩放地图
            m_ScaleSlider.value = value;
        }
        private void ChangMapScaleTo(float f_ToValue)
        {
            DOTween.Kill(EUIMapDGIDType.ChangMapScaleTo);

            var curMOusePos = Input.mousePosition;
            DOTween.To(() => m_ScaleSlider.value, (value) =>
            {
                m_ScaleRect.localScale = Vector3.one * value;
                //m_MapScroll.velocity = Vector2.one / m_MapCurScale.value;
                //UIMapFlagPointManager.Instance.UpdateFlagPointScale(1 / value);

                //SetScaleRootPivotTo(curMOusePos);
                m_ScaleCallback?.Invoke(value);
            }, f_ToValue, 1.0f)
                        .SetId(EUIMapDGIDType.ChangMapScaleTo);
        }

        #endregion

        #region 多手指操作   
        bool m_IsHaveTouchDown;
        float m_TouchDistance;
        EUIMapStatus m_MapState;
        private void OnTouchClick()
        {
            m_IsHaveTouchDown = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0);
            if (Input.touchCount < 2 || m_MapState != EUIMapStatus.BigMap) return;
            var dis = Vector2.Distance(Input.GetTouch(0).deltaPosition, Input.GetTouch(1).deltaPosition);

            switch (Input.GetTouch(1).phase)
            {
                case TouchPhase.Began:
                    m_TouchDistance = dis;
                    break;
                case TouchPhase.Moved:
                    #region 手指移动

                    var inrcementDis = dis - m_TouchDistance;
                    inrcementDis /= 100;
                    OnMouseScrollWheelClick(inrcementDis);
                    m_TouchDistance = dis;

                    #endregion
                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Ended:
                    break;
                case TouchPhase.Canceled:
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
