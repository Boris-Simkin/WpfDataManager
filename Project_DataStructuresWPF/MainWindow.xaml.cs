using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Project_DataStructures
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyDB db = new MyDB();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClearTextBoxes()
        {
            customerIdBox.Text = "";
            companyNameBox.Text = "";
            contactNameBox.Text = "";
            phoneNumberBox.Text = "";
        }

        private void RefreshGrid()
        {
            dataGrid.ItemsSource = db.ToList();
            CollectionViewSource.GetDefaultView(dataGrid.ItemsSource).Refresh();
            dataGrid.Columns[0].Visibility = Visibility.Hidden;
        }

        private void loadFromSQLBtn_Click(object sender, RoutedEventArgs e)
        {
            db.Insert(LoadFromSQL.Load());
            RefreshGrid();
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var column = e.Column as DataGridBoundColumn;

                if (column != null)
                {
                    //the name of the column
                    var bindingPath = (column.Binding as Binding).Path.Path;
                    //the value of the edited element
                    var newValue = (e.EditingElement as TextBox).Text;

                    DataGridCellInfo selected = dataGrid.SelectedCells[0];
                    Customer customer = selected.Item as Customer;

                    if (bindingPath == "CompanyName")
                        customer.CompanyName = newValue;

                    if (bindingPath == "ContactName")
                        customer.ContactName = newValue;

                    if (bindingPath == "Phone")
                        customer.Phone = newValue;

                    db.Update(customer);
                    //int rowIndex = e.Row.GetIndex();
                    // rowIndex has the row index
                    debugTxtBlk.Text = customer.CustomerID;
                }

            }

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int count = dataGrid.SelectedItems.Count;
            if (count == 1)
            {
                DataGridCellInfo selected = dataGrid.SelectedCells[0];
                Customer customer = selected.Item as Customer;
                customerIdBox.Text = customer.CustomerID;
                companyNameBox.Text = customer.CompanyName;
                contactNameBox.Text = customer.ContactName;
                phoneNumberBox.Text = customer.Phone;
            }
        }

        private bool DeleteSelected()
        {
            int count = dataGrid.SelectedItems.Count;
            MessageBoxResult result = MessageBoxResult.None;

            if (count > 1)
            {
                result = MessageBox.Show(
                $"About to delete {count} selected rows.\n\nProceed?",
                "Delete selection",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);
            }

            if (result != MessageBoxResult.No)
            {
                var items = dataGrid.SelectedItems;
                db.MultipleDelete(items);
                return true;
            }

            return false;
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                e.Handled = !DeleteSelected();
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteSelected())
            {
                ClearTextBoxes();
                RefreshGrid();
            }
        }

        private void insertBtn_Click(object sender, RoutedEventArgs e)
        {

            if (!db.ContainsCustomerID(customerIdBox.Text))
            {
                db.Insert(new Customer(customerIdBox.Text, companyNameBox.Text, contactNameBox.Text,
                    phoneNumberBox.Text));
                RefreshGrid();
            }
            else
                MessageBox.Show($"Customer with the ID: '{customerIdBox.Text}' is already exist in the table.",
                    "ID in use", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBoxes();
        }


    }
}
