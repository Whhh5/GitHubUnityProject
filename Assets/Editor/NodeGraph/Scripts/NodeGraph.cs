using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeGraph
{
    [CreateAssetMenu(fileName = "NewNodeGraph", menuName = "NodeGraph/NodeGraph")]
    public class NodeGraph : ScriptableObject
    {
        [SerializeField] private List<StartNodeData> _startNodeDatas = new List<StartNodeData>()
        {
            new StartNodeData()
        };
        [SerializeField] private List<DialogNodeData> _dialogNodeDatas = new List<DialogNodeData>();
        [SerializeField] private List<NpcNodeData> _npcNodeDatas = new List<NpcNodeData>();
        
        [SerializeField] private List<NodeLinkData> links = new List<NodeLinkData>();
        public List<NodeLinkData> Links => links;

        public void AddNodeToListFromTypeName(string typeName, object data)
        {
            if (String.Equals(typeName,typeof(StartNodeData).FullName))
            {
                _startNodeDatas.Add((StartNodeData) data);
            }
            else if (String.Equals(typeName,typeof(DialogNodeData).FullName))
            {
                _dialogNodeDatas.Add((DialogNodeData) data);
            }
            else if (String.Equals(typeName, typeof(NpcNodeData).FullName))
            {
                _npcNodeDatas.Add((NpcNodeData) data);
            }
            else
            {
                throw new Exception($"Node Data Type Name : {typeName} is not defined");
            }
        }

        public List<NodeBaseData> GetAllNodeDatas()
        {
            var list = new List<NodeBaseData>();
            list = list.Concat(_startNodeDatas.Select<StartNodeData, NodeBaseData>(node => (NodeBaseData) node).ToList()).ToList();
            list = list.Concat(_dialogNodeDatas.Select<DialogNodeData, NodeBaseData>(node => (NodeBaseData)node).ToList()).ToList();
            list = list.Concat(_npcNodeDatas.Select<NpcNodeData, NodeBaseData>(node => (NodeBaseData) node).ToList()).ToList();
            
            return list;
        }

        public void ClearAllNodeDatas()
        {
            _startNodeDatas.Clear();
            _dialogNodeDatas.Clear();
            _npcNodeDatas.Clear();
            links.Clear();
        }
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