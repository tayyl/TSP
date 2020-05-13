using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Model
{
    public class City
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Number { get; private set; }
        public int Xview { get; private set; }
        public int Yview { get; private set; }
        public City(int x, int y, int number)
        {
            X = x;
            Y = y;
            Xview = x-10;
            Yview = y-10;
            Number = number;
        }
    }
}
