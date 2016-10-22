using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_DataStructures
{
    public class SelectWindow : SelectionWindow
    {
        SelectWindowCopyTo selectWindowCopyTo = new SelectWindowCopyTo();

        internal SelectWindow(MyDB db, List<string> keyList) : base(db)
        {
            window.Height += 50;
            stackPanel.Children.Add(selectWindowCopyTo.grid);
            selectWindowCopyTo.tablesComboBox.ItemsSource = keyList;
        }

        public string SelectedTable
        {
            get { return (string)selectWindowCopyTo.tablesComboBox.SelectedValue; }
        }

        public bool newTable
        {
            get { return !selectWindowCopyTo.tablesComboBox.IsEnabled; }
        }

        private bool querySubmitted;

        protected override bool QuerySubmitted
        {
            get { return querySubmitted; }
            set
            {
                querySubmitted = value;
                submitBtn.IsEnabled = QuerySubmitted;
                selectWindowCopyTo.checkBox.IsEnabled = value;
                selectWindowCopyTo.tablesComboBox.IsEnabled = value;

                if (selectWindowCopyTo.checkBox.IsChecked == true)
                    selectWindowCopyTo.tablesComboBox.IsEnabled = false;
                if (selectWindowCopyTo.checkBox.IsChecked == false && value)
                    selectWindowCopyTo.tablesComboBox.IsEnabled = true;
            }
        }
    }
}
