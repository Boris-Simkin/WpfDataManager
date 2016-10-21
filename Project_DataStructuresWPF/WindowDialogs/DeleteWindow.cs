using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_DataStructures
{
    class DeleteWindow : SelectionWindow
    {
        internal DeleteWindow(MyDB db) : base(db)
        {
            submitBtn.Content = "Delete";
            window.Title = "Delete";
        }
    }
}
