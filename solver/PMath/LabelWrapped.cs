using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solver.PMath
{
    public class LabelWrapped<T>
    {
        public string Label { get; private set; }
        public T Wrapped { get; private set; }

        public LabelWrapped(string label, T value)
        {
            Label = label;
            Wrapped = value;
        }
    }
}
