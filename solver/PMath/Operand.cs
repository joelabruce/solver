using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solver.PMath
{
    public class Operand
    {
        public string Label { get; private set; }
        public float Value { get; private set; }
        public float Gradient { get; private set; } 
        
        private Operand[] previous;
        private Operators? Operator;

        public Action BackPropagation { get; private set; }
        public IActivatingFunction Squish { get; set; }

        private Operand(float value, Operators? operation, params Operand[] operands)
        {
            Value = value;
            Gradient = 0;
            Operator = operation;
            previous = operands;
        }

        public Operand(float value, string label)
        {
            Value = value;
            Label = label;
        }

        private void AuxilaryLabel(Operand self, Operand other, Operators operatorContext)
        {
            Label = $"({self.Label}{operatorContext.GetSymbol()}{other.Label})";
        }

        /// <summary>
        /// Adding two operands, and it's back-propagation.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static Operand operator+ (Operand self, Operand other)
        {
            var x = new Operand(self.Value + other.Value, Operators.Add, self, other);
            x.AuxilaryLabel(self, other, Operators.Add);
            x.BackPropagation = () =>
            {
                self.Gradient += x.Gradient;
                other.Gradient += x.Gradient;
            };

            return x;
        }

        /// <summary>
        /// Multiplying two operands and it's back-propagation.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static Operand operator* (Operand self, Operand other)
        {
            var x = new Operand(self.Value * other.Value, Operators.Multiply, self, other);
            x.AuxilaryLabel(self, other, Operators.Multiply);
            x.BackPropagation = () =>
            {
                self.Gradient += other.Value * x.Gradient;
                other.Gradient += self.Value * x.Gradient;
            };

            return x;
        }

        public static Operand operator^ (Operand self, Operand other)
        {
            var x = new Operand(MathF.Pow(self.Value, other.Value), Operators.Power, self, other);
            x.AuxilaryLabel(self, other, Operators.Power);
            x.BackPropagation = () =>
            {
                self.Gradient += (other.Value * MathF.Pow(self.Value, other.Value - 1) * x.Gradient);
            };

            return x;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public string ToExpression()
        {
            return Label;
        }
    }

    public static class OperandExtensions
    {
        /// <summary>
        /// Wrap in Operand.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static Operand _(this float value, string label)
        {
            return new Operand(value, label);
        }
    }

    public enum Operators
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Power
    }

    public static class OperatorExtensions
    {
        private static Dictionary<Operators, string> symbols = new Dictionary<Operators, string>() {
            { Operators.Add, "+" },
            { Operators.Subtract, "-" },
            { Operators.Multiply, "*" },
            { Operators.Power, "**" },
            { Operators.Divide, "/" }
        };


        public static string GetSymbol(this Operators operatorContext)
        {
            return symbols[operatorContext];
        }
    }
}
