using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Project_DataStructures
{
    public class UpdateWindow : SelectionWindow
    {
        internal UpdateWindow(MyDB db) : base(db)
        {
            submitBtn.Content = "Update";
            window.Title = "Update";

            window.Height += 100;

            CustomerFields customerFields = new CustomerFields();

            customerFields.companyNameBox.IsEnabled = true;
            customerFields.contactNameBox.IsEnabled = true;
            customerFields.phoneNumberBox.IsEnabled = true;
            //stackPanel.Children.Add(SaveButton);
            stackPanel.Children.Add(customerFields.grid);
            //stackPanel.
        }
    }
}
