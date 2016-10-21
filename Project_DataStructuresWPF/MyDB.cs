using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.OleDb;
using System.Data;
using System.Reflection;
using System.Linq.Expressions;

namespace Project_DataStructures
{
    class MyDB : IEnumerable<Customer>
    {
        [Flags]
        enum CustomerField
        {
            CustomerID = 1,
            CompanyName = 2,
            ContactName = 4,
            Phone = 8,
            All = ~0
        };

        MyDoublyLinkedList<Customer> _customersTable = new MyDoublyLinkedList<Customer>();

        //Dictionaries for each field
        //The dictionary (except the CustomerID) holds as value a Linked list of links to the table
        Dictionary<string, Link<Customer>> dictCustomerID = new Dictionary<string, Link<Customer>>();
        Dictionary<string, MyDoublyLinkedList<Link<Customer>>> dictCompanyName = new Dictionary<string, MyDoublyLinkedList<Link<Customer>>>();
        Dictionary<string, MyDoublyLinkedList<Link<Customer>>> dictContactName = new Dictionary<string, MyDoublyLinkedList<Link<Customer>>>();
        Dictionary<string, MyDoublyLinkedList<Link<Customer>>> dictPhone = new Dictionary<string, MyDoublyLinkedList<Link<Customer>>>();

        public MyDB() { }

        /// <summary>
        /// The constructor receive a linked list of customers
        /// </summary>
        /// <param name="customersTable"></param>
        public MyDB(MyDoublyLinkedList<Customer> customersTable)
        {
            Insert(customersTable);
        }

        /// <summary>
        /// Returns whether the table is empty
        /// </summary>
        public bool IsEmpty()
        {
            return _customersTable.IsEmpty();
        }

        public bool ContainsCustomerID(string id)
        {
            return dictCustomerID.ContainsKey(id);
        }

        public bool ContainsCompanyName(string companyName)
        {
            return dictCompanyName.ContainsKey(companyName);
        }

        public bool ContainsContactName(string contactName)
        {
            return dictContactName.ContainsKey(contactName);
        }

        public bool ContainsPhone(string phone)
        {
            return dictPhone.ContainsKey(phone);
        }

        /// <summary>
        /// returns the number of customers
        /// </summary>
        public int Count
        {
            get { return _customersTable.Count; }
        }

        /// <summary>
        /// Select customers by combination of specifying fields
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public MyDB Select(Customer customer)
        {
            MyDB newDB = new MyDB();
            newDB = SelectBy(customer.CustomerID, CustomerField.CustomerID);
            if (!newDB.IsEmpty())
                return newDB;

            if (customer.CompanyName != "")
            {
                newDB = SelectBy(customer.CompanyName, CustomerField.CompanyName);
                if (newDB.IsEmpty()) return newDB;
            }

            if (customer.ContactName != "")
            {
                if (!newDB.IsEmpty())
                    newDB = newDB.SelectBy(customer.ContactName, CustomerField.ContactName);
                else
                    newDB = SelectBy(customer.ContactName, CustomerField.ContactName);
                if (newDB.IsEmpty()) return newDB;
            }

            if (customer.Phone != "")
            {
                if (!newDB.IsEmpty())
                    newDB = newDB.SelectBy(customer.Phone, CustomerField.Phone);
                else
                    newDB = SelectBy(customer.Phone, CustomerField.Phone);
                if (newDB.IsEmpty()) return newDB;
            }

            return newDB;
        }

        //Select Customers only by single field
        //and return them as MyDB
        private MyDB SelectBy(string field, CustomerField selectBy)
        {
            MyDoublyLinkedList<Link<Customer>> newList = new MyDoublyLinkedList<Link<Customer>>();
            MyDB result = new MyDB();

            if (selectBy == CustomerField.CustomerID && field != "")
                if (dictCustomerID.ContainsKey(field))
                {
                    var line = dictCustomerID[field].Data;
                    result.Insert(line);
                    return result;
                }

            if (selectBy == CustomerField.CompanyName && field != "")
                if (dictCompanyName.ContainsKey(field))
                {
                    foreach (var link in dictCompanyName[field])
                    {
                        var line = link.Data;
                        result.Insert(line);
                    }
                }

            if (selectBy == CustomerField.ContactName && field != "")
                if (dictContactName.ContainsKey(field))
                {
                    foreach (var link in dictContactName[field])
                    {
                        var line = link.Data;
                        result.Insert(line);
                    }
                }

            if (selectBy == CustomerField.Phone && field != "")
                if (dictPhone.ContainsKey(field))
                {
                    foreach (var link in dictPhone[field])
                    {
                        var line = link.Data;
                        result.Insert(line);
                    }
                }
            return result;
        }

        void AddToDictionaries(Link<Customer> link, CustomerField field)
        {
            if (field.HasFlag(CustomerField.CustomerID))
                dictCustomerID.Add(link.Data.CustomerID, link);

            if (field.HasFlag(CustomerField.CompanyName))
                if (!dictCompanyName.ContainsKey(link.Data.CompanyName))
                    dictCompanyName.Add(link.Data.CompanyName, new MyDoublyLinkedList<Link<Customer>>(link));
                else dictCompanyName[link.Data.CompanyName].Insert(link);

            if (field.HasFlag(CustomerField.ContactName))
                if (!dictContactName.ContainsKey(link.Data.ContactName))
                    dictContactName.Add(link.Data.ContactName, new MyDoublyLinkedList<Link<Customer>>(link));
                else dictContactName[link.Data.ContactName].Insert(link);

            if (field.HasFlag(CustomerField.Phone))
                if (!dictPhone.ContainsKey(link.Data.Phone))
                    dictPhone.Add(link.Data.Phone, new MyDoublyLinkedList<Link<Customer>>(link));
                else dictPhone[link.Data.Phone].Insert(link);
        }
        
        void RemoveFromDictionaries(Link<Customer> link, CustomerField field)
        {
            if (field.HasFlag(CustomerField.CustomerID))
                dictCustomerID.Remove(link.Data.CustomerID);

            if (field.HasFlag(CustomerField.CompanyName))
            {
                dictCompanyName[link.Data.CompanyName].Remove(link);
                if (dictCompanyName[link.Data.CompanyName].IsEmpty()) dictCompanyName.Remove(link.Data.CompanyName);
            }

            if (field.HasFlag(CustomerField.ContactName))
            {
                dictContactName[link.Data.ContactName].Remove(link);
                if (dictContactName[link.Data.ContactName].IsEmpty()) dictContactName.Remove(link.Data.ContactName);
            }

            if (field.HasFlag(CustomerField.Phone))
            {
                dictPhone[link.Data.Phone].Remove(link);
                if (dictPhone[link.Data.Phone].IsEmpty()) dictPhone.Remove(link.Data.Phone);
            }
        }

        /// <summary>
        /// Insert Customers as MyDoublyLinkedList
        /// and return true if some of customers ID were reserved
        /// </summary>
        /// <param name="customersTable">MyDoublyLinkedList<Customer></param>
        /// <returns></returns>
        public bool Insert(MyDoublyLinkedList<Customer> customersTable)
        {
            bool reservedID = false;
            foreach (var customer in customersTable)
            {
                if (!dictCustomerID.ContainsKey(customer.CustomerID))
                {
                    _customersTable.Insert(customer);
                    AddToDictionaries(_customersTable.Head, CustomerField.All);
                }
                else reservedID = true;
            }
            return reservedID;
        }

        /// <summary>
        /// Insert another MyDB into this instance of MyDB.
        /// </summary>
        /// <param name="db">MyDB</param>
        public void Insert(MyDB db)
        {
            Insert(db._customersTable);
        }
        
        /// <summary>
        /// Insert a single customer
        /// </summary>
        /// <param name="customer"></param>
        public void Insert(Customer customer)
        {
            Insert(new MyDoublyLinkedList<Customer>(new Customer(customer.CustomerID, customer.CompanyName,
                        customer.ContactName, customer.Phone)));
        }

        /// <summary>
        /// Removes customer from the table by it's ID
        /// </summary>
        /// <param name="id">CustomerID</param>
        public void Delete(string id)
        {
            if (!dictCustomerID.ContainsKey(id.ToString()))
                throw new ArgumentException($"Customer with the ID: '{id}' does not exist.");

            var link = dictCustomerID[id];
            RemoveFromDictionaries(link, CustomerField.All);
            _customersTable.RemovePos(link);
        }

        /// <summary>
        /// Delete Customers by given list of Customers.
        /// </summary>
        /// <param name="db">IEnumerable Database that stores 'Customer's</param>
        public void MultipleDelete(IEnumerable db)
        {
            foreach (var item in db)
                Delete(((Customer)item).CustomerID);
        }

        /// <summary>
        /// Update Customers by given list of Customers.
        /// </summary>
        /// <param name="db">the table is MyDB</param>
        public void Update(MyDB db)
        {
            Update(db._customersTable);
        }

        /// <summary>
        /// Update Customers by given list of Customers.
        /// </summary>
        /// <param name="customersTable">the table is MyDoublyLinkedList</param>
        public void Update(MyDoublyLinkedList<Customer> customersTable)
        {
            foreach (var customer in customersTable)
                UpdateByID(customer);
        }

        /// <summary>
        /// Updates only one row by given ID and fields to update
        /// </summary>
        /// <param name="customer"></param>
        public void UpdateByID(Customer customer)
        {
            if (customer.CustomerID == "")
                throw new ArgumentException("The customer ID is empty.");
            if (!dictCustomerID.ContainsKey(customer.CustomerID))
                throw new ArgumentException($"Customer with the ID: '{customer.CustomerID}' is not exist.");

            var link = dictCustomerID[customer.CustomerID];

            if (link.Data.CompanyName != customer.CompanyName && customer.CompanyName != "")
            {
                RemoveFromDictionaries(link, CustomerField.CompanyName);
                link.Data.CompanyName = customer.CompanyName;
                AddToDictionaries(link, CustomerField.CompanyName);
            }

            if (link.Data.ContactName != customer.ContactName && customer.ContactName != "")
            {
                RemoveFromDictionaries(link, CustomerField.ContactName);
                link.Data.ContactName = customer.ContactName;
                AddToDictionaries(link, CustomerField.ContactName);
            }

            if (link.Data.Phone != customer.Phone && customer.Phone != "")
            {
                RemoveFromDictionaries(link, CustomerField.Phone);
                link.Data.Phone = customer.Phone;
                AddToDictionaries(link, CustomerField.Phone);
            }
        }

        /// <summary>
        /// Convert MyDB to Microsoft's List
        /// </summary>
        /// <returns></returns>
        public List<Customer> ToList()
        {
            List<Customer> list = new List<Customer>();
            foreach (var customer in this)
                list.Add(customer);

            return list;
        }

        public IEnumerator<Customer> GetEnumerator()
        {
            Link<Customer> link = _customersTable.Head;
            while (link != null)
            {
                yield return link.Data;
                link = link.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
