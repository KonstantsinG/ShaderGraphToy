using ShaderGraph.ComponentModel.Implementation.NodeComponents;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Controls
{
    /// <summary>
    /// Логика взаимодействия для InscriptionComponent.xaml
    /// </summary>
    public partial class InscriptionComponent : UserControl
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(InscriptionComponentData), typeof(InscriptionComponent), new PropertyMetadata(null));

        public InscriptionComponentData Model
        {
            get => (InscriptionComponentData)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        } 

        public InscriptionComponent()
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
    }
}
