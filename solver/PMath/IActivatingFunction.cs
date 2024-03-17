using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solver.PMath
{
    public interface IActivatingFunction
    {
        public Func<Operand> AF { get; }
        public Action<Operand> BackPropagation { get; }
    }
}
