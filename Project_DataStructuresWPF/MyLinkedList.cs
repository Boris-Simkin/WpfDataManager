using System;
using System.Collections;
using System.Collections.Generic;
//using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_DataStructures
{
    public class MyLinkedList<T> : IEnumerable<T>
    {
        private Link<T> _head;

        public Link<T> Head
        {
            get
            {
                return _head;
            }
        }

        public MyLinkedList() { }

        public MyLinkedList(T data)
        {
            Insert(data);
        }


        /// <summary>
        /// Inserts a record at the head of the list
        /// </summary>
        /// <param name="data"></param>
        public void Insert(T data)
        {
            Link<T> link = new Link<T>(data, _head);
            _head = link;
        }
        /// <summary>
        /// Inserts a record after the elment p in the list
        /// </summary>
        /// <param name="data"></param>
        /// <param name="p"></param>
        public void Insert(T data, Link<T> p)
        {
            if (p == null)
            {
                Insert(data);
            }
            else
            {
                Link<T> link = new Link<T>(data, p.Next);
                p.Next = link;
            }
        }

        /// <summary>
        /// Removes the first node from the list and returns its data
        /// </summary>
        /// <returns></returns>
        public T RemoveFirst()
        {
            Link<T> res = null;
            if (_head != null)
            {
                res = _head;
                _head = _head.Next;

            }
            return res.Data;
        }

        public bool IsEmpty()
        {
            return _head == null;
        }

        public bool Remove(T data)
        {
            if (_head != null)
            {
                if (_head.Data.Equals(data))
                {
                    _head = _head.Next;
                    return true;
                }

                Link<T> link = _head.Next;
                Link<T> preLink = _head;

                while (link != null && !link.Data.Equals(data))
                {
                    preLink = link;
                    link = link.Next;
                }

                if (link != null)
                {
                    preLink.Next = link.Next;
                    return true;
                }
            }
            return false;
        }

        public void RemoveAllData(T data)
        {
            while (Remove(data));
        }

        public T RemovePos(Link<T> p)
        {
            Link<T> res = p;
            if (p == _head)
            {
                if (p.Next == null)
                    throw new ArgumentException("Cannot remove the only element");
                _head = _head.Next;
                return res.Data;
            }
            
            Link<T> pr = Prev(p);
            pr.Next = p.Next;
            return res.Data;
        }

        public Link<T> Prev(Link<T> p)
        {
            Link<T> pr = this._head;
            if (p == _head)
                 throw new ArgumentException("The specified link is Head and has not a previus element");
            while (pr.Next != null && pr.Next != p)
            {
                pr = pr.Next;
            }
            if (pr.Next == null && pr != p)
                throw new KeyNotFoundException("The specified link does not exist in the linked list");
            return pr;
        }

        public int Count()
        {
            Link<T> p = _head;
            int cnt = 0;
            while (p != null)
            {
                cnt++;
                p = p.Next;
            }
            return cnt;
        }

        public override string ToString()
        {
            if (_head == null)
            {
                return "List is empty!!";
            }

            StringBuilder sb = new StringBuilder();

            Link<T> link = _head;
            while (link != null)
            {
                sb.Append(link.ToString());
                sb.Append(Environment.NewLine);
                link = link.Next;
            }

            return sb.ToString();
        }

        public IEnumerator<T> GetEnumerator()
        {
            Link<T> link = _head;
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
