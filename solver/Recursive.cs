using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solver
{
    public class Recursive
    {
        public Recursive[] Parents { get; private set; }

        public long Terminal { get; private set; }

        public Recursive(long i, long j)
        {
            Parents = new Recursive[1];
            Terminal = j;
        }

        public Recursive(long i, IEnumerable<Recursive> children)
        {
            foreach(var child in children)
            {

            }
        }

        //public override string ToString()
        //{
        //    return $"{{{string.Join(", ", Values)}}}";
        //}
    }
}
