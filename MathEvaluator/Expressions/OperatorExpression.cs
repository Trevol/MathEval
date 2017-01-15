using System;
using System.Dynamic;
using MathEvaluator.Parsing;

namespace MathEvaluator.Expressions
{
    public abstract class OperatorExpression: IArithExpression
    {
        protected OperatorExpression(IArithExpression lOperand, IArithExpression rOperand)
        {
            if (rOperand == null) throw new ParserException("ROperand is required");
            LOperand = lOperand;
            ROperand = rOperand;
        }

        public IArithExpression LOperand { get; }
        public IArithExpression ROperand { get; }
        public abstract double Evaluate();

        public static OperatorExpression Create(string op, IArithExpression lOperand, IArithExpression rOperand)
        {
            switch (op)
            {
                case "+":
                    return AdditiveOperatorExpression.Addition(lOperand, rOperand);
                case "-":
                    return AdditiveOperatorExpression.Subtraction(lOperand, rOperand);
                case "*":
                    return MultiplicativeOperatorExpression.Multiplication(lOperand, rOperand);
                case "/":
                    return MultiplicativeOperatorExpression.Division(lOperand, rOperand);
                default:
                    throw new ParserException("Unexpected operator " + op);
            }
        }
    }
}