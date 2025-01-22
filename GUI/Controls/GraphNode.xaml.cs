using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI.Controls
{
    /// <summary>
    /// Логика взаимодействия для GraphNode.xaml
    /// </summary>
    public partial class GraphNode : UserControl
    {
        private StackPanel _numberPanel = null;
        private StackPanel _vec2Panel = null;
        private StackPanel _vec3Panel = null;
        private StackPanel _vec4Panel = null;
        private StackPanel _boolPanel = null;
        private StackPanel _colorPanel = null;

        private StackPanel _selectedPanel = null;


        public GraphNode()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string val = (string)( (ComboBoxItem)( ((ComboBox)sender).SelectedValue ) ).Content;
            
            switch (val)
            {
                case "Число":
                    SelectPanel(ref _numberPanel);
                    break;

                case "Вектор 2":
                    SelectPanel(ref _vec2Panel);
                    break;

                case "Вектор 3":
                    SelectPanel(ref _vec3Panel);
                    break;

                case "Вектор 4":
                    SelectPanel(ref _vec4Panel);
                    break;

                case "Логическое значение":
                    SelectPanel(ref _boolPanel);
                    break;

                case "Цвет":
                    SelectPanel(ref _colorPanel);
                    break;
            }
        }


        private void SelectPanel(ref StackPanel panel)
        {
            if (panel == null) FindAllPanels();

            _numberPanel.Visibility = Visibility.Collapsed;
            _vec2Panel.Visibility = Visibility.Collapsed;
            _vec3Panel.Visibility = Visibility.Collapsed;
            _vec4Panel.Visibility = Visibility.Collapsed;
            _boolPanel.Visibility = Visibility.Collapsed;
            _colorPanel.Visibility = Visibility.Collapsed;

            _selectedPanel = panel;
            panel.Visibility = Visibility.Visible;
        }

        private void FindAllPanels()
        {
            _numberPanel = FindPanel("numberPanel");
            _vec2Panel = FindPanel("vec2Panel");
            _vec3Panel = FindPanel("vec3Panel");
            _vec4Panel = FindPanel("vec4Panel");
            _boolPanel = FindPanel("boolPanel");
            _colorPanel = FindPanel("colorPanel");
        }

        private StackPanel FindPanel(string uid)
        {
            StackPanel outerPanel = (StackPanel)graphNode.NodeContent;

            foreach (FrameworkElement el in outerPanel.Children)
            {
                if (el is StackPanel)
                {
                    if (el.Uid == uid) return (StackPanel)el;
                }
            }

            return null;
        }
    }
}
