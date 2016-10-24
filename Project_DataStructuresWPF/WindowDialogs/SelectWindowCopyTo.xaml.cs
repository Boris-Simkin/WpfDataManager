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
    /// Interaction logic for CustomerFields.xaml
    /// </summary>
    public partial class SelectWindowCopyTo : Window
    {
        public SelectWindowCopyTo()
        {
            InitializeComponent();
            if (tablesComboBox.Items.Count == 0)
                checkBox.IsEnabled = false;
            this.RemoveLogicalChild(grid);
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (tablesComboBox != null)
                tablesComboBox.IsEnabled = false;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (tablesComboBox.Items.Count != 0)
                tablesComboBox.IsEnabled = true;
        }
    }
}
