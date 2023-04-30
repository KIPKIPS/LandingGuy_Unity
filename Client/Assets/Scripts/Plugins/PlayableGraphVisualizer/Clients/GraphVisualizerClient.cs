using System.Collections.Generic;
using UnityEngine.Playables;

public class GraphVisualizerClient {
    private static GraphVisualizerClient s_Instance;
    private List<PlayableGraph> m_Graphs = new List<PlayableGraph>();
    public static GraphVisualizerClient instance => s_Instance ??= new GraphVisualizerClient();
    ~GraphVisualizerClient() {
        m_Graphs.Clear();
    }
    public static void Show(PlayableGraph graph) {
        if (!instance.m_Graphs.Contains(graph)) {
            instance.m_Graphs.Add(graph);
        }
    }
    public static void Hide(PlayableGraph graph) {
        if (instance.m_Graphs.Contains(graph)) {
            instance.m_Graphs.Remove(graph);
        }
    }
    public static IEnumerable<PlayableGraph> GetGraphs() {
        return instance.m_Graphs;
    }
}