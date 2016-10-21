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
using System.Windows.Shapes;

namespace Project_DataStructures
{
    /// <summary>
    /// Interaction logic for NewTableTab.xaml
    /// </summary>
    public partial class NewTableTab : Window
    {
        public NewTableTab(bool itemsCheckBox, List<string> keyList)
        {
            InitializeComponent();
            includeSelected.IsChecked = itemsCheckBox;
            includeSelected.IsEnabled = itemsCheckBox;
            _keyList = keyList;
        }

        private List<string> _keyList;

        public string TableName
        {
            get
            {
                return tableNameTextBox.Text;
            }
        }

        private void createBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void cancleBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tableNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool nameSatisfy = !_keyList.Any(name => name == tableNameTextBox.Text);

            if (!nameSatisfy)
                errorMessageTextBlock.Visibility = Visibility.Visible;
            else
                errorMessageTextBlock.Visibility = Visibility.Hidden;

            createBtn.IsEnabled = tableNameTextBox.Text.Length > 0 && nameSatisfy;
        }

        private void tableNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && createBtn.IsEnabled)
            {
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
