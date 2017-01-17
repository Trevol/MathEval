using System;
using MathEvaluator.Parsing;

namespace MathEvaluator.Expressions
{
    /// <summary>
    /// * or /
    /// </summary>
    public class MultiplicativeOperatorExpression : OperatorExpression
    {
        public bool IsDivision;

        private MultiplicativeOperatorExpression(IArithExpression lOperand, IArithExpression rOperand, bool isDivision) : base(lOperand, rOperand)
        {            
            if (LOperand == null) throw new ParsingException("LOperand is required");
            IsDivision = isDivision;
        }

        public override double Evaluate()
        {
            return Op(LOperand.Evaluate(), ROperand.Evaluate(), IsDivision);
        }

        static double Op(double larg, double rarg, bool divide)
        {
            return divide ? larg / rarg : larg * rarg;
        }

        public static MultiplicativeOperatorExpression Multiplication(IArithExpression lOperand, IArithExpression rOperand)
        {
            return new MultiplicativeOperatorExpression(lOperand, rOperand, false);
        }
        public static MultiplicativeOperatorExpression Division(IArithExpression lOperand, IArithExpression rOperand)
        {
            return new MultiplicativeOperatorExpression(lOperand, rOperand, true);
        }

        public override string ToString()
        {
            var op = IsDivision?"div" : "mult";
            return $"{op}({LOperand}, {ROperand})";
        }
    }
}