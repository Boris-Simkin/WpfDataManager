using System;
//using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_DataStructures
{
    public class Link<T>
    {
        public T Data { get; set; }
        public Link<T> Next { get; set; }
        public Link<T> Prev { get; set; }

        public Link(T data, Link<T> next, Link<T> prev)
        {
            Data = data;
            Next = next;
            Prev = prev;
        }

        public override string ToString()
        {
            return Data.ToString();
        }
    }
}
