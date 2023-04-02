using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

namespace B1.Event
{
    public class MessagingSystem : Singleton<MessagingSystem>
    {
        private Dictionary<EEvent, Dictionary<IMessageSystem, (object tUserdata, string tDesc)>> m_DicEvent = new();
        public void SendEvent(EEvent f_EEvent, object f_Param, string f_Description)
        {
            if (m_DicEvent.TryGetValue(f_EEvent, out var value))
            {
                foreach (var item in value)
                {
                    var tempItem = item;
                    if (tempItem.Key != null)
                    {
                        Log($"触发事件  event name = {f_EEvent}  description = {f_Description}");
                        tempItem.Key.ReceptionEvent(f_EEvent, f_Param, tempItem.Value.tUserdata, tempItem.Value.tDesc, f_Description);
                    }
                }
            }
            else
            {
                LogWarning($"当前事件未注册 event name = {f_EEvent}");
            }
        }
        public void Subscribe<T>(EEvent f_EEvent, T f_EventReception, object f_UserData, string f_SubDesc)
            where T : IMessageSystem
        {
            if (!m_DicEvent.ContainsKey(f_EEvent))
            {
                m_DicEvent.Add(f_EEvent, new());
            }
            if (m_DicEvent[f_EEvent].ContainsKey(f_EventReception))
            {
                LogError($"重复添加事件 event name = {f_EEvent},   SubDesc = {f_SubDesc}");
                return;
            }

            m_DicEvent[f_EEvent].Add(f_EventReception, (f_UserData, f_SubDesc));
            LogWarning($"订阅事件   event name = {f_EEvent}     param = {f_UserData}   SubDesc = {f_SubDesc}");
        }
        public void Unsubscribe<T>(EEvent f_EEvent, T f_EventReception)
            where T : IMessageSystem
        {
            if (m_DicEvent.TryGetValue(f_EEvent, out var value) && value.ContainsKey(f_EventReception))
            {
                if (value.Count > 1)
                {
                    value.Remove(f_EventReception);
                }
                else
                {
                    m_DicEvent.Remove(f_EEvent);
                }
                LogWarning($"取消订阅事件 event name = {f_EEvent}");
            }
            else
            {
                LogError($"取消订阅事件, 不存在该事件 event name = {f_EEvent}");
            }
        }
        public void UnsubscribeAllByEvent(EEvent f_EEvent)
        {
            if (m_DicEvent.TryGetValue(f_EEvent, out var list))
            {
                #region Log
                string str = $"当前取消全部订阅 event name = {f_EEvent}    count = {list.Count}";
                str += "\n{";
                foreach (var item in list)
                {
                    str += $"\n\t event desc = {item.Value.tDesc} ";
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
        }
        public void UnsubscribeAll()
        {
            foreach (var item in m_DicEvent)
            {
                #region Log
                string str = $"当前取消全部订阅 event name = {item.Key}    count = {item.Value.Count}";
                str += "\n{";
                foreach (var value in item.Value)
                {
                    str += $"\n\t event desc = {value.Value.tDesc} ";
                }
                str += "\n}";
                LogWarning(str);
                #endregion
            }
            m_DicEvent = new();
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
                        $"\n\t\tevent layer \t= {item.Value.tDesc}" +
                        $"\n\t\tevent action \t= {item.Key}" +
                        $"\n\t}}";
                }
                str += $"\n}}";
            }
            LogWarning(str);
        }
    }
}
