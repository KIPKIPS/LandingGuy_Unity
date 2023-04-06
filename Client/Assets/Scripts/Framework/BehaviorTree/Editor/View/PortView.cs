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
        DefaultEdgeConnectorListener listener = new DefaultEdgeConnectorListener();
        PortView port = new PortView(orientation, direction, capacity, type) {
            m_EdgeConnector = new EdgeConnector<TEdge>(listener)
        };
        port.AddManipulator(port.m_EdgeConnector);
        var portConnector = port.Q("connector");
        portConnector.style.borderTopLeftRadius = portConnector.style.borderTopRightRadius = portConnector.style.borderBottomLeftRadius = portConnector.style.borderBottomRightRadius = 0;
        var portCap = portConnector.Q("cap");
        portCap.style.borderTopLeftRadius = portCap.style.borderTopRightRadius = portCap.style.borderBottomLeftRadius = portCap.style.borderBottomRightRadius = 0;
        return port;
    }
    private class DefaultEdgeConnectorListener : IEdgeConnectorListener {
        private readonly GraphViewChange m_GraphViewChange;
        private readonly List<Edge> m_EdgesToCreate;
        private readonly List<GraphElement> m_EdgesToDelete;
        public DefaultEdgeConnectorListener() {
            m_EdgesToCreate = new List<Edge>();
            m_EdgesToDelete = new List<GraphElement>();
            m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
        }
        public void OnDropOutsidePort(Edge edge, Vector2 position) {
        }
        public void OnDrop(GraphView graphView, Edge edge) {
            m_EdgesToCreate.Clear();
            m_EdgesToCreate.Add(edge);
            m_EdgesToDelete.Clear();
            if (edge.input.capacity == Capacity.Single) {
                foreach (Edge connection in edge.input.connections) {
                    if (connection != edge) {
                        m_EdgesToDelete.Add(connection);
                    }
                }
            }
            if (edge.output.capacity == Capacity.Single) {
                foreach (Edge connection2 in edge.output.connections) {
                    if (connection2 != edge) {
                        m_EdgesToDelete.Add(connection2);
                    }
                }
            }
            if (m_EdgesToDelete.Count > 0) {
                graphView.DeleteElements(m_EdgesToDelete);
            }
            List<Edge> edgesToCreate = m_EdgesToCreate;
            if (graphView.graphViewChanged != null) {
                edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
            }
            foreach (Edge item in edgesToCreate) {
                graphView.AddElement(item);
                edge.input.Connect(item);
                edge.output.Connect(item);
            }
        }
    }
}