using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NodeGraph
{
    [CreateAssetMenu(fileName = "NewNodeGraph", menuName = "NodeGraph/NodeGraph")]
    public class NodeGraph : SerializedScriptableObject
    {
        [SerializeField] private List<NodeLinkData> links = new List<NodeLinkData>();
        public List<NodeLinkData> Links => links;

        public void AddNodeToListFromTypeName(string typeName, object data)
        {
            var type = Type.GetType(typeName);
            if (!m_TestDataDic.ContainsKey(type))
            {
                m_TestDataDic.Add(type, new());
            }

            m_TestDataDic[type].Add(data);



        }

        public List<NodeBaseData> GetAllNodeDatas()
        {
            var list = new List<NodeBaseData>();

            if (!m_TestDataDic.ContainsKey(typeof(StartNodeData)))
            {
                m_TestDataDic.Add(typeof(StartNodeData), new() { new StartNodeData() });
            }


            foreach (var item in m_TestDataDic)
            {
                list = list.Concat(item.Value.Select(node => (NodeBaseData)node).ToList()).ToList();
            }
            return list;
        }

        public void ClearAllNodeDatas()
        {
            m_TestDataDic.Clear();
            links.Clear();
        }



        [SerializeField] private Dictionary<Type, List<object>> m_TestDataDic = new();
    }
    
    [Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGUID;
        public string OutputPortName;
        public string TargetNodeGUID;
        public string TargetPortName;
    }


}