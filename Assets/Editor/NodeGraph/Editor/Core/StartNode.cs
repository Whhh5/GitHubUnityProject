using NodeGraph;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeGraph
{
    public class StartNode :NodeBase<StartNode, StartNodeData>
    {
        public readonly Vector2 _defaultNodeSize = new Vector2(150,200);

        public override void OnCreated(NodeGraphView view)
        {
            title = "Start";
            var outputPort = GeneratePort(this, Direction.Output, typeof(string));
            outputPort.portName = "Output";
            outputContainer.Add(outputPort);
        
            RefreshExpandedState();
            RefreshPorts();
            SetPosition(new Rect(new Vector2(200,200),_defaultNodeSize));
        }
    }
}