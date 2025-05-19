using Microsoft.Xaml.Behaviors;
using Nodes2Shader.Compilation.MathGraph;
using Nodes2Shader.DataTypes;
using Nodes2Shader.GraphNodesImplementation.Components;
using System.Windows;
using System.Windows.Controls;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для InputComponent.xaml
    /// </summary>
    public partial class InputComponentView : UserControl, INodeComponentView
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(InputComponent), typeof(InputComponentView), new PropertyMetadata(null));

        public InputComponent Model
        {
            get => (InputComponent)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public InputComponentView()
        {
            InitializeComponent();

            inputConnector.IsInput = true;
        }

        public NodesConnector? GetConnector()
        {
            if (Model.HasInput) return inputConnector;
            else return null;
        }

        public string GetContent() => Model.Content;
        public void SetContent(string content)
        {
            Model.Content = content;
            
        }

        public NodeEntry? GetData()
        {
            if (inputConnector.IsBusy)
            {
                return new(Model.InputType, Model.Content, NodeEntry.EntryType.Input) { Id = GetConnector()!.ConnectorId };
            }
            else
            {
                if (!DataTypesConverter.IsAnyValid(Model.Content))
                    throw new FormatException($"Value <{Model.Content}> in component ({inputConnector.NodeId}, {inputConnector.ConnectorId}) is invalid!");

                string currType = DataTypesConverter.DefineType(Model.Content);
                if (Model.HasInput)
                {
                    if (!DataTypesConverter.IsCastPossible(currType, Model.InputType))
                        throw new ArgumentException($"Value <{Model.Content}> in component ({inputConnector.NodeId}, {inputConnector.ConnectorId}) must have value of type {Model.InputType}!");
                }


                return new(currType, Model.Content, NodeEntry.EntryType.Value);
            }
        }
    }
}
