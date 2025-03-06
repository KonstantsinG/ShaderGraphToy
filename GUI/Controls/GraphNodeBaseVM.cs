﻿using GUI.Utilities;
using ShaderGraph.Assemblers;
using ShaderGraph.ComponentModel.Info;
using ShaderGraph.ComponentModel.Implementation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace GUI.Controls
{
    public class GraphNodeBaseVM : INotifyPropertyChanged
    {
        private GraphNodeTypeContentInfo? _contentModel;
        public GraphNodeTypeContentInfo? ContentModel
        {
            get => _contentModel;
            set
            {
                _contentModel = value;
                OnPropertyChanged(nameof(ContentModel));
            }
        }

        private GraphNodeTypeInfo? _nodeModel;
        public GraphNodeTypeInfo? NodeModel
        {
            get => _nodeModel;
            set
            {
                _nodeModel = value;
                OnPropertyChanged(nameof(NodeModel));
            }
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

        private int _selectedOperationIndex = -1;
        public int SelectedOperationIndex
        {
            get => _selectedOperationIndex;
            set
            {
                _selectedOperationIndex = value;
                OnPropertyChanged(nameof(SelectedOperationIndex));
            }
        }

        private int _selectedSubOperationIndex = -1;
        public int SelectedSubOperationIndex
        {
            get => _selectedSubOperationIndex;
            set
            {
                _selectedSubOperationIndex = value;
                OnPropertyChanged(nameof(SelectedSubOperationIndex));
            }
        }

        public ObservableCollection<GraphNodeOperationInfo> NodeOperations { get; set; } = [];
        public ObservableCollection<GraphNodeSubOperationInfo> NodeSubOperations { get; set; } = [];
        public ObservableCollection<UserControl> NodeComponents { get; set; } = [];


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

        public void OperationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        public void SubOperationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem is GraphNodeSubOperationInfo item)
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

        public void LoadNodeContent(int id)
        {
            var compsData = GraphNodesAssembler.Instance.GetTypeContentInfo(id);
            ContentModel = compsData!;

            var comps = GraphComponentsFactory.ConstructComponents(compsData!.Components);
            NodeComponents.Clear();
            foreach (var comp in comps) NodeComponents.Add(comp);
            IsConnectorsVisible = true;
        }

        public void ConstructNode(int nodeId)
        {
            string strId = nodeId.ToString();
            int idFirstPart = int.Parse(strId[0].ToString());

            GraphNodeTypeInfo info = GraphNodesAssembler.Instance.GetTypeInfo(idFirstPart)!;
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

                if (strId.Length == 2)
                {
                    SelectedOperationIndex = int.Parse(strId[1].ToString()) - 1;
                }
                else if (strId.Length == 3)
                {
                    SelectedOperationIndex = int.Parse(strId[1].ToString()) - 1;
                    SelectedSubOperationIndex = int.Parse(strId[2].ToString()) - 1;
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
