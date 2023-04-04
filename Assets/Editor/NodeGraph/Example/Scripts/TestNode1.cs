using NodeGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TestNode1Data : NodeBaseData
{
    public string content;

    public TestNode1Data()
    {
        NodeName = "TestNode1";
    }
}

public class TestNode1 : NodeBase<TestNode1, TestNode1Data>, INode
{
    public readonly Vector2 _defaultNodeSize = new Vector2(150, 200);


    public override void OnCreated(NodeGraphView view)
    {
        if (view == null) return;
        title = NodeName;

        var textField = new TextField("Content", -1, true, false, default);
        textField.RegisterValueChangedCallback(evt =>
        {
            data.content = evt.newValue;
        });
        textField.SetValueWithoutNotify(name);
        textField.value = $"{data?.content}";
        mainContainer.Add(textField);

        var inputPort = GeneratePort(this, Direction.Input, typeof(string), Port.Capacity.Multi);
        inputPort.portName = "Input";
        inputPort.portType = typeof(int);
        inputContainer.Add(inputPort);

        var inputPort2 = GeneratePort(this, Direction.Input, typeof(string), Port.Capacity.Multi);
        inputPort2.portName = "Input2";
        inputPort2.portType = typeof(string);
        inputContainer.Add(inputPort2);

        var inputPort3 = GeneratePort(this, Direction.Input, typeof(string), Port.Capacity.Multi);
        inputPort3.portName = "Input2";
        inputPort3.portType = typeof(GameObject);
        inputContainer.Add(inputPort3);

        var outputPort = GeneratePort(this, Direction.Output, typeof(string), Port.Capacity.Multi);
        outputPort.portName = "Output";
        outputContainer.Add(outputPort);

        var outputPort2 = GeneratePort(this, Direction.Output, typeof(string), Port.Capacity.Multi);
        outputPort2.portName = "Output";
        outputPort2.portType = typeof(string);
        outputContainer.Add(outputPort2);


        var outputPort3 = GeneratePort(this, Direction.Output, typeof(string), Port.Capacity.Multi);
        outputPort3.portName = "Output";
        outputPort3.portType = typeof(GameObject);
        outputContainer.Add(outputPort3);



        var classList = inputContainer.GetClasses();


        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(new Vector2(0, 0), _defaultNodeSize));
    }
}
