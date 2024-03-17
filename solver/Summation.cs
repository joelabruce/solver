using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solver
{
    public class Summation
    {
        public long Start { get; private set; }
        public long End { get; private set; }
        public Summation(long start, long end)
        {
            Start = start;
            End = end;
        }
    }
}
