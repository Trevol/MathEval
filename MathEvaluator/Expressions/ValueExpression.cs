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
    }
}