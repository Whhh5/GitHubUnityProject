using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace B1.UIComponent
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class Button : MaskableGraphic, ICanvasElement,
        //UIBehaviour, ICanvasElement,
        ICanvasRaycastFilter,
        IPointerClickHandler, IEventSystemHandler, ISubmitHandler,
        IMoveHandler, ISelectHandler, IDeselectHandler
    {
        Image image = null;
        UnityEngine.UI.Button btn = null;
        //ICanvasRaycastFilter
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            Debug.Log($"  {sp}    {eventCamera}");
            return true;
        }
        //IMoveHandler
        public void OnMove(AxisEventData eventData)
        {

        }
        //IPointerDownHandler
        public void OnPointerDown(PointerEventData eventData)
        {

        }
        //ISelectHandler
        public void OnSelect(BaseEventData eventData)
        {
            Debug.Log("OnSelect");
        }
        //IDeselectHandler
        public void OnDeselect(BaseEventData eventData)
        {
            Debug.Log("OnDeselect");
        }
        //IPointerClickHandler
        public void OnPointerClick(PointerEventData eventData)
        {

        }
        //ISubmitHandler
        public void OnSubmit(BaseEventData eventData)
        {

        }
        //ICanvasElement
        //public void Rebuild(CanvasUpdate executing)
        //{
        //    Debug.Log("Rebuild");
        //}

        //public void LayoutComplete()
        //{
        //    Debug.Log("LayoutComplete");
        //}

        //public void GraphicUpdateComplete()
        //{
        //    Debug.Log("GraphicUpdateComplete");
        //}

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Debug.Log($"{source}   {destination}");
        }
    }
}
