using System.Net.Http.Headers;
using System.Reflection;

namespace Nodes2Shader.Resources
{
    internal static class ResourceManager
    {
        // Resource paths
        private static readonly string _gnTypesInfoPath = "Nodes2Shader.Resources.GraphNodes_TypesInfo.gnTypes";
        private static readonly string _gnContentsPath = "Nodes2Shader.Resources.GraphNodes_Contents.gnContents_";


        public static List<string> GraphNodesContentsIds
        {
            get => [ "1", "2", "3", "4", "5" ];
        }


        // Resource getters
        internal static string GetGrahNodesTypesInfoResource() => ReadFileFromResources($"{_gnTypesInfoPath}.json");
        internal static string GetGrahNodesContentsResource(string id)
        {
            string name = GetContentName(id);
            return ReadFileFromResources($"{_gnContentsPath}{name}.json");
        }


        private static string GetContentName(string id)
        {
            return id switch
            {
                "1" => "1Constant",
                "2" => "2InputData",
                "3" => "3OutputData",
                "4" => "4MathOperations",
                "5" => "5MathFunctions",
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
