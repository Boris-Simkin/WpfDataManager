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

        public Link(T data, Link<T> next)
        {
            Data = data;
            Next = next;
        }

        public override string ToString()
        {
            return Data.ToString();
        }
    }
}
