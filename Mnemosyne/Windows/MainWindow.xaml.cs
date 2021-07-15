using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mnemosyne
{
    public partial class MainWindow : Window
    {
        private CipherFile _file;

        private bool _uncheck_all_items;

        private static DependencyProperty _InitialMinWidthProperty = DependencyProperty.Register("_InitialWidth", typeof(double), typeof(MainWindow));
        private static DependencyProperty _InitialMinHeightProperty = DependencyProperty.Register("_InitialHeight", typeof(double), typeof(MainWindow));

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
            _uncheck_all_items = true;

            _ReloadListView();
            _SelectAll(true);
            _SelectAll(false);

            ((INotifyCollectionChanged)_lvw_Credentials.Items).CollectionChanged += new NotifyCollectionChangedEventHandler(_CredentialItems_CollectionChanged);
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

        private void _SelectAll(bool select = true)
        {
            foreach (CredentialItem item in _lvw_Credentials.Items)
            {
                if (select)
                {
                    _lvw_Credentials.SelectedItems.Add(item);
                }
                else
                {
                    _lvw_Credentials.SelectedItems.Remove(item);
                }
            }
        }

        private void _OpenCredentialWindow(Credential credential = null)
        {
            if ((bool)new CredentialWindow(_file, credential).ShowDialog())
            {
                _ReloadListView();
            }
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
            foreach (var item in _lvw_Credentials.SelectedItems)
            {
                _file.Data.Credentials.Remove((item as CredentialItem).Credential);
            }
            _lvw_Credentials.SelectedItems.Clear();
            _file.Save();
            _ReloadListView();
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
            _SelectAll(true);
        }

        private void _chk_All_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_uncheck_all_items)
            {
                _SelectAll(false);
            }
        }

        private void _lvw_Credentials_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _UpdateControls();

            _uncheck_all_items = false;
            _chk_All.IsChecked = _lvw_Credentials.SelectedItems.Count == _lvw_Credentials.Items.Count;
            _uncheck_all_items = true;
        }

        private void _CredentialItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _chk_All.IsChecked = false;
        }

        private void _lvi_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            _OpenCredentialWindow(((CredentialItem)_lvw_Credentials.SelectedItem).Credential);
        }
    }
}
