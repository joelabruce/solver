using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solver.PMath
{
    public abstract class Operable
    {
        public static Operable operator+(Operable self, Operable other)
        {
            return self.Add(other);
        }

        public static Operable operator*(Operable self, Operable other)
        {
            return self.Multiply(other);
        }

        public abstract Operable Add(Operable other);
        public abstract Operable Multiply(Operable other);
    }
}
