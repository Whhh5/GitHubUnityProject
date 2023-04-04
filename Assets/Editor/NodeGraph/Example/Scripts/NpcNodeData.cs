using System;
using NodeGraph;

[Serializable]
public class NpcNodeData : NodeBaseData
{
    public string NpcName;
    public int NpcLevel;

    public NpcNodeData()
    {
        NodeName = "NpcNode";
    }
}