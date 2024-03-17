using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solver
{
    public class Doublet
    {
        public long Pre { get; private set; }
        public long Post { get; private set; }

        public Doublet(long a, long b)
        {
            Pre = a;
            Post = b;
        }

        public override string ToString()
        {
            return $"{Pre}, {Post}";
        }
    }
}
