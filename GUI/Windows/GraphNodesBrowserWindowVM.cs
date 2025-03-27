using GUI.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace GUI.Windows
{
    public class GraphNodesBrowserWindowVM : INotifyPropertyChanged
    {
        //private readonly List<TreeViewerItem>? _sourceItems;

        //private ObservableCollection<TreeViewerItem> _treeItems = [];
        //public ObservableCollection<TreeViewerItem> TreeItems
        //{
        //    get => _treeItems;
        //    set
        //    {
        //        _treeItems = value;
        //        OnPropertyChanged(nameof(TreeItems));
        //    }
        //}

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

        //private TreeViewerItem? _selectedItem;

        public delegate void TreeViewerCallback(int? nodeId);
        public event TreeViewerCallback ItemCreated = delegate { };


        public GraphNodesBrowserWindowVM()
        {
            //_sourceItems = GraphComponentsFactory.GetNodeTypesInfo().ToList();
            //TreeItems = DeepCopyTreeViewerItems(_sourceItems!);
        }

        public void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //if (((TreeView)sender).SelectedItem is TreeViewerItem item)
            //{
            //    _selectedItem = item;
            //    SelectedDescription = item.Model!.Description;
            //}
            //else
            //{
            //    _selectedItem = null;
            //    SelectedDescription = string.Empty;
            //}
        }

        public void CrossRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //TreeItems = DeepCopyTreeViewerItems(_sourceItems!);
            SearchText = string.Empty;
        }

        public void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TreeViewerItem? item;
            //TreeViewerItem? subItem;
            //TreeViewerItem? subSubItem;
            //TreeItems = DeepCopyTreeViewerItems(_sourceItems!);
            //if (SearchText == string.Empty) return;

            //for (int i = 0; i < TreeItems.Count; i++)
            //{
            //    item = TreeItems[i];
            //    item.IsExpanded = true;

            //    for (int j = 0; j < item.Children.Count; j++)
            //    {
            //        subItem = item.Children[j];
            //        subItem.IsExpanded = true;

            //        for (int k = 0; k < subItem.Children.Count; k++)
            //        {
            //            subSubItem = subItem.Children[k];
            //            subSubItem.IsExpanded = true;

            //            if (!subSubItem.Model!.Synonyms.Any(s => s.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)))
            //            {
            //                subItem.Children.RemoveAt(k);
            //                k--;
            //            }
            //        }

            //        if (!subItem.Model!.Synonyms.Any(s => s.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)))
            //        {
            //            if (subItem.Children.Count > 0)
            //            {
            //                foreach (var child in subItem.Children)
            //                    item.Children.Add(child);
            //            }

            //            item.Children.RemoveAt(j);
            //            j--;
            //        }
            //    }

            //    if (!item.Model!.Synonyms.Any(s => s.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)))
            //    {
            //        if (item.Children.Count > 0)
            //        {
            //            foreach (var child in item.Children)
            //                TreeItems.Add(child);
            //        }

            //        TreeItems.RemoveAt(i);
            //        i--;
            //    }
            //}
        }

        public void SearchBoxKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat) return;

            if (e.Key == Key.Enter)
            {
                //_selectedItem ??= TreeItems.FirstOrDefault();
                //ItemCreated.Invoke(_selectedItem?.Model!.TypeId);
            }
        }

        public void TreeViewItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat) return;
            //if (((TreeViewItem)sender).Header != _selectedItem) return;

            //if (e.Key == Key.Enter)
                //ItemCreated.Invoke(_selectedItem?.Model!.TypeId);
        }

        public void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //ItemCreated.Invoke(_selectedItem?.Model!.TypeId);
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ItemCreated.Invoke(null);
        }

        //public static ObservableCollection<TreeViewerItem> DeepCopyTreeViewerItems(List<TreeViewerItem> source)
        //{
        //    ObservableCollection<TreeViewerItem> copy = [];

        //    foreach (var item in source)
        //        copy.Add(DeepCopyTreeViewerItem(item));

        //    return copy;
        //}

        //private static TreeViewerItem DeepCopyTreeViewerItem(TreeViewerItem original)
        //{
        //    TreeViewerItem copy = new ()
        //    {
        //        Model = original.Model,
        //        IsExpanded = original.IsExpanded
        //    };

        //    foreach (var child in original.Children)
        //        copy.Children.Add(DeepCopyTreeViewerItem(child));

        //    return copy;
        //}


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
