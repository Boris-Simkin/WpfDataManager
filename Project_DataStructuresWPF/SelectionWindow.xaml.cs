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
    /// Interaction logic for SelectionWindow.xaml
    /// </summary>
    public partial class SelectionWindow : Window
    {
        public SelectionWindow()
        {
            InitializeComponent();
        }

        #region Events
        private void cancleBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void selectBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void switchRadioButtons(object sender, RoutedEventArgs e)
        {
            companyNameBox.IsEnabled ^= true;
            contactNameBox.IsEnabled ^= true;
            phoneNumberBox.IsEnabled ^= true;
            customerIdBox.IsEnabled ^= true;
            selectBtn.IsEnabled = satisfyFields();
        }

        private void FieldsTextChanged(object sender, TextChangedEventArgs e)
        {
            selectBtn.IsEnabled = satisfyFields();
        }
        #endregion

        private void fields_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && selectBtn.IsEnabled)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private bool satisfyFields()
        {
            return (companyNameBox.IsEnabled && (companyNameBox.Text != "" || contactNameBox.Text != "" || phoneNumberBox.Text != ""))
                || (!companyNameBox.IsEnabled && customerIdBox.Text != "");
        }
    }
}
