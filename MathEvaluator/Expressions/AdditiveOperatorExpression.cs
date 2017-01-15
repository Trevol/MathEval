using MathEvaluator.Utils;

namespace MathEvaluator.Expressions
{
    /// <summary>
    /// + or -. Can be in unary form (without LOperand, +23, -45)
    /// </summary>
    public class AdditiveOperatorExpression: OperatorExpression
    {
        public bool IsSubtraction;

        private AdditiveOperatorExpression(IArithExpression lOperand, IArithExpression rOperand, bool isSubtraction): base(lOperand, rOperand)
        {
            IsSubtraction = isSubtraction;
        }

        public bool IsUnary => LOperand.IsNull();

        public override double Evaluate()
        {
            return IsUnary
                ? UnaryOp(ROperand.Evaluate(), IsSubtraction)
                : BinaryOp(LOperand.Evaluate(), ROperand.Evaluate(), IsSubtraction);
        }

        private static double UnaryOp(double arg, bool subtract) => subtract ? -arg : arg;

        private static double BinaryOp(double larg, double rarg, bool subtract) => subtract ? larg - rarg : larg + rarg;

        public static AdditiveOperatorExpression Addition(IArithExpression lOperand, IArithExpression rOperand)
        {
            return new AdditiveOperatorExpression(lOperand, rOperand, false);
        }
        public static AdditiveOperatorExpression Subtraction(IArithExpression lOperand, IArithExpression rOperand)
        {
            return new AdditiveOperatorExpression(lOperand, rOperand, true);
        }
    }
}