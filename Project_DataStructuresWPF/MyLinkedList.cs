using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_DataStructures
{
    class MyLinkedList<T> : IEnumerable<T>
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
        /// Returns whether the list is empty
        /// </summary>
        public bool IsEmpty()
        {
            return _head == null;
        }

        /// <summary>
        /// Inserts a record at the head of the list
        /// </summary>
        /// <param name="data"></param>
        public void Insert(T data)
        {
            Link<T> link = new Link<T>(data, _head, null);

            if (_head != null)
                _head.Prev = link;

            _head = link;

            Count++;
        }
        /// <summary>
        /// Inserts a record after the elment p in the list
        /// </summary>
        /// <param name="data"></param>
        /// <param name="p"></param>
        public void Insert(T data, Link<T> p)
        {
            if (p == null)
                Insert(data);
            else
            {
                Link<T> link = new Link<T>(data, p.Next, null);

                if (p.Next != null)
                    p.Next.Prev = link;

                p.Next = link;
                Count++;
            }
        }

        public void RemoveAllData(T data)
        {
            while (Remove(data)) ;
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
                if (_head != null)
                    _head.Prev = null;
                Count--;
            }
            return res.Data;
        }

        /// <summary>
        /// Removes the node that contains the specific data
        /// and return true if the data was found
        /// </summary>
        public bool Remove(T data)
        {
            if (_head != null)
            {
                if (_head.Data.Equals(data))
                {
                    _head = _head.Next;
                    if (_head != null)
                        _head.Prev = null;
                    Count--;
                    return true;
                }

                Link<T> link = _head.Next;

                while (link != null && !link.Data.Equals(data))
                    link = link.Next;

                if (link != null)
                {
                    link.Prev.Next = link.Next;
                    if (link.Next != null)
                        link.Next.Prev = link.Prev;
                    Count--;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the node by link
        /// and return its data
        /// </summary>
        public T RemovePos(Link<T> p)
        {
            Link<T> res = p;
            if (IsEmpty())
                throw new ArgumentException("The list is empty.");

            if (p == _head)
            {
                if (p.Next != null)
                {
                    _head = _head.Next;
                    _head.Prev = null;
                }
                else
                    _head = null;

                Count--;
                return res.Data;
            }

            p.Prev.Next = p.Next;
            if (p.Next != null)
                p.Next.Prev = p.Prev;

            Count--;
            return res.Data;
        }

        /// <summary>
        /// returns the number of nodes
        /// </summary>
        public int Count
        {
            get; private set;
        }

        public override string ToString()
        {
            if (IsEmpty())
                return "List is empty!!";

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
