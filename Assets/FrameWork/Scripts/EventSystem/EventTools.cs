using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace B1.Event
{
    public enum EEvent
    {
        None,
        SCENE_LOAD_START,
        SCENE_LOAD_FINISH,
        /// <summary>
        /// 窗口初始化完成之后调用
        /// </summary>
        UI_WINDOW_LOAD_FINISH,
        /// <summary>
        /// 窗口卸载完成之后调用，
        /// 参数是一个枚举 EWindow  
        /// </summary>
        UI_WINDOW_UNLOAD_FINISH,
        UI_WINDOW_SHOW,
        UI_WINDOW_HIDE,
        EnumCount,
    }
    public interface IEventReception<TUserData>
    {
        delegate void tyyu();
        void EventReception(EEvent eEvent)
        {
            Debug.Log("2222");
        }
        void LLL();
    }
}