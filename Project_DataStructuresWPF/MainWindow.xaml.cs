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
        //All the tables are stored in this dictionary
        Dictionary<string, MyDB> tableList = new Dictionary<string, MyDB>();

        //count tabs created by selection
        static byte selectionTabCount = 0;

        //enum SelectMode
        //{
        //    Select,
        //    Delete,
        //    Update
        //}

        public MainWindow()
        {
            InitializeComponent();
            CreateNewTable("new table");
        }

        #region Properties
        //Current MyDB table property
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
        #endregion

        //Statusbar selected row counter
        private void SetSelectedRowCount()
        {
            lblSelectedRowCount.Text = currentDataGrid.SelectedItems.Count.ToString();
        }

        //Statusbar total row counter
        private void SetRowCount()
        {
            // lblRowCount.Text = currentDataGrid.Items.Count.ToString();
            lblRowCount.Text = CurrentTable.Count.ToString();
        }

        private void RefreshGrid()
        {
            currentDataGrid.ItemsSource = CurrentTable.ToList();
            CollectionViewSource.GetDefaultView(currentDataGrid.ItemsSource).Refresh();
        }

        //private void Select(SelectMode selectMode)
        //{
        //    SelectionWindow selectionWindow = new SelectionWindow(CurrentTable, selectMode.ToString());
        //    //Invoke select window dialog
        //    if (selectionWindow.ShowDialog() == true)
        //    {
        //        //var items = currentDataGrid.SelectedItems;
        //        Customer customer = new Customer(selectionWindow.customerIdBox.Text, selectionWindow.companyNameBox.Text,
        //            selectionWindow.contactNameBox.Text, selectionWindow.phoneNumberBox.Text);

        //        var items = CurrentTable.Select(customer);

        //        if (!items.IsEmpty())
        //        {
        //            switch (selectMode)
        //            {
        //                case SelectMode.Delete:
        //                    CurrentTable.MultipleDelete(items);
        //                    break;

        //                case SelectMode.Select:
        //                    CreateNewTable("selected items " + ++selectionTabCount);

        //                    CurrentTable.Insert(items);

        //                    //Enabling Delete, Update &Select buttons
        //                    SetButtonsActivation(true);

        //                    break;
        //                case SelectMode.Update:
        //                    CurrentTable.Update(items);

        //                    break;
        //                 default:
        //                    break;
        //            }

        //            RefreshGrid();

        //        }
        //        else
        //            MessageBox.Show("No results containing all your search terms were found.", "Nothing found", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}

        private bool DeleteSelected()
        {
            int count = currentDataGrid.SelectedItems.Count;
            bool res = false;

            MessageBoxResult result = MessageBoxResult.None;
            //Showing confirmation window only if selected items > 1
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
                //Delete selected items from the table
                var items = currentDataGrid.SelectedItems;
                CurrentTable.MultipleDelete(items);
                res = true;
            }

            //Enabling Delete, Update &Select buttons in case the current table is not empty
            SetButtonsActivation(CurrentTable.Count > 0);

            SetRowCount();
            return res;
        }

        //The method creates new ItemTab, DataGrid and a new MyDB table
        private void CreateNewTable(string tableName)
        {
            //creating new DataGrid
            DataGrid newDG = new DataGrid();
            newDG.CanUserReorderColumns = false;
            //Adding a background to the DataGrid
            newDG.Background = new ImageBrush(imageBackground.Source);
            //Set row color to transparent
            newDG.RowBackground = new SolidColorBrush(Colors.Transparent);
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
            tableTabControl.Items.Insert(tableTabControl.Items.Count - 1, newItem);
            //select the new tab
            newItem.IsSelected = true;
            //Activate the delete button if there is more than 1 tab
            deleteTableBtn.IsEnabled = tableList.Count > 1;
        }

        private void SetButtonsActivation(bool value)
        {
            deleteBtn.IsEnabled = value;
            selectBtn.IsEnabled = value;
            updateBtn.IsEnabled = value;
        }

        private void tableTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                //Enabling Delete, Update &Select buttons in case the current table is not empty
                SetButtonsActivation(currentDataGrid.Items.Count > 0);
                SetSelectedRowCount();
                SetRowCount();
            }
        }

        #region DataGrid events
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Get the dataGrid that called the event
            DataGrid dataGrid = sender as DataGrid;

            if (e.EditAction == DataGridEditAction.Commit)
            {
                var column = e.Column as DataGridBoundColumn;

                if (column != null)
                {
                    var newValue = (e.EditingElement as TextBox).Text;

                    DataGridCellInfo selected = dataGrid.SelectedCells[0];
                    string selectedId = ((Customer)selected.Item).CustomerID;
                    Customer customer = new Customer(selectedId, "", "", "");

                    //Disabling the accesss to the CustomerID key field
                    if (column.Header.Equals("CustomerID"))
                        column.IsReadOnly = true;

                    if (column.Header.Equals("CompanyName"))
                        customer.CompanyName = newValue;

                    if (column.Header.Equals("ContactName"))
                        customer.ContactName = newValue;

                    if (column.Header.Equals("Phone"))
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
            //Delete selected items if 'delete' key was pressed
            if (e.Key == Key.Delete)
                e.Handled = !DeleteSelected();

        }
        #endregion

        #region Buttons click events
        private void insertBtn_Click(object sender, RoutedEventArgs e)
        {
            InsertWindow insertWindow = new InsertWindow(CurrentTable);
            //Showing the insert window dialog
            if (insertWindow.ShowDialog() == true)
            {
                Customer customer = new Customer(insertWindow.customerIdBox.Text, insertWindow.companyNameBox.Text,
                    insertWindow.contactNameBox.Text, insertWindow.phoneNumberBox.Text);
                //Insert the user input data
                CurrentTable.Insert(customer);
                RefreshGrid();
                //Enabling Delete, Update &Select buttons
                SetButtonsActivation(true);
                SetRowCount();
            }
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteWindow deleteWindow = new DeleteWindow(CurrentTable);
            if (deleteWindow.ShowDialog() == true)
            {
                CurrentTable.MultipleDelete(deleteWindow.ResultDB);
                //Enabling Delete, Update &Select buttons in case the current table is not empty
                SetButtonsActivation(CurrentTable.Count > 0);
                SetRowCount();
                RefreshGrid();
            }
        }

        private void selectBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectionWindow selectionWindow = new SelectionWindow(CurrentTable);
            if (selectionWindow.ShowDialog() == true)
            {


                CreateNewTable("selected items " + ++selectionTabCount);

                CurrentTable.Insert(selectionWindow.ResultDB);

                //Enabling Delete, Update &Select buttons
                SetButtonsActivation(true);
                RefreshGrid();
            }
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateWindow updateWindow = new UpdateWindow(CurrentTable);
            if (updateWindow.ShowDialog() == true)
            {
                RefreshGrid();
            }
        }

        private void loadFromSQLBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentTable.Insert(LoadFromSQL.Load());
            //Enabling Delete, Update & Select buttons 
            SetButtonsActivation(true);
            RefreshGrid();
            SetRowCount();
        }

        private void addNewTab_Clicked(object sender, MouseButtonEventArgs e)
        {
            var items = currentDataGrid.SelectedItems;
            NewTableTab newTableTab = new NewTableTab(items.Count > 0, new List<string>(tableList.Keys));
            //Showing the dialog window for new table tab creation
            if (newTableTab.ShowDialog() == true)
            {
                //Create new tab & table
                CreateNewTable(newTableTab.TableName);
                if (newTableTab.includeSelected.IsChecked == true)
                {
                    //Copying the selected items from the last tab
                    foreach (Customer item in items)
                        CurrentTable.Insert(item);
                    RefreshGrid();
                    //Enabling Delete, Update &Select buttons
                    SetButtonsActivation(true);
                }
            }
        }

        private void deleteTableBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentTabItem = (TabItem)tableTabControl.SelectedItem;
            string tableName = currentTabItem.Header.ToString();

            //Confirmation window dialog
            MessageBoxResult result = MessageBoxResult.None;
            result = MessageBox.Show(
                $"About to delete the table '{tableName}'.\n\nProceed?",
                "Delete table",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (result != MessageBoxResult.No)
            {
                //unsubscribe the datagrid from its events
                currentDataGrid.SelectionChanged -= dataGrid_SelectionChanged;
                currentDataGrid.CellEditEnding -= dataGrid_CellEditEnding;
                currentDataGrid.PreviewKeyDown -= dataGrid_PreviewKeyDown;

                //removing the database
                tableList.Remove(tableName);

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

        #endregion

    }
}
