using Newtonsoft.Json;
using ShaderGraphToy.Representation.GraphNodes;
using System.IO;

namespace ShaderGraphToy.Utilities.Serializers
{
    internal static class GraphSerializer
    {
        private static readonly JsonSerializerSettings _settings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Error
        };


        public static void Serialize(List<GraphNodeBase> nodes, string path)
        {
            List<NodeModel> nModels = []; List<ConnectionModel> cModels = [];
            NodeModel nModel; ConnectionModel cModel;

            foreach (GraphNodeBase nb in nodes)
            {
                nModel = new()
                {
                    Id = nb.NodeId,
                    TypeId = (int)nb.NodeSubTypeId,
                    TranslateX = nb.GetTranslate().X,
                    TranslateY = nb.GetTranslate().Y,
                    Contents = nb.GetContents()
                };
                nModels.Add(nModel);

                foreach (NodesConnector nc in nb.GetAllConnectors())
                {
                    if (!nc.IsBusy) continue;

                    if (nc.IsInput)
                    {
                        cModel = new()
                        {
                            InputNode = nc.NodeId,
                            OutputNode = nc.ConnectedNodesIds[0],
                            InputConnector = nc.ConnectorId,
                            OutputConnector = nc.ConnectedConnectorsIds[0]
                        };
                        if (cModels.All(cm => cm != cModel)) cModels.Add(cModel);
                    }
                    else
                    {
                        for (int i = 0; i < nc.ConnectedNodesIds.Count; i++)
                        {
                            cModel = new()
                            {
                                InputNode = nc.ConnectedNodesIds[i],
                                OutputNode = nc.NodeId,
                                InputConnector = nc.ConnectedConnectorsIds[i],
                                OutputConnector = nc.ConnectorId
                            };
                            if (cModels.All(cm => cm != cModel)) cModels.Add(cModel);
                        }
                    }
                }
            }

            GraphModel graph = new()
            {
                Nodes = nModels,
                Connections = cModels
            };
            string json = JsonConvert.SerializeObject(graph);
            File.WriteAllText(path, json);
        }

        public static GraphModel Deserialize(string filePath)
        {

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<GraphModel>(json, _settings)
                ?? throw new FileFormatException("Unable to load graph, source file is corrupted.");
        }
    }
}
