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
    /// Interaction logic for InsertWindow.xaml
    /// </summary>
    public partial class InsertWindow : Window
    {
        internal InsertWindow(MyDB db)
        {
            InitializeComponent();
            _db = db;
        }

        MyDB _db;

        private void InsertBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void cancleBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void fields_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && InsertBtn.IsEnabled)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void CustomerIDTextChanged(object sender, TextChangedEventArgs e)
        {
            bool customerIdSatisfy = !_db.ContainsCustomerID(customerIdBox.Text);

            if (!customerIdSatisfy)
                errorMessageTextBlock.Visibility = Visibility.Visible;
            else
                errorMessageTextBlock.Visibility = Visibility.Hidden;


            InsertBtn.IsEnabled = customerIdBox.Text.Length > 0 && customerIdSatisfy;
        }
    }
}
