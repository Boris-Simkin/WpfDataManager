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
    /// Interaction logic for CustomerFields.xaml
    /// </summary>
    public partial class CustomerFields : Window
    {
        public CustomerFields()
        {
            InitializeComponent();
            this.RemoveLogicalChild(grid);
        }

        private void fields_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void FieldsTextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
