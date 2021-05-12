using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Mnemosyne
{
    public partial class MainWindow : Window
    {
        CipherFile file;

        public List<CredentialItem> CredentialItems
        {
            get
            {
                return credentialItems;
            }
            set
            {
                credentialItems = value;
            }
        }

        private List<CredentialItem> credentialItems;
        
        public MainWindow(CipherFile file)
        {
            InitializeComponent();

            this.file = file;

            loadListView();
            initializeControls();
            selectAll(true);
            selectAll(false);
        }

        private void loadListView()
        {
            vw_credentials.Items.Clear();
            foreach (Credential credential in file.Credentials)
            {
                vw_credentials.Items.Add(new CredentialItem(credential));
            }
        }

        private void initializeControls()
        {
            bool one = vw_credentials.SelectedItems.Count == 1,
                many = vw_credentials.SelectedItems.Count > 1;

            btn_edit.IsEnabled = one;
            btn_delete.IsEnabled = one || many;
            btn_show.IsEnabled = one || many;
            btn_hide.IsEnabled = one || many;
        }

        private void showHidePasswords(bool show)
        {
            foreach (CredentialItem item in vw_credentials.SelectedItems)
            {
                item.ShowPassword = show;
            }
        }

        private void selectAll(bool select = true)
        {
            foreach (CredentialItem item in vw_credentials.Items)
            {
                if (select)
                {
                    vw_credentials.SelectedItems.Add(item);
                }
                else
                {
                    vw_credentials.SelectedItems.Remove(item);
                }
            }
        }

        private void openCredentialWindow(Credential credential = null)
        {
            if ((bool)new CredentialWindow(file, credential).ShowDialog())
            {
                loadListView();
            }
        }

        private void btn_newClick(object sender, RoutedEventArgs e)
        {
            openCredentialWindow();
        }

        private void btn_editClick(object sender, RoutedEventArgs e)
        {
            openCredentialWindow(((CredentialItem)vw_credentials.SelectedItems[0]).Credential);
        }

        private void btn_deleteClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in vw_credentials.SelectedItems)
            {
                file.Credentials.Remove((item as CredentialItem).Credential);
            }
            vw_credentials.SelectedItems.Clear();
            file.Save();
            loadListView();
        }

        private void btn_showClick(object sender, RoutedEventArgs e)
        {
            showHidePasswords(true);
        }

        private void btn_hideClick(object sender, RoutedEventArgs e)
        {
            showHidePasswords(false);
        }

        private void chk_allChecked(object sender, RoutedEventArgs e)
        {
            selectAll(true);
        }

        private void chk_allUnchecked(object sender, RoutedEventArgs e)
        {
            selectAll(false);
        }

        private void vw_selectedCredentialsChanged(object sender, SelectionChangedEventArgs e)
        {
            initializeControls();
        }
    }
}
