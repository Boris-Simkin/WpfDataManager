using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        private void SetSelectedRowCount()
        {
            lblSelectedRowCount.Text = currentDataGrid.SelectedItems.Count.ToString();
        }

        private void SetRowCount()
        {
            lblRowCount.Text = currentDataGrid.Items.Count.ToString();
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


        private void RefreshGrid()
        {
            currentDataGrid.ItemsSource = CurrentTable.ToList();
            CollectionViewSource.GetDefaultView(currentDataGrid.ItemsSource).Refresh();

        }

        private void loadFromSQLBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentTable.Insert(LoadFromSQL.Load());
            //enabling Delete, Update & Select buttons
            SetButtonsActivation(true);
            RefreshGrid();
            //SetRowCount();
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
            SetSelectedRowCount();
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

        private void addNewTab_Clicked(object sender, MouseButtonEventArgs e)
        {
            var items = currentDataGrid.SelectedItems;
            NewTableTab newTableTab = new NewTableTab(items.Count > 0, new List<string>(tableList.Keys));
            if (newTableTab.ShowDialog() == true)
            {

                CreateNewTable(newTableTab.TableName);
                if (newTableTab.includeSelected.IsChecked == true)
                {
                    foreach (Customer item in items)
                        CurrentTable.Insert(item);

                    RefreshGrid();
                    SetButtonsActivation(true);
                }
            }

        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void insertBtn_Click(object sender, RoutedEventArgs e)
        {
            InsertWindow insertWindow = new InsertWindow(CurrentTable);

            if (insertWindow.ShowDialog() == true)
            {
                Customer customer = new Customer(insertWindow.customerIdBox.Text, insertWindow.companyNameBox.Text,
                    insertWindow.contactNameBox.Text, insertWindow.phoneNumberBox.Text);
                CurrentTable.Insert(customer);
                RefreshGrid();
                SetButtonsActivation(true);
            }
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

            CollectionView myCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(newDG.Items);
            ((INotifyCollectionChanged)myCollectionView).CollectionChanged += new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);

            //creating new TabItem
            TabItem newItem = new TabItem
            {
                //the content of the TabItem is the created DataGrid
                Content = newDG,
                Header = tableName
            };
            //adding the TabItem to the tabControl
            //tableTabControl.Items.Add(newItem);
            tableTabControl.Items.Insert(tableTabControl.Items.Count - 1, newItem);
            //select the new tab
            newItem.IsSelected = true;
            //tableTabControl.SelectedIndex--;

            deleteTableBtn.IsEnabled = tableList.Count > 1;
        }

        private void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetRowCount();
        }

        private void SetButtonsActivation(bool value)
        {
            deleteBtn.IsEnabled = value;
            selectBtn.IsEnabled = value;
            updateBtn.IsEnabled = value;
        }

        private void selectBtn_Click(object sender, RoutedEventArgs e)
        {
            //int count = currentDataGrid.SelectedItems.Count;
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

        private void tableTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetButtonsActivation(currentDataGrid.Items.Count > 0);
            SetSelectedRowCount();
            SetRowCount();
        }

        private void deleteTableBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentTabItem = (TabItem)tableTabControl.SelectedItem;
            string tableName = currentTabItem.Header.ToString();

            //Confirmation window
            MessageBoxResult result = MessageBoxResult.None;
            result = MessageBox.Show(
                $"About to delete the table '{tableName}'.\n\nProceed?",
                "Delete table",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (result != MessageBoxResult.No)
            {
                currentDataGrid.SelectionChanged -= dataGrid_SelectionChanged;
                currentDataGrid.CellEditEnding -= dataGrid_CellEditEnding;
                currentDataGrid.PreviewKeyDown -= dataGrid_PreviewKeyDown;

                //CollectionView myCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(currentDataGrid.Items);
                //((INotifyCollectionChanged)myCollectionView).CollectionChanged -= new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);


                //removing the database
                tableList.Remove(tableName);

                //removing the datagrid
                //tableTabControl.SelectedItem = 

           
                var itemToRemove = currentTabItem;

                //Change the selected tab position
                if (tableTabControl.SelectedIndex != 0)
                    tableTabControl.SelectedIndex--;
                else
                    tableTabControl.SelectedIndex++;
                
                //removing the tab item
                tableTabControl.Items.Remove(itemToRemove);
            
                //Activate the delete button if there is more than 1 tab
                deleteTableBtn.IsEnabled = tableList.Count > 1;
            }

        }
    }
}
