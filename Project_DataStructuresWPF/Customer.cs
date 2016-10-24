using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_DataStructures
{
    class Customer
    {
        public Customer(string customerID, string companyName, string contactName,
      string phone)
        {
            CustomerID = customerID;
            CompanyName = companyName;
            ContactName = contactName;
            Phone = phone;
        }

        public string CustomerID { get; private set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
    }
}
