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
        CustomerFields customerFields = new CustomerFields();

        internal UpdateWindow(MyDB db) : base(db)
        {
            submitBtn.Content = "Update";
            window.Title = "Update";
            window.Height += 100;

            customerFields.TextChanged += FieldsTextChanged;
            customerFields.EnterPressed += CustomerFields_EnterPressed;
            submitBtn.Click += SubmitBtn_Click;

            stackPanel.Children.Add(customerFields.grid);

        }

        private void CustomerFields_EnterPressed(object sender, EventArgs e)
        {
            if (!customerFields.DetailsFieldsEmpty())
            {
                ApplyUpdates();
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ApplyUpdates()
        {
            foreach (var customer in ResultDB)
            {
                if (customerFields.companyNameBox.Text != "")
                    customer.CompanyName = customerFields.companyNameBox.Text;
                if (customerFields.contactNameBox.Text != "")
                    customer.ContactName = customerFields.contactNameBox.Text;
                if (customerFields.phoneNumberBox.Text != "")
                    customer.Phone = customerFields.phoneNumberBox.Text;
            }
        }

        private void SubmitBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ApplyUpdates();
        }

        private void FieldsTextChanged(object sender, EventArgs e)
        {
            submitBtn.IsEnabled = !customerFields.DetailsFieldsEmpty() && QuerySubmitted;
        }

        private bool querySubmitted;

        protected override bool QuerySubmitted
        {
            get { return querySubmitted; }
            set
            {
                querySubmitted = value;
                customerFields.companyNameBox.IsEnabled = value;
                customerFields.contactNameBox.IsEnabled = value;
                customerFields.phoneNumberBox.IsEnabled = value;
                submitBtn.IsEnabled = !customerFields.DetailsFieldsEmpty() && QuerySubmitted;
            }
        }

    }
}
