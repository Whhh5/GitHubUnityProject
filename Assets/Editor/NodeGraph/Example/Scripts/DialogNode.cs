using System;
using NodeGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogNode : NodeBase<DialogNode, DialogNodeData>, INode
{
    public readonly Vector2 _defaultNodeSize = new Vector2(150,200);
    public string content;
    public override void OnCreated(NodeGraphView view)
    {
        if (view == null) return;
        title = NodeName;
        
        var textField = new TextField("Content", -1, true, false, default);
        textField.RegisterValueChangedCallback(evt =>
        {
            content = evt.newValue;
        });
        textField.SetValueWithoutNotify(name);
        mainContainer.Add(textField);

        var inputPort = GeneratePort(this, Direction.Input, typeof(string), Port.Capacity.Multi);
        inputPort.portName = "Input";
        inputContainer.Add(inputPort);
        
        var outputPort = GeneratePort(this, Direction.Output, typeof(string));
        outputPort.portName = "Output";
        outputPort.portType = typeof(int);
        outputContainer.Add(outputPort);
        
        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(new Vector2(0,0),_defaultNodeSize));
    }
}
