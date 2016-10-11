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
        enum Dictionary
        {
            CustomerID = 1,
            CompanyName = 2,
            ContactName = 4,
            Phone = 8,
            All = ~0
        };

        MyLinkedList<Customer> _customersTable = new MyLinkedList<Customer>();

        Dictionary<string, Link<Customer>> byCustomerID = new Dictionary<string, Link<Customer>>();
        Dictionary<string, MyLinkedList<Link<Customer>>> byCompanyName = new Dictionary<string, MyLinkedList<Link<Customer>>>();
        Dictionary<string, MyLinkedList<Link<Customer>>> byContactName = new Dictionary<string, MyLinkedList<Link<Customer>>>();
        Dictionary<string, MyLinkedList<Link<Customer>>> byPhone = new Dictionary<string, MyLinkedList<Link<Customer>>>();

        public MyDB() { }

        public MyDB(MyLinkedList<Customer> customersTable)
        {
            Insert(customersTable);
        }

        public bool ContainsCustomerID(string id)
        {
            return byCustomerID.ContainsKey(id);
        }

        public bool ContainsCompanyName(string companyName)
        {
            return byCompanyName.ContainsKey(companyName);
        }

        public bool ContainsContactName(string contactName)
        {
            return byContactName.ContainsKey(contactName);
        }

        public bool ContainsPhone(string phone)
        {
            return byPhone.ContainsKey(phone);
        }

        public MyDB Select(Customer customer)
        {
            MyDB newDB = new MyDB();

            newDB.SelectBy(customer.CustomerID, Dictionary.CustomerID);
            newDB.SelectBy(customer.CompanyName, Dictionary.CompanyName);
            newDB.SelectBy(customer.ContactName, Dictionary.ContactName);
            newDB.SelectBy(customer.Phone, Dictionary.Phone);

            return newDB;
        }

        private MyDB SelectBy(string field, Dictionary selectBy)
        {
            MyLinkedList<Link<Customer>> newList = new MyLinkedList<Link<Customer>>();
            MyDB result = new MyDB();

            if (selectBy == Dictionary.CustomerID && field != "")
                if (byCustomerID.ContainsKey(field))
                {
                    var line = byCustomerID[field].Data;
                    result.Insert(line);
                    return result;
                }

            if (selectBy == Dictionary.CompanyName && field != "")
                if (byCompanyName.ContainsKey(field))
                {
                    foreach (var link in byCompanyName[field])
                    {
                        var line = link.Data;
                        result.Insert(line);
                    }
                }

            if (selectBy == Dictionary.ContactName && field != "")
                if (byContactName.ContainsKey(field))
                {
                    foreach (var link in byContactName[field])
                    {
                        var line = link.Data;
                        result.Insert(line);
                    }
                }

            if (selectBy == Dictionary.Phone && field != "")
                if (byPhone.ContainsKey(field))
                {
                    foreach (var link in byPhone[field])
                    {
                        var line = link.Data;
                        result.Insert(line);
                    }
                }

            return result;
        }

        void AddToDictionaries(Link<Customer> link, Dictionary dictionary)
        {
            if (dictionary.HasFlag(Dictionary.CustomerID))
                byCustomerID.Add(link.Data.CustomerID, link);

            if (dictionary.HasFlag(Dictionary.CompanyName))
                if (!byCompanyName.ContainsKey(link.Data.CompanyName))
                    byCompanyName.Add(link.Data.CompanyName, new MyLinkedList<Link<Customer>>(link));
                else byCompanyName[link.Data.CompanyName].Insert(link);

            if (dictionary.HasFlag(Dictionary.ContactName))
                if (!byContactName.ContainsKey(link.Data.ContactName))
                    byContactName.Add(link.Data.ContactName, new MyLinkedList<Link<Customer>>(link));
                else byContactName[link.Data.ContactName].Insert(link);

            if (dictionary.HasFlag(Dictionary.Phone))
                if (!byPhone.ContainsKey(link.Data.Phone))
                    byPhone.Add(link.Data.Phone, new MyLinkedList<Link<Customer>>(link));
                else byPhone[link.Data.Phone].Insert(link);
        }

        void RemoveFromDictionaries(Link<Customer> link, Dictionary dictionary)
        {
            if (dictionary.HasFlag(Dictionary.CustomerID))
            {
                byCustomerID.Remove(link.Data.CustomerID);
            }

            if (dictionary.HasFlag(Dictionary.CompanyName))
            {
                byCompanyName[link.Data.CompanyName].Remove(link);
                if (byCompanyName[link.Data.CompanyName].IsEmpty()) byCompanyName.Remove(link.Data.CompanyName);
            }

            if (dictionary.HasFlag(Dictionary.ContactName))
            {
                byContactName[link.Data.ContactName].Remove(link);
                if (byContactName[link.Data.ContactName].IsEmpty()) byContactName.Remove(link.Data.ContactName);
            }

            if (dictionary.HasFlag(Dictionary.Phone))
            {
                byPhone[link.Data.Phone].Remove(link);
                if (byPhone[link.Data.Phone].IsEmpty()) byPhone.Remove(link.Data.Phone);
            }
        }

        public void Insert(MyLinkedList<Customer> customersTable)
        {
            foreach (var customer in customersTable)
            {
                if (byCustomerID.ContainsKey(customer.CustomerID))
                    throw new ArgumentException($"Customer with the ID: '{customer.CustomerID}' is already exist in the table.");

                _customersTable.Insert(customer);
                AddToDictionaries(_customersTable.Head, Dictionary.All);
            }
        }

        public void Insert(MyDB db)
        {
            Insert(db._customersTable);
        }

        public void Insert(Customer customer)
        {
            Insert(new MyLinkedList<Customer>(customer));
        }

        //O(n) - Нужно исправить MyLinkedList чтобы получить O(1)
        public void Delete(string id)
        {
            if (!byCustomerID.ContainsKey(id.ToString()))
                throw new ArgumentException($"Customer with the ID: '{id}' does not exist.");

            var link = byCustomerID[id];
            RemoveFromDictionaries(link, Dictionary.All);
            _customersTable.RemovePos(link);
        }

        public void MultipleDelete(IEnumerable db)
        {
            foreach (var item in db)
                Delete(((Customer)item).CustomerID);
        }

        public void Update(Customer customer)
        {
            if (customer.CustomerID == "")
                throw new ArgumentException("The customer ID is empty.");
            if (!byCustomerID.ContainsKey(customer.CustomerID))
                throw new ArgumentException($"Customer with the ID: '{customer.CustomerID}' is not exist.");

            var link = byCustomerID[customer.CustomerID];

            if (link.Data.CompanyName != customer.CompanyName && customer.CompanyName != "")
            {
                RemoveFromDictionaries(link, Dictionary.CompanyName);
                link.Data.CompanyName = customer.CompanyName;
                AddToDictionaries(link, Dictionary.CompanyName);
            }

            if (link.Data.ContactName != customer.ContactName && customer.ContactName != "")
            {
                RemoveFromDictionaries(link, Dictionary.ContactName);
                link.Data.ContactName = customer.ContactName;
                AddToDictionaries(link, Dictionary.ContactName);
            }

            if (link.Data.Phone != customer.Phone && customer.Phone != "")
            {
                RemoveFromDictionaries(link, Dictionary.Phone);
                link.Data.Phone = customer.Phone;
                AddToDictionaries(link, Dictionary.Phone);
            }
        }

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
