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
        }
    }
}
