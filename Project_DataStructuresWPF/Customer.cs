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
            //
            CustomerID = customerID;
            CompanyName = companyName;
            ContactName = contactName;
            //ContactTitle = contactTitle;
            //Address = address;
            //City = city;
            //Region = region;
            //PostalCode = postalCode;
            //Country = country;
            Phone = phone;
           // Index = index;
            //Fax = fax;
        }

        //public Link<Customer> Index { get; set; }
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        //public string ContactTitle { get; set; }
        //public string Address { get; set; }
        //public string City { get; set; }
        //public string Region { get; set; }
        //public string PostalCode { get; set; }
        //public string Country { get; set; }
        public string Phone { get; set; }
        //public string Fax { get; set; }
    }
}
