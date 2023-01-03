using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

namespace B1.Event
{
    public class EventManager : Singleton<EventManager>
    {
        private Dictionary<EEvent, List<(string layer, Action<EEvent, object, (string layer, string des)> action)>> m_DicEvent =
            new Dictionary<EEvent, List<(string layer, Action<EEvent, object, (string layer, string des)> action)>>();
        public void FireEvent(EEvent f_EEvent, object f_Parameter, string f_Description)
        {
            if (m_DicEvent.TryGetValue(f_EEvent, out var value))
            {
                foreach (var item in value)
                {
                    if (item.action != null)
                    {
                        Log($"触发事件  event name = {f_EEvent}  description = {f_Description}");
                        item.action.Invoke(f_EEvent, f_Parameter, (item.layer, f_Description));
                    }
                }
            }
            else
            {
                LogWarning($"当前事件未注册 event name = {f_EEvent}");
            }
            LogEvent();
        }
        public void Subscribe(EEvent f_EEvent, Action<EEvent, object, (string layer, string des)> f_Function, string f_Layer)
        {
            if (!m_DicEvent.ContainsKey(f_EEvent))
            {
                m_DicEvent.Add(f_EEvent, new List<(string layer, Action<EEvent, object, (string layer, string des)> action)>());
            }
            if (m_DicEvent[f_EEvent].Contains((f_Layer, f_Function)))
            {
                LogError($"重复添加事件 event name = {f_EEvent},   layer = {f_Layer}");
                return;
            }
            m_DicEvent[f_EEvent].Add((f_Layer, f_Function));
            LogWarning($"订阅事件   event name = {f_EEvent}   layer = {f_Layer}");
            LogEvent();
        }
        public void Unsubscribe(EEvent f_EEvent, Action<EEvent, object, (string layer, string des)> f_function, string f_Layer)
        {
            if (m_DicEvent.TryGetValue(f_EEvent, out var value) && value.Contains((f_Layer, f_function)))
            {
                if (value.Count > 1)
                {
                    value.Remove((f_Layer, f_function));
                }
                else
                {
                    m_DicEvent.Remove(f_EEvent);
                }
                LogWarning($"取消订阅事件 event name = {f_EEvent},   layer = {f_Layer}");
            }
            else
            {
                LogError($"取消订阅事件, 不存在该事件 event name = {f_EEvent},   layer = {f_Layer}");
            }
            LogEvent();
        }
        public void UnsubscribeAll(EEvent f_EEvent)
        {
            if (m_DicEvent.TryGetValue(f_EEvent, out var list))
            {
                #region Log
                string str = $"当前取消全部订阅 event name = {f_EEvent}    count = {list.Count}";
                str += "\n{";
                foreach (var item in list)
                {
                    str += $"\n\t event layer = {item.layer} ";
                }
                str += "\n}";
                LogWarning(str);
                #endregion

                m_DicEvent.Remove(f_EEvent);
            }
            else
            {
                LogError($"当前事件未注册 event name = {f_EEvent}");
            }
            LogEvent();
        }
        public void LogEvent()
        {
            string str = $" Console Event Subscribe   Event Count = {m_DicEvent.Count}";
            uint index = 0;
            foreach (var dicEvent in m_DicEvent)
            {
                str += $"\n[ {index++} ] [ {dicEvent.Key} {dicEvent.Value.Count} ] = " +
                    $"\n{{";
                uint eventIndex = 0;
                foreach (var item in dicEvent.Value)
                {
                    str += $"\n\t[ {eventIndex++} ] = " +
                        $"\n\t{{" +
                        $"\n\t\tevent layer \t= {item.layer}" +
                        $"\n\t\tevent action \t= {item.action}" +
                        $"\n\t}}";
                }
                str += $"\n}}";
            }
            LogWarning(str);
        }
    }
}
