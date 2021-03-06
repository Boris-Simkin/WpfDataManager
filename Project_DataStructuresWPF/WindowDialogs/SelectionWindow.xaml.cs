﻿using System;
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

        MyDB _db;

        internal MyDB ResultDB { get; private set; }

        protected virtual bool QuerySubmitted
        {
            get { return submitBtn.IsEnabled; }
            set { submitBtn.IsEnabled = value; }
        }

        internal SelectionWindow(MyDB db)
        {
            InitializeComponent();
            submitBtn.Content = "Select";
            window.Title = "Select";
            _db = db;
        }

        #region Events
        private void cancleBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void submitBtn_Click(object sender, RoutedEventArgs e)
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
            QuerySubmitted = SatisfyFields();
            PrintMessage();
        }

        private void fields_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && QuerySubmitted)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void FieldsTextChanged(object sender, TextChangedEventArgs e)
        {
            QuerySubmitted = SatisfyFields();
            PrintMessage();
        }
        #endregion

        private bool DetailsFieldsEmpty()
        {
            return companyNameBox.Text == "" && contactNameBox.Text == "" && phoneNumberBox.Text == "";
        }

        private void PrintMessage()
        {
            if (ResultDB == null) return;

            if (!SelectedNotEmpty())
                messageTextBlock.Text = "";

            if (!QuerySubmitted)
            {
                if (ResultDB.IsEmpty() && SelectedNotEmpty())
                    messageTextBlock.Text = "No results containing all your search terms were found.";
            }
            else
            {
                if (ResultDB.Count == 1)
                    messageTextBlock.Text = "One row was found.";
                else
                    messageTextBlock.Text = $"{ResultDB.Count} rows were found.";
            }

        }

        private bool SelectedNotEmpty()
        {
            return ((companyNameBox.IsEnabled && !DetailsFieldsEmpty())
              || (!companyNameBox.IsEnabled && customerIdBox.Text != ""));
        }

        private bool SatisfyFields()
        {
            if (SelectedNotEmpty())
            {
                Customer userInput;
                if (companyNameBox.IsEnabled)
                    userInput = new Customer("", companyNameBox.Text, contactNameBox.Text, phoneNumberBox.Text);
                else
                    userInput = new Customer(customerIdBox.Text, "", "", "");

                ResultDB = _db.Select(userInput);
                return !ResultDB.IsEmpty();
            }
            return false;
        }
    }
}
