using System.Globalization;

namespace MathEvaluator.Expressions
{
    public class ValueExpression : IArithExpression
    {
        public double Value { get; }

        public ValueExpression(double value)
        {
            Value = value;
        }

        public double Evaluate()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}