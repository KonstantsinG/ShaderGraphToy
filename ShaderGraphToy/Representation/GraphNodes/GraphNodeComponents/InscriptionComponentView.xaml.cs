using Nodes2Shader.Compilation.MathGraph;
using Nodes2Shader.DataTypes;
using Nodes2Shader.GraphNodesImplementation.Components;
using System.Windows;
using System.Windows.Controls;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для InscriptionComponent.xaml
    /// </summary>
    public partial class InscriptionComponentView : UserControl, INodeComponentView
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(InscriptionComponent), typeof(InscriptionComponentView), new PropertyMetadata(null));

        public InscriptionComponent Model
        {
            get => (InscriptionComponent)GetValue(ModelProperty);
            set
            {
                SetValue(ModelProperty, value);
                SetFormatting(Model?.Formatting!);
            }
        }

        public InscriptionComponentView()
        {
            InitializeComponent();

            inputConnector.IsInput = true;
            outputConnector.IsInput = false;
        }

        public List<NodesConnector> GetConnectors()
        {
            List<NodesConnector> conns = [];

            if (Model.HasInput) conns.Add(inputConnector);
            if (Model.HasOutput) conns.Add(outputConnector);

            return conns;
        }

        public string GetContent() => string.Empty;
        public void SetContent(string content) { }

        public NodeEntry? GetData()
        {
            NodeEntry entry = new();

            if (Model.DefaultInput == "Error" && !inputConnector.IsBusy)
                throw new ArgumentException($"Inscription component ({inputConnector.NodeId}, {inputConnector.ConnectorId}) must have a value!");

            else if ((Model.DefaultInput == "Ignore") && (!inputConnector.IsBusy && !outputConnector.IsBusy))
                return null;

            else if (inputConnector.IsBusy)
            {
                entry.Id = inputConnector.ConnectorId;
                entry.Value = "ToReveal";
                entry.Type = Model.InputType;
                entry.Behavior = NodeEntry.EntryType.Input;
            }

            else if (outputConnector.IsBusy)
            {
                entry.Id = outputConnector.ConnectorId;
                entry.Type = Model.OutputType;
                entry.Behavior = NodeEntry.EntryType.Output;
            }

            else
            {
                if (!DataTypesConverter.IsAnyValid(Model.DefaultInput))
                    throw new ArgumentException($"Value <{Model.DefaultInput}> in component ({inputConnector.NodeId}, {inputConnector.ConnectorId}) is invalid!");
                else
                {
                    entry.Id = inputConnector.ConnectorId;
                    entry.Value = Model.DefaultInput;
                    entry.Type = DataTypesConverter.DefineType(Model.DefaultInput);
                    entry.Behavior = NodeEntry.EntryType.Value;
                }
            }

            return entry;
        }

        public void SetFormatting(List<string> props)
        {
            if (props == null) return;

            foreach (string prop in props)
            {
                switch (prop)
                {
                    case "Centred":
                        tb.TextAlignment = TextAlignment.Center;
                        panel.HorizontalAlignment = HorizontalAlignment.Center;
                        break;

                    case "Bold":
                        tb.FontWeight = FontWeights.Bold;
                        break;

                    case "Left":
                        panel.HorizontalAlignment = HorizontalAlignment.Left;
                        break;

                    case "Right":
                        panel.HorizontalAlignment = HorizontalAlignment.Right;
                        break;

                    case "Margin":
                        tb.Margin = new Thickness(10,-20,10,10);
                        break;
                }
            }
        }
    }
}
