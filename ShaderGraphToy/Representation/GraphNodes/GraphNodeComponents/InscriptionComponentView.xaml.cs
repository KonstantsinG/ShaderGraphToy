using Nodes2Shader.GraphNodesImplementation.Components;
using System.Windows;
using System.Windows.Controls;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для InscriptionComponent.xaml
    /// </summary>
    public partial class InscriptionComponentView : UserControl
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

        public void SetFormatting(List<string> props)
        {
            if (props == null) return;

            foreach (string prop in props)
            {
                switch (prop)
                {
                    case "Centred":
                        tb.TextAlignment = TextAlignment.Center;
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
                        tb.Margin = new Thickness(5,0,5,5);
                        break;
                }
            }
        }
    }
}
