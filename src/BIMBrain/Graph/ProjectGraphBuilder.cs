using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain
{
    public class ProjectGraphBuilder
    {
        private readonly Document _doc;
        private long _syntheticId = -1000;

        public ProjectGraphBuilder(Document doc)
        {
            _doc = doc;
        }

        private ElementId NextSyntheticId() => new ElementId(_syntheticId--);

        public ProjectGraph Build()
        {
            var graph = new ProjectGraph();

            try
            {
                if (_doc == null) return graph;

                var projectName = _doc.Title ?? "Projeto";
                var projectNode = new GraphNode(NextSyntheticId(), "Projeto", projectName, projectName);
                graph.AddNode(projectNode);

                var ctx = new ModelContext(_doc);

                var documents = new List<DocumentInfo>();
                if (ctx.MainDocument?.Document != null)
                    documents.Add(ctx.MainDocument);
                foreach (var link in ctx.Links)
                    if (link.IsLoaded && link.Document != null)
                        documents.Add(link);

                foreach (var docInfo in documents)
                {
                    BuildModel(graph, projectNode, docInfo);
                }
            }
            catch
            {
                // Nunca lançar exceções: retorna o grafo parcial já construído.
            }

            return graph;
        }

        private void BuildModel(ProjectGraph graph, GraphNode projectNode, DocumentInfo docInfo)
        {
            var modelName = docInfo.Name;
            var modelNode = new GraphNode(NextSyntheticId(), "Modelo", modelName, modelName);
            graph.AddNode(modelNode);
            graph.AddEdge(new GraphEdge(projectNode.ElementId, modelNode.ElementId, GraphRelation.Contains));

            var modelDoc = docInfo.Document;
            if (modelDoc == null) return;

            var levelNodes = BuildLevels(graph, modelNode, modelDoc, modelName);
            var levelById = levelNodes.ToDictionary(n => n.ElementId, n => n);

            BuildPanelsAndCircuits(graph, modelNode, modelDoc, modelName, levelById);
        }

        private static IReadOnlyList<GraphNode> BuildLevels(
            ProjectGraph graph, GraphNode modelNode, Document modelDoc, string modelName)
        {
            var levels = new FilteredElementCollector(modelDoc)
                .OfClass(typeof(Level))
                .ToElements()
                .Cast<Level>()
                .ToList();

            var nodes = new List<GraphNode>();
            foreach (var level in levels)
            {
                var levelNode = new GraphNode(level.Id, "Nível", level.Name, modelName);
                graph.AddNode(levelNode);
                graph.AddEdge(new GraphEdge(modelNode.ElementId, levelNode.ElementId, GraphRelation.Contains));
                nodes.Add(levelNode);
            }

            return nodes;
        }

        private static void BuildPanelsAndCircuits(
            ProjectGraph graph,
            GraphNode modelNode,
            Document modelDoc,
            string modelName,
            IReadOnlyDictionary<ElementId, GraphNode> levelById)
        {
            var panels = new PanelAnalyzer(modelDoc).Analyze();
            var panelNodesByName = new Dictionary<string, GraphNode>(StringComparer.OrdinalIgnoreCase);

            foreach (var panel in panels)
            {
                var panelNode = new GraphNode(panel.Id, "Painel", panel.Name, modelName);
                graph.AddNode(panelNode);
                panelNodesByName[panel.Name] = panelNode;

                var levelId = GetPanelLevelId(modelDoc, panel.Id);
                if (levelId != ElementId.InvalidElementId &&
                    levelById.TryGetValue(levelId, out var levelNode))
                {
                    graph.AddEdge(new GraphEdge(levelNode.ElementId, panelNode.ElementId, GraphRelation.Contains));
                }
            }

            var circuits = new ElectricalCircuitAnalyzer(modelDoc).Analyze();
            foreach (var circuit in circuits)
            {
                var circuitNode = new GraphNode(circuit.Id, "Circuito", circuit.Name, modelName);
                graph.AddNode(circuitNode);

                if (!string.IsNullOrEmpty(circuit.PanelName) &&
                    panelNodesByName.TryGetValue(circuit.PanelName, out var panelNode))
                {
                    graph.AddEdge(new GraphEdge(panelNode.ElementId, circuitNode.ElementId, GraphRelation.FedBy));
                }
            }
        }

        private static ElementId GetPanelLevelId(Document modelDoc, ElementId panelId)
        {
            var element = modelDoc.GetElement(panelId) as FamilyInstance;
            if (element == null) return ElementId.InvalidElementId;

            if (element.LevelId != null && element.LevelId != ElementId.InvalidElementId)
                return element.LevelId;

            var param = element.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
            if (param != null && param.HasValue)
                return param.AsElementId();

            return ElementId.InvalidElementId;
        }
    }
}
