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
        //MyDB table = new MyDB();
        //MyLinkedList<MyDB> dbList = new MyLinkedList<MyDB>();
        Dictionary<string, MyDB> tableList = new Dictionary<string, MyDB>();
        

        //string currentTable = "new table";

        public MainWindow()
        {
            InitializeComponent();
            //Init();
            //Init();
            //TabItem ti = tableTabControl.SelectedItem as TabItem;

            //dg.ItemsSource = currentTable.ToList();

            // tableList.Add(GetCurrentTableName(), new MyDB());
        }

        private void Init()
        {
            //TabItem ti = (TabItem)tableTabControl.Items[1];
            TabItem ti = tableTabControl.SelectedItem as TabItem;
            DataGrid newDG = new DataGrid();
            ti.Content = newDG;

            DataGrid dg = (DataGrid)ti.Content;
            //dg.ItemsSource = currentTable.ToList();
        }

        private string GetCurrentTableName()
        {
            TabItem ti = tableTabControl.SelectedItem as TabItem;
            return ((TextBlock)ti.Header).Text;
        }

        private MyDB currentTable
        {
            get
            {
                return tableList[GetCurrentTableName()];
            }

            set
            {
                tableList[GetCurrentTableName()] = value;
            }
        }

        public DataGrid currentDataGrid
        {
            get
            {
                TabItem ti = tableTabControl.SelectedItem as TabItem;
                DataGrid dg = (DataGrid)ti.Content;
                return dg;
            }
            set
            {
                TabItem ti = tableTabControl.SelectedItem as TabItem;
                DataGrid dg = (DataGrid)ti.Content;
                dg = value;
            }
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
            currentDataGrid.ItemsSource = currentTable.ToList();//table.ToList();


            CollectionViewSource.GetDefaultView(currentDataGrid.ItemsSource).Refresh();
            currentDataGrid.Columns[0].Visibility = Visibility.Hidden;
        }

        private void loadFromSQLBtn_Click(object sender, RoutedEventArgs e)
        {
            currentTable.Insert(LoadFromSQL.Load());
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

                    DataGridCellInfo selected = currentDataGrid.SelectedCells[0];
                    Customer customer = selected.Item as Customer;

                    if (bindingPath == "CompanyName")
                        customer.CompanyName = newValue;

                    if (bindingPath == "ContactName")
                        customer.ContactName = newValue;

                    if (bindingPath == "Phone")
                        customer.Phone = newValue;

                    currentTable.Update(customer);
                    //int rowIndex = e.Row.GetIndex();
                    // rowIndex has the row index
                }

            }

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int count = currentDataGrid.SelectedItems.Count;
            if (count == 1)
            {
                DataGridCellInfo selected = currentDataGrid.SelectedCells[0];
                Customer customer = selected.Item as Customer;
                customerIdBox.Text = customer.CustomerID;
                companyNameBox.Text = customer.CompanyName;
                contactNameBox.Text = customer.ContactName;
                phoneNumberBox.Text = customer.Phone;
            }
            if (count > 1)
            {
                ClearTextBoxes();
            }
        }

        private bool DeleteSelected()
        {
            int count = currentDataGrid.SelectedItems.Count;
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
                var items = currentDataGrid.SelectedItems;
                currentTable.MultipleDelete(items);
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

            if (!currentTable.ContainsCustomerID(customerIdBox.Text))
            {
                currentTable.Insert(new Customer(customerIdBox.Text, companyNameBox.Text, contactNameBox.Text,
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

        private void newTableBtn_Click(object sender, RoutedEventArgs e)
        {
            NewTableTab newTableTab = new NewTableTab();
            if (newTableTab.ShowDialog() == true)
            {
                CreateNewTable(newTableTab.TableName);
            }
        }

        private void CreateNewTable(string tableName)
        {
            DataGrid newDG = new DataGrid();
            TabItem newItem = new TabItem
            {
                Content = newDG,
                Header = new TextBlock { Text = tableName }
            };
            tableTabControl.Items.Add(newItem);
            newItem.IsSelected = true;



            //tableList.Add(tableTabControl.Items.Count, new MyDB());
            tableList.Add(GetCurrentTableName(), new MyDB());
            //var a = tableList[GetCurrentTableName()];
            //tableTabControl.Items[3]
            TabItem ti = tableTabControl.SelectedItem as TabItem;
            DataGrid dg = (DataGrid)ti.Content;
            dg.ItemsSource = currentTable.ToList();
            //ctrl.
            //ti.Content

            //tableTabControl.Items[0] = newItem;
        }

        private void tableTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //currentDataGrid.SelectionChanged += dataGrid_SelectionChanged;

            //TabItem item = sender as TabItem;
            //if (item != null)
            //{
            //    TabItem ti = tableTabControl.SelectedItem as TabItem;
            //    MessageBox.Show("This is " + ti.Header + " tab");

            //}
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Init();
        }
    }
}
