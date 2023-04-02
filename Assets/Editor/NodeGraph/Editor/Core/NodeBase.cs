using System;
using UnityEditor.Experimental.GraphView;

namespace NodeGraph
{
    public abstract class NodeBase<T> : Node
    {
        public string GUID;
        public string NodeName = "NodeBase";

        public NodeBase()
        {
            GUID = System.Guid.NewGuid().ToString();
            NodeName = typeof(T).FullName;
        }
        
        public abstract void OnCreated(NodeGraphView view);
        
        protected Port GeneratePort(Node node,Direction portDir,Type type, Port.Capacity capacity= Port.Capacity.Single) 
        {
            return node.InstantiatePort(Orientation.Horizontal,portDir,capacity,type);
        }
    }
}