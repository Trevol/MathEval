using MathEvaluator.Parsing;

namespace MathEvaluator.Tests
{
    public static class Evaluator
    {
        public static double Evaluate(string expression)
        {
            return new Parser().Parse(expression).Evaluate();
        }
    }
}