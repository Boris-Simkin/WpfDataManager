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
        Dictionary<string, MyDB> tableList = new Dictionary<string, MyDB>();

        //count tabs created by selection
        static byte selectionTabCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            CreateNewTable("new table");
        }

        private MyDB CurrentTable
        {
            get
            {
                TabItem ti = tableTabControl.SelectedItem as TabItem;
                return tableList[ti.Header.ToString()];
            }

            set
            {
                TabItem ti = tableTabControl.SelectedItem as TabItem;
                tableList[ti.Header.ToString()] = value;
            }
        }

        private DataGrid currentDataGrid
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

        //private void ClearTextBoxes()
        //{
        //    customerIdBox.Text = "";
        //    companyNameBox.Text = "";
        //    contactNameBox.Text = "";
        //    phoneNumberBox.Text = "";
        //}

        private void RefreshGrid()
        {
            currentDataGrid.ItemsSource = CurrentTable.ToList();
            CollectionViewSource.GetDefaultView(currentDataGrid.ItemsSource).Refresh();

        }

        private void loadFromSQLBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentTable.Insert(LoadFromSQL.Load());
            RefreshGrid();
        }

        #region dataGridEvents
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

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
                    string selectedId = ((Customer)selected.Item).CustomerID;
                    Customer customer = new Customer(selectedId, "", "", "");

                    if (bindingPath == "CompanyName")
                        customer.CompanyName = newValue;

                    if (bindingPath == "ContactName")
                        customer.ContactName = newValue;

                    if (bindingPath == "Phone")
                        customer.Phone = newValue;

                    CurrentTable.Update(customer);

                }

            }

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DataGrid dataGrid = sender as DataGrid;

            //int count = dataGrid.SelectedItems.Count;
            //if (count == 1)
            //{
            //    DataGridCellInfo selected = dataGrid.SelectedCells[0];
            //    Customer customer = selected.Item as Customer;
            //    customerIdBox.Text = customer.CustomerID;
            //    companyNameBox.Text = customer.CompanyName;
            //    contactNameBox.Text = customer.ContactName;
            //    phoneNumberBox.Text = customer.Phone;
            //}
            //if (count > 1)
            //{
            //    ClearTextBoxes();
            //}
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                e.Handled = !DeleteSelected();
        }
        #endregion

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
                CurrentTable.MultipleDelete(items);
                return true;
            }

            return false;
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            //if (DeleteSelected())
            //{
            //    ClearTextBoxes();
            //    RefreshGrid();
            //}
        }

        private void insertBtn_Click(object sender, RoutedEventArgs e)
        {

            //if (!CurrentTable.ContainsCustomerID(customerIdBox.Text))
            //{
            //    CurrentTable.Insert(new Customer(customerIdBox.Text, companyNameBox.Text, contactNameBox.Text,
            //        phoneNumberBox.Text));
            //    RefreshGrid();
            //}
            //else
            //    MessageBox.Show($"Customer with the ID: '{customerIdBox.Text}' is already exist in the table.",
            //        "ID in use", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //private void clearBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    ClearTextBoxes(); 
        //}

        private void newTableBtn_Click(object sender, RoutedEventArgs e)
        {
            NewTableTab newTableTab = new NewTableTab();
            if (newTableTab.ShowDialog() == true)
                CreateNewTable(newTableTab.TableName);
        }

        private void CreateNewTable(string tableName)
        {
            //creating new DataGrid
            DataGrid newDG = new DataGrid();

            //creating new MyDB table
            MyDB newTable = new MyDB();

            //adding this table to the created DataGrid
            newDG.ItemsSource = newTable.ToList();

            //adding this table to tables dictionary 
            tableList.Add(tableName, newTable);

            //subscribing the created DataGrid to the events
            newDG.SelectionChanged += dataGrid_SelectionChanged;
            newDG.CellEditEnding += dataGrid_CellEditEnding;
            newDG.PreviewKeyDown += dataGrid_PreviewKeyDown;

            //creating new TabItem
            TabItem newItem = new TabItem
            {
                //the content of the TabItem is the created DataGrid
                Content = newDG,
                Header = tableName
            };
            //adding the TabItem to the tabControl
            tableTabControl.Items.Add(newItem);
            //select the new tab
            newItem.IsSelected = true;
        }

        private void selectBtn_Click(object sender, RoutedEventArgs e)
        {


            int count = currentDataGrid.SelectedItems.Count;
            if (count > 0)
            {
                SelectionWindow selectionWindow = new SelectionWindow();
                //if (new SelectionWindow().ShowDialog() == true)
                if (selectionWindow.ShowDialog() == true)
                {

                    var items = currentDataGrid.SelectedItems;
                    CreateNewTable("selected items " + ++selectionTabCount);

                    foreach (Customer item in items)
                        CurrentTable.Insert(item);

                    RefreshGrid();

                }



            }
        }

    }
}
