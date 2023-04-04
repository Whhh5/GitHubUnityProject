using NodeGraph;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NpcNode : NodeBase<NpcNode, NpcNodeData>, INode
{
    public readonly Vector2 _defaultNodeSize = new Vector2(150,200);

    public string NpcName;
    public int NpcLevel;
    
    public override void OnCreated(NodeGraphView view)
    {
        if (view == null) return;
        title = NodeName;
        
        var textField = new TextField("Npc Name");
        textField.RegisterValueChangedCallback(evt =>
        {
            NpcName = evt.newValue;
        });
        mainContainer.Add(textField);
            
        var intField = new IntegerField("Level");
        intField.RegisterValueChangedCallback(evt =>
        {
            NpcLevel = evt.newValue;
        });
        mainContainer.Add(intField);

        var inputPort = GeneratePort(this, Direction.Input, typeof(string), Port.Capacity.Multi);
        inputPort.portName = "Input";
        inputContainer.Add(inputPort);
        
        var outputPort = GeneratePort(this, Direction.Output, typeof(string));
        outputPort.portName = "Output";
        outputContainer.Add(outputPort);
        
        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(new Vector2(0,0),_defaultNodeSize));
    }
}