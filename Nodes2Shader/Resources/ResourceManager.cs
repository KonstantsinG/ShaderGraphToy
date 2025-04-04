﻿using System.Reflection;

namespace Nodes2Shader.Resources
{
    internal static class ResourceManager
    {
        // Resource paths
        private static readonly string _gnTypesInfoPath = "Nodes2Shader.Resources.GraphNodes_TypesInfo.gnTypes_";
        private static readonly string _gnContentsPath = "Nodes2Shader.Resources.GraphNodes_Contents.gnContents_";


        // Resource getters
        internal static string GetGrahNodesTypesInfoResource(string lang) => ReadFileFromResources($"{_gnTypesInfoPath}{lang}.json");
        internal static string GetGrahNodesContentsResource(string lang) => ReadFileFromResources($"{_gnContentsPath}{lang}.json");


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
