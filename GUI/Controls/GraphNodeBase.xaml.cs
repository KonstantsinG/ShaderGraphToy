using GUI.Controls.GraphNodeComponents;
using GUI.Utilities;
using ShaderGraph.Assemblers;
using ShaderGraph.ComponentModel.Implementation;
using ShaderGraph.ComponentModel.Info;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace GUI.Controls
{
    /// <summary>
    /// Логика взаимодействия для GraphNodeBase.xaml
    /// </summary>
    public partial class GraphNodeBase : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ContentModelProperty = DependencyProperty.Register(
            nameof(ContentModel), typeof(GraphNodeTypeContentInfo), typeof(GraphNodeBase), new PropertyMetadata(null));

        public GraphNodeTypeContentInfo? ContentModel
        {
            get => (GraphNodeTypeContentInfo?)GetValue(ContentModelProperty);
            set => SetValue(ContentModelProperty, value);
        }

        public static readonly DependencyProperty NodeModelProperty = DependencyProperty.Register(
            nameof(NodeModel), typeof(GraphNodeTypeInfo), typeof(GraphNodeBase), new PropertyMetadata(null));

        public GraphNodeTypeInfo? NodeModel
        {
            get => (GraphNodeTypeInfo?)GetValue(NodeModelProperty);
            set => SetValue(NodeModelProperty, value);
        }

        private bool _isConnectorsVisible = false;
        public bool IsConnectorsVisible
        {
            get => _isConnectorsVisible;
            set
            {
                _isConnectorsVisible = value;
                OnPropertyChanged(nameof(IsConnectorsVisible));
            }
        }


        public ObservableCollection<GraphNodeOperationInfo> NodeOperations { get; set; } = [];
        public ObservableCollection<GraphNodeSubOperationInfo> NodeSubOperations { get; set; } = [];
        public ObservableCollection<UserControl> NodeComponents { get; set; } = [];


        private bool _isTaken = false;
        private Point _mouseOffset;


        public GraphNodeBase()
        {
            InitializeComponent();
        }


        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            borderRect.Fill = (Brush)FindResource("HighlightBlue");
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            borderRect.Fill = (Brush)FindResource("Gray_00");
        }

        private void HeaderPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isTaken = true;
            _mouseOffset = e.GetPosition(this);
            headerPanel.CaptureMouse();
        }

        private void HeaderPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isTaken = false;
            headerPanel.ReleaseMouseCapture();
        }

        private void HeaderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isTaken)
            {
                Point currentPosition;

                if (Parent is Canvas)
                {
                    currentPosition = e.GetPosition(Parent as Canvas);

                    Canvas.SetLeft(this, currentPosition.X - _mouseOffset.X);
                    Canvas.SetTop(this, currentPosition.Y - _mouseOffset.Y);
                }
                else
                {
                    if (((UserControl)Parent).Parent is Canvas)
                    {
                        currentPosition = e.GetPosition(((UserControl)Parent).Parent as Canvas);

                        Canvas.SetLeft((UserControl)Parent, currentPosition.X - _mouseOffset.X);
                        Canvas.SetTop((UserControl)Parent, currentPosition.Y - _mouseOffset.Y);
                    }
                }
            }
        }



        public void LoadNodeTypeData(string typeName)
        {
            GraphNodeTypeInfo info = GraphNodesAssembler.Instance.GetTypeInfo(typeName)!;
            NodeModel = info;

            if (!info.UsingOperations)
            {
                int id = info.OperationsTypes[0].SubTypes[0].TypeId;
                LoadNodeContent(id);
            }
            else
            {
                NodeOperations.Clear();
                foreach (var op in info.OperationsTypes) NodeOperations.Add(op);
            }
        }

        private void OperationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!NodeModel!.UsingSubOperations)
            {
                int id = (((ComboBox)sender).SelectedItem as GraphNodeOperationInfo)!.SubTypes[0].TypeId;
                LoadNodeContent(id);
            }
            else
            {
                NodeSubOperations.Clear();
                var subs = (((ComboBox)sender).SelectedItem as GraphNodeOperationInfo)!.SubTypes;
                foreach (var sub in subs) NodeSubOperations.Add(sub);
            }
        }

        private void SubOperationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ((ComboBox)sender).SelectedItem as GraphNodeSubOperationInfo;
            if (item != null)
            {
                LoadNodeContent(item.TypeId);
            }
            else
            {
                ContentModel = null;
                NodeComponents.Clear();
                IsConnectorsVisible = false;
            }
        }

        private void LoadNodeContent(int id)
        {
            var compsData = GraphNodesAssembler.Instance.GetTypeContentInfo(id);
            ContentModel = compsData!;

            var comps = GraphComponentsFactory.ConstructComponents(compsData!.Components);
            NodeComponents.Clear();
            foreach (var comp in comps) NodeComponents.Add(comp);
            IsConnectorsVisible = true;
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
