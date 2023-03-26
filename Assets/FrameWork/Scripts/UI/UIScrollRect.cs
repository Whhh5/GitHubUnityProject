using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIScrollRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public Vector2 m_BegionPosition = Vector2.zero;
    public Vector2 m_OnPosition = Vector2.zero;
    public Vector2 m_EndPosition = Vector2.zero;


    public RectTransform m_Content = null;
    public float m_EdgeDistance;
    public Vector2 m_Delta = Vector2.zero;

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_BegionPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_OnPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_EndPosition = eventData.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        
    }


    public void GetMinMaxPosition()
    {
        var minPos = m_Content.sizeDelta.x * m_Content.pivot.x;
    }
}
