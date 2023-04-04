using System;
using UnityEditor.Experimental.GraphView;

namespace NodeGraph
{
    public abstract class NodeBase<TClass, TData> : Node
        where TData:new()
    {
        public string GUID;
        public string NodeName;

        protected TData data;
        public NodeBase()
        {
            NodeName = typeof(TClass).ToString();
            GUID = System.Guid.NewGuid().ToString();
            data = new();
        }

        public abstract void OnCreated(NodeGraphView view);

        protected Port GeneratePort(Node node, Direction portDir, Type type, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDir, capacity, type);
        }
    }
}