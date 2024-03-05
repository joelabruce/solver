using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solver
{
    public class Summation
    {
        public int Start { get; private set; }
        public int End { get; private set; }
        public Summation(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}
