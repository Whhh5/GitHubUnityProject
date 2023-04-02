using System.Collections;
using System.Collections.Generic;
using B1.Event;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace B1
{
    public class MonoBase : MonoBehaviour, ILog
    {
        public void Log<T>(T message) => Debug.Log($" 【 {GetType()} 】\n{message}");
        public void LogWarning<T>(T message) => Debug.LogWarning($" 【 {GetType()} 】\n{message}");
        public void LogError<T>(T message) => Debug.LogError($" 【 {GetType()} 】\n{message}");

        public async UniTask DelayAsync(int f_DelayTime = 0)
        {
            await UniTask.Delay(f_DelayTime);
        }


        private Dictionary<EEvent, List<(object tUserdata, string tDesc)>> m_MsgDic = null;
        protected virtual void Awake()
        {
            Awa_Msg();
        }
        protected virtual void OnDestroy()
        {
            Des_Msg();
        }

        #region 消息系统处理
        private void Awa_Msg()
        {
            //消息接口处理
            var eventSystem = this as IMessageSystem;
            if (!object.ReferenceEquals(eventSystem, null))
            {
                m_MsgDic = eventSystem.SubscribeList();
                foreach (var item in m_MsgDic)
                {
                    var tempItem = item;
                    foreach (var msg in tempItem.Value)
                    {
                        MessagingSystem.Instance.Subscribe(tempItem.Key, eventSystem, msg.tUserdata, msg.tDesc);
                    }
                }
            }
        }
        private void Des_Msg()
        {
            //消息接口处理
            if (!object.ReferenceEquals(m_MsgDic, null))
            {
                var eventSystem = this as IMessageSystem;
                foreach (var item in m_MsgDic)
                {
                    var tempItem = item;
                    MessagingSystem.Instance.Unsubscribe(tempItem.Key, eventSystem);
                }
            }
        } 
        #endregion
    }
}
