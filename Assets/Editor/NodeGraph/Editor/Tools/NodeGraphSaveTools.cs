using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph
{
    public static class NodeGraphSaveTools
    {

        #region Save
        
        public static void Save(NodeGraphWindow window)
        {
            var view = window.NodeGraphView;
            var edges = view.edges.ToList();
            var nodes = view.nodes.ToList();
            
            var so = window.Data;
            so.ClearAllNodeDatas();
            
            var runtimeAssembly = typeof(NodeGraph).Assembly;
            
            //保存node
            foreach (var node in nodes)
            {
                var data = NodeToData(node, runtimeAssembly, out var dataTypeName);
                so.AddNodeToListFromTypeName(dataTypeName, data);
            }
            
            //保存link
            var connectedPorts = edges?.Where(x=>x.input.node!=null)?.ToArray();
            if (connectedPorts != null)
            {
                for (int i = 0; i < connectedPorts.Count(); i++)
                {
                    var outputNode = connectedPorts[i].output.node;
                    var inputNode = connectedPorts[i].input.node;

                    var outputNodeType = outputNode.GetType();
                    var outputGuidField = outputNodeType.GetField("GUID");
                    var targetNodeNodeType = inputNode.GetType();
                    var targetGuidField = targetNodeNodeType.GetField("GUID");

                    so.Links.Add(new NodeLinkData
                    {
                        BaseNodeGUID = $"{outputGuidField.GetValue(outputNode)}",
                        OutputPortName = connectedPorts[i].output.portName,
                        TargetNodeGUID = $"{targetGuidField.GetValue(inputNode)}",
                        TargetPortName = connectedPorts[i].input.portName
                    });
                }
            }

            EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static object NodeToData(Node node, Assembly runtimeAssembly, out string dataTypeName)
        {
            var nodeType = node.GetType();
            var nodeNameField = nodeType.GetField("NodeName");
            var nodeGuidField = nodeType.GetField("GUID");
            if (IsStringFieldValid(nodeNameField) && IsStringFieldValid(nodeGuidField))
            {
                string nodeTypeName = nodeNameField.GetValue(node).ToString();
                dataTypeName = $"{nodeTypeName}Data";
                var data = runtimeAssembly.CreateInstance(dataTypeName);

                var originalData = node.GetType().GetField("data", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(node);

                CopyData(originalData, data);

                FillPosition(node, data);
                return data;
            }

            dataTypeName = string.Empty;
            return null;
        }

        private static bool IsStringFieldValid(FieldInfo field)
        {
            return field != null && field.FieldType.Name.StartsWith("String");
        }

        private static void FillPosition(Node node, object data)
        {
            //position
            var dataSourceType = data.GetType();
            var dataPositionField = dataSourceType.GetField("Position");
            var rect = node.GetPosition();
            dataPositionField.SetValue(data, rect);
        }
        #endregion

        #region Load

        public static void Load(NodeGraphWindow window, NodeGraph data)
        {
            var view = window.NodeGraphView;
            
            //加载节点
            var nodeDatas = data.GetAllNodeDatas();
            foreach (var nodeData in nodeDatas)
            {
                var node = LoadNode(nodeData, nodeData.NodeDataName, view);
                view.AddElement((GraphElement) node);
            }

            //加载link
            var nodes = view.nodes.ToList();
            nodes.ForEach(node =>
            {
                var nodeType = node.GetType();
                var nodeGuidField = nodeType.GetField("GUID");
                var guid = $"{nodeGuidField.GetValue(node)}";
                var connections = data.Links?.Where(x => String.Equals(x.BaseNodeGUID, guid))?.ToList();
                if (connections != null)
                {
                    connections.ForEach(connection =>
                    {
                        var targetNodeGuid = connection.TargetNodeGUID;
                        var targetNode = nodes.First(x =>
                        {
                            var xType = x.GetType();
                            var xGuidField = xType.GetField("GUID");
                            var xGuid = $"{xGuidField.GetValue(x)}";
                            return String.Equals(xGuid, targetNodeGuid);
                        });
                        var outputPort = node.outputContainer.Query<Port>().ToList().First(p =>
                            p.portName.Equals(connection.OutputPortName));
                        var targetPort = targetNode.inputContainer.Query<Port>().ToList().First(p =>
                            p.portName.Equals(connection.TargetPortName));
                        LinkNodes(view, outputPort, targetPort);
                    });
                }
            });
        }
        
        private static void LinkNodes(GraphView view, Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            view.Add(tempEdge);
        }

        private static object LoadNode(object data,string dataTypeName, NodeGraphView view)
        {
            var editorAssembly = typeof(NodeBase<,>).Assembly;
            var node = editorAssembly.CreateInstance(((NodeBaseData) data).NodeName);


            var data2 = node.GetType().GetField("data", BindingFlags.NonPublic | BindingFlags.Instance);
            data2.SetValue(node, data);

            var nodeType = node.GetType();
            var onCreated = nodeType.GetMethod("OnCreated");
            onCreated?.Invoke(node, new[] {view});
            SetPosition((Node)node, data);
            return node;
        }

        private static void SetPosition(Node node, object data)
        {
            var dataSourceType = data.GetType();
            var dataPositionField = dataSourceType.GetField("Position");
            var rect = dataPositionField.GetValue(data);
            node.SetPosition((Rect)rect);
        }
        
        #endregion
        
        private static void CopyData(object dataSource, object dataTarget)
        {
            var dataSourceType = dataSource.GetType();
            var dataTargetType = dataTarget.GetType();
            var dataSourceFields = dataSourceType.GetFields().ToDictionary<FieldInfo, string>(p => p.Name);
            var dataTargetFields = dataTargetType.GetFields().ToDictionary<FieldInfo, string>(p => p.Name);



            //var field = dataTargetType.BaseType.GetField("data", BindingFlags.NonPublic | BindingFlags.Instance);
            //field.SetValue(dataTarget, dataSource);


            foreach (var sourceField in dataSourceFields)
            {
                if (dataTargetFields.ContainsKey(sourceField.Key))
                {
                    dataTargetFields[sourceField.Key].SetValue(dataTarget, sourceField.Value.GetValue(dataSource));
                }
            }
        }
    }
}