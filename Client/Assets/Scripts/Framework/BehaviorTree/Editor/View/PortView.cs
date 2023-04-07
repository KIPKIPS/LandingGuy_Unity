// author:KIPKIPS
// date:2023.04.06 22:45
// describe:
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PortView : Port {
    private PortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity, System.Type type) : base(portOrientation, portDirection, portCapacity, type) {
        style.height = 25;
    }
    public new static PortView Create<TEdge>(Orientation orientation, Direction direction, Capacity capacity, System.Type type) where TEdge : Edge, new() {
        var listener = new DefaultEdgeConnectorListener();
        var port = new PortView(orientation, direction, capacity, type) {
            m_EdgeConnector = new EdgeConnector<TEdge>(listener)
        };
        port.AddManipulator(port.m_EdgeConnector);
        var portConnector = port.Q("connector");
        portConnector.style.borderTopLeftRadius = portConnector.style.borderTopRightRadius = portConnector.style.borderBottomLeftRadius = portConnector.style.borderBottomRightRadius = 0;
        var portCap = portConnector.Q("cap");
        portCap.style.borderTopLeftRadius = portCap.style.borderTopRightRadius = portCap.style.borderBottomLeftRadius = portCap.style.borderBottomRightRadius = 0;
        port.m_ConnectorText.style.height = 15;
        return port;
    }
    private class DefaultEdgeConnectorListener : IEdgeConnectorListener {
        private readonly GraphViewChange _mGraphViewChange;
        private readonly List<Edge> _mEdgesToCreate;
        private readonly List<GraphElement> _mEdgesToDelete;
        public DefaultEdgeConnectorListener() {
            _mEdgesToCreate = new List<Edge>();
            _mEdgesToDelete = new List<GraphElement>();
            _mGraphViewChange.edgesToCreate = _mEdgesToCreate;
        }
        public void OnDropOutsidePort(Edge edge, Vector2 position) {
        }
        public void OnDrop(GraphView graphView, Edge edge) {
            _mEdgesToCreate.Clear();
            _mEdgesToCreate.Add(edge);
            _mEdgesToDelete.Clear();
            if (edge.input.capacity == Capacity.Single) {
                foreach (var connection in edge.input.connections) {
                    if (connection != edge) {
                        _mEdgesToDelete.Add(connection);
                    }
                }
            }
            if (edge.output.capacity == Capacity.Single) {
                foreach (var connection2 in edge.output.connections) {
                    if (connection2 != edge) {
                        _mEdgesToDelete.Add(connection2);
                    }
                }
            }
            if (_mEdgesToDelete.Count > 0) {
                graphView.DeleteElements(_mEdgesToDelete);
            }
            var edgesToCreate = _mEdgesToCreate;
            if (graphView.graphViewChanged != null) {
                edgesToCreate = graphView.graphViewChanged(_mGraphViewChange).edgesToCreate;
            }
            foreach (var item in edgesToCreate) {
                graphView.AddElement(item);
                edge.input.Connect(item);
                edge.output.Connect(item);
            }
        }
    }
}