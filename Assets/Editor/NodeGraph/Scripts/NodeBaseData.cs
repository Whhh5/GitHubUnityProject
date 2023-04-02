using System;
using UnityEngine;

namespace NodeGraph
{
    [Serializable]
    public abstract class NodeBaseData
    {
        public string GUID;
        public string NodeName = "NodeBase";
        public Rect Position = Rect.zero;

        public string NodeDataName
        {
            get { return $"{NodeName}Data"; }
        }
    }
}