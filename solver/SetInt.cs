using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solver
{
    public class Setlong
    {
        private Lazy<long> sum; 
        public long[] Values { get; private set; }

        public long Sum
        {
            get => sum.Value; 
        }

        private Setlong()
        {
            sum = new Lazy<long>(() => Values.Sum());
        }

        public Setlong(params long[] args) : this()
        {
            Values = args;
        }
    }
}
