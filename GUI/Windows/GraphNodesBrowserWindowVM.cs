using GUI.Utilities;
using ShaderGraph.ComponentModel.Info.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace GUI.Windows
{
    public class GraphNodesBrowserWindowVM : INotifyPropertyChanged
    {
        private List<TreeViewerItem>? _sourceItems;

        public ObservableCollection<TreeViewerItem> TreeItems { get; set; } = [];

        private string _selectedDescription = string.Empty;
        public string SelectedDescription
        {
            get => _selectedDescription;
            set
            {
                _selectedDescription = value;
                OnPropertyChanged(nameof(SelectedDescription));
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }


        public GraphNodesBrowserWindowVM()
        {
            _sourceItems = GraphComponentsFactory.GetNodeTypesInfo().ToList();
            TreeItems = new ObservableCollection<TreeViewerItem>(new List<TreeViewerItem>(_sourceItems!));
        }

        public void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewerItem item = (((TreeView)sender).SelectedItem as TreeViewerItem)!;
            SelectedDescription = item.Model!.Description;
        }

        public void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TreeViewerItem? item;
            TreeViewerItem? subItem;
            TreeViewerItem? subSubItem;
            TreeItems = new ObservableCollection<TreeViewerItem>(new List<TreeViewerItem>(_sourceItems!));
            if (SearchText == string.Empty) return;

            for (int i = 0; i < TreeItems.Count; i++)
            {
                item = TreeItems[i];

                for (int j = 0; j < item.Children.Count; j++)
                {
                    subItem = item.Children[j];

                    for (int k = 0; k < subItem.Children.Count; k++)
                    {
                        subSubItem = subItem.Children[k];

                        if (!subSubItem.Model!.Synonyms.Any(s => s.StartsWith(SearchText)))
                        {
                            subItem.Children.RemoveAt(k);
                            k--;
                        }
                    }

                    if (!subItem.Model!.Synonyms.Any(s => s.StartsWith(SearchText)))
                    {
                        if (subItem.Children.Count > 0)
                        {
                            foreach (var child in subItem.Children)
                                item.Children.Add(child);
                        }

                        item.Children.RemoveAt(j);
                        j--;
                    }
                }

                if (!item.Model!.Synonyms.Any(s => s.StartsWith(SearchText)))
                {
                    if (item.Children.Count > 0)
                    {
                        foreach (var child in item.Children)
                            TreeItems.Add(child);
                    }

                    TreeItems.RemoveAt(i);
                    i--;
                }
            }
        }

        private List<TreeViewerItem> GetTreeItemsDeepCopy(List<TreeViewerItem> source)
        {
            List<TreeViewerItem> destinnation = [];
            List<TreeViewerItem> newSubItems;
            List<TreeViewerItem> newSubSubItems;

            foreach (var item in source)
            {
                newSubItems = [];

                foreach (var subItem in item.Children)
                {
                    newSubSubItems = [];

                    foreach (var subSubItem in subItem.Children)
                    {
                        newSubSubItems.Add(subSubItem);
                    }

                    //newSubItems.Add(newSubItems);
                }
            }

            return destinnation;
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
