using System.Reflection;

namespace Nodes2Shader.Resources
{
    internal static class ResourceManager
    {
        // Resource paths
        private static readonly string _gnTypesInfoPath = "Nodes2Shader.Resources.GraphNodes_TypesInfo.gnTypes";
        private static readonly string _gnContentsPath = "Nodes2Shader.Resources.GraphNodes_Contents.gnContents_";
        private static readonly string _gnExpressionsPath = "Nodes2Shader.Resources.GraphNodes_Expressions.glsl.gnExpressions_";
        private static readonly string _externalFunctionsPath = "Nodes2Shader.Resources.GraphNodes_Expressions.glsl.gnExpressions_ExternalFunctions";


        public static List<string> GraphNodesTypesIds
        {
            get => [ "1", "2", "3", "4", "5", "6" ];
        }


        // Resource getters
        internal static string GetExternalFunctionsResource() => ReadFileFromResources($"{_externalFunctionsPath}.json");
        internal static string GetGraphNodesTypesInfoResource() => ReadFileFromResources($"{_gnTypesInfoPath}.json");
        internal static string GetGraphNodesContentsResource(string id)
        {
            string name = GetTypeName(id);
            return ReadFileFromResources($"{_gnContentsPath}{name}.json");
        }
        internal static string GetGraphNodesExpressionsResource(string id)
        {
            string name = GetTypeName(id);
            return ReadFileFromResources($"{_gnExpressionsPath}{name}.json");
        }


        private static string GetTypeName(string id)
        {
            return id switch
            {
                "1" => "1Constant",
                "2" => "2InputData",
                "3" => "3OutputData",
                "4" => "4MathOperations",
                "5" => "5MathFunctions",
                "6" => "6Sampler",
                _ => string.Empty
            };
        }

        // Read resource content
        private static string ReadFileFromResources(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourcePath);
            using var reader = new StreamReader(stream!);

            return reader.ReadToEnd();
        }
    }
}
