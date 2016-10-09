using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_DataStructures
{
    static class LoadFromSQL
    {
        public static MyLinkedList<Customer> Load()
        {
            MyLinkedList<Customer> customersTable = new MyLinkedList<Customer>();

            string location = "Northwind.mdb";
            string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0; data source=" + location;
            string sSql = "select * from Customers";

            OleDbConnection con = new OleDbConnection(connectionString);
            OleDbCommand myCmd = new OleDbCommand(sSql, con);
            OleDbDataAdapter adapter = new OleDbDataAdapter();//יצירת אוביקט 
            adapter.SelectCommand = myCmd;// command קישור ל 
            DataSet dataset = new DataSet();// יצירת טבלה בזיכרון
            adapter.Fill(dataset, "tblusers"); //מילוי הטבלה ומתן שם
            dataset.Tables["tblusers"].PrimaryKey = new DataColumn[] { dataset.Tables["tblusers"].Columns["UserID"] };

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                customersTable.Insert(new Customer(row[0].ToString(), row[1].ToString(),
                    row[2].ToString(), row[9].ToString()));
            }

            return customersTable;
        }
    }
}
