using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mnemosyne
{
    public partial class MainWindow : Window
    {
        private static DependencyProperty _InitialMinWidthProperty = DependencyProperty.Register("_InitialWidth", typeof(double), typeof(MainWindow));
        private static DependencyProperty _InitialMinHeightProperty = DependencyProperty.Register("_InitialHeight", typeof(double), typeof(MainWindow));

        private CipherFile _file;

        private bool _uncheck_all_items_flag;
        private int _drag_drop_selected_index;
        private Point _drag_drop_start_point;

        private double _InitialWidth
        {
            set
            {
                SetValue(_InitialMinWidthProperty, value);
            }
        }
        private double _InitialHeight
        {
            set
            {
                SetValue(_InitialMinHeightProperty, value);
            }
        }

        public MainWindow(CipherFile file)
        {
            InitializeComponent();

            _file = file;
            _uncheck_all_items_flag = true;

            _drag_drop_start_point = new Point();
            _drag_drop_selected_index = -1;

            _ReloadListView();
            _SelectAll();
            _SelectAll(false);

            ((INotifyCollectionChanged)_lvw_Credentials.Items).CollectionChanged += new NotifyCollectionChangedEventHandler(_lvw_Credentials_Items_CollectionChanged);
        }

        private void _Save()
        {
            _file.Save();
            _ReloadListView();
        }

        private void _ReloadListView()
        {
            _lvw_Credentials.Items.Clear();
            foreach (Credential credential in _file.Data.Credentials)
            {
                _lvw_Credentials.Items.Add(new CredentialItem(credential));
            }

            _UpdateControls();
        }

        private void _UpdateControls()
        {
            bool one = _lvw_Credentials.SelectedItems.Count == 1,
                many = _lvw_Credentials.SelectedItems.Count > 1;

            _btn_Edit.IsEnabled      = one;
            _btn_Delete.IsEnabled    = one || many;
            _btn_Show.IsEnabled      = one || many;
            _btn_Hide.IsEnabled      = one || many;
        }

        private void _ShowSelectedPasswords(bool show)
        {
            foreach (CredentialItem item in _lvw_Credentials.SelectedItems)
            {
                item.ShowPassword = show;
            }
        }

        private void _Select(int[] indices, bool select = true)
        {
            foreach (int index in indices)
            {
                if (select)
                {
                    _lvw_Credentials.SelectedItems.Add(_lvw_Credentials.Items[index]);
                }
                else
                {
                    _lvw_Credentials.SelectedItems.Remove(_lvw_Credentials.Items[index]);
                }
            }
        }

        private void _SelectAll(bool select = true)
        {
            int[] indices = new int[_lvw_Credentials.Items.Count];

            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
            }

            _Select(indices, select);
        }

        private void _OpenCredentialWindow(Credential credential = null)
        {
            new CredentialWindow(_file, credential).ShowDialog();

            _ReloadListView();
        }

        private void _win_Main_Loaded(object sender, RoutedEventArgs e)
        {
            _InitialWidth = ActualWidth;
            _InitialHeight = ActualHeight;
        }

        private void _btn_New_Click(object sender, RoutedEventArgs e)
        {
            _OpenCredentialWindow();
        }

        private void _btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            _OpenCredentialWindow(((CredentialItem)_lvw_Credentials.SelectedItem).Credential);
        }

        private void _btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            foreach (CredentialItem item in _lvw_Credentials.SelectedItems)
            {
                _file.Data.Credentials.Remove(item.Credential);
            }
            _lvw_Credentials.SelectedItems.Clear();
            _Save();
        }

        private void _btn_Show_Click(object sender, RoutedEventArgs e)
        {
            _ShowSelectedPasswords(true);
        }

        private void _btn_Hide_Click(object sender, RoutedEventArgs e)
        {
            _ShowSelectedPasswords(false);
        }

        private void _btn_Rekey_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)new CipherWindow(_file).ShowDialog())
            {
                Close();
            }
        }

        private void _chk_All_Checked(object sender, RoutedEventArgs e)
        {
            _SelectAll();
        }

        private void _chk_All_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_uncheck_all_items_flag)
            {
                _SelectAll(false);
            }
        }

        private void _lvw_Credentials_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _UpdateControls();

            _uncheck_all_items_flag = false;
            _chk_All.IsChecked = _lvw_Credentials.SelectedItems.Count == _lvw_Credentials.Items.Count;
            _uncheck_all_items_flag = true;
        }

        private void _lvw_Credentials_Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _chk_All.IsChecked = false;
        }

        private void _lvw_Credentials_ContextMenu_CopyUsername_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(((CredentialItem)_lvw_Credentials.SelectedItem).Credential.Username);
        }

        private void _lvw_Credentials_ContextMenu_CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(((CredentialItem)_lvw_Credentials.SelectedItem).Credential.Password);
        }

        private void _lvi_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            _OpenCredentialWindow(((CredentialItem)_lvw_Credentials.SelectedItem).Credential);
        }

        /* https://www.codeproject.com/Articles/1236549/Csharp-WPF-listview-Drag-Drop-a-Custom-Item */

        private void _lvw_Credentials_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _drag_drop_start_point = e.GetPosition(null);
        }

        private void _lvw_Credentials_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Vector diff = _drag_drop_start_point - e.GetPosition(null);

                if (Math.Abs(diff.X) > 0 || Math.Abs(diff.Y) > 0)
                {
                    if (_drag_drop_selected_index == -1)
                    {
                        _drag_drop_selected_index = _lvw_Credentials.SelectedIndex;
                    }
                    else
                    {
                        _lvw_Credentials.SelectedIndex = _drag_drop_selected_index;
                    }

                    if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        DataObject drag_drop_data = new DataObject(new CredentialItem[_lvw_Credentials.SelectedItems.Count]);
                        _lvw_Credentials.SelectedItems.CopyTo((CredentialItem[])drag_drop_data.GetData(typeof(CredentialItem[])), 0);

                        DragDrop.DoDragDrop(_lvw_Credentials, drag_drop_data, DragDropEffects.Copy | DragDropEffects.Move);
                    }
                }
            }
            else
            {
                _drag_drop_selected_index = -1;
            }
        }

        private void _lvw_Credentials_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(CredentialItem[])) || sender != e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void _lvw_Credentials_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CredentialItem[])) && sender == e.Source)
            {
                int new_index;

                try
                {
                    new_index = _file.Data.Credentials.IndexOf(((CredentialItem)_lvw_Credentials.ItemContainerGenerator.ItemFromContainer(_FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource))).Credential);
                }
                catch (ArgumentException)
                {
                    new_index = _lvw_Credentials.Items.Count - 1;
                }

                CredentialItem[] dragged_items = (CredentialItem[])e.Data.GetData(typeof(CredentialItem[]));
                foreach (CredentialItem item in dragged_items)
                {
                    _file.Data.Credentials.Remove(item.Credential);
                    _file.Data.Credentials.Insert(new_index, item.Credential);
                }

                int[] new_indices = new int[dragged_items.Length];
                for (int i = 0; i < dragged_items.Length; i++)
                {
                    new_indices[i] = _file.Data.Credentials.IndexOf(dragged_items[i].Credential);
                }

                _Save();
                _Select(new_indices);
            }

            T _FindAncestor<T>(DependencyObject current) where T : DependencyObject
            {
                do
                {
                    if (current is T)
                    {
                        return (T)current;
                    }
                    current = System.Windows.Media.VisualTreeHelper.GetParent(current);
                }
                while (current != null);

                return null;
            }
        }

        /* https://stackoverflow.com/questions/665719/wpf-animate-listbox-scrollviewer-horizontaloffset */

        private void _lvw_Credentials_DragOver(object sender, DragEventArgs e)
        {
            ScrollViewer sv = FindVisualChild<ScrollViewer>(_lvw_Credentials);

            double tolerance = _lvw_Credentials.ActualHeight / 10,
                offset = 1,
                vertical_posistion = e.GetPosition(_lvw_Credentials).Y;

            if (vertical_posistion < tolerance)
            {
                sv.ScrollToVerticalOffset(sv.VerticalOffset - offset);
            }
            else if (vertical_posistion > _lvw_Credentials.ActualHeight - tolerance)
            {
                sv.ScrollToVerticalOffset(sv.VerticalOffset + offset);
            }

            T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
            {
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(obj, i);

                    if (child != null && child is T)
                        return (T)child;

                    else
                    {
                        T child_of_child = FindVisualChild<T>(child);

                        if (child_of_child != null)
                            return child_of_child;
                    }
                }

                return null;
            }
        }
    }
}
