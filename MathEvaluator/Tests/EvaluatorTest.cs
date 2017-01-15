using System;
using System.Globalization;
using FluentAssertions;
using MathEvaluator.Parsing;
using MathEvaluator.Utils;
using NUnit.Framework;

namespace MathEvaluator.Tests
{
    [TestFixture]
    public class EvaluatorTest
    {
        [Test]
        public void BinaryAdditionTest()
        {
            var expression = "2+2";
            var expected = 2 + 2;
            TestExpressionEvaluation(expression, expected);
        }
        [Test]
        public void BinaryDoubleAdditionTest()
        {
            TestExpressionEvaluation("2+2.45", 2 + 2.45);
            TestExpressionEvaluation("3.4-3.4", 3.4 - 3.4);
        }
        [Test]
        public void BinarySubtractionTest()
        {
            var expression = "2-3";
            var expected = 2 - 3;
            TestExpressionEvaluation(expression, expected);
        }
        [Test]
        public void MultipleAdditiveOperatorsTest()
        {
            var expression = "-2-3+5.5+6.97";
            var expected = -2 - 3 + 5.5 + 6.97;
            TestExpressionEvaluation(expression, expected);
        }
        [Test]
        public void UnaryAdditionTest()
        {
            var expression = "+2";
            var expected = +2;
            TestExpressionEvaluation(expression, expected);
        }
        [Test]
        public void UnarySubtractionTest()
        {
            var expression = "-3";
            var expected = -3;
            TestExpressionEvaluation(expression, expected);
        }
        [Test]
        public void UnarySubtractionWhiteSpaceTest()
        {
            var expression = "- 3";
            var expected = -3;
            TestExpressionEvaluation(expression, expected);
        }

        [Test]
        public void MultiplicativeExpressionsTest()
        {
            TestExpressionEvaluation("3.4*6.7", 3.4 * 6.7);
            TestExpressionEvaluation("3.4/6.7", 3.4 / 6.7);
        }

        [Test]
        public void ValueExpressionTest()
        {
            TestExpressionEvaluation("3.56", 3.56);
        }

        [Test]
        public void MixAdditiveAndMultilicativeTest()
        {
            TestExpressionEvaluation("2.56*6-10.56", 2.56 * 6 - 10.56);
            TestExpressionEvaluation("-6*2", -6*2);
            TestExpressionEvaluation("2*-6", 2 * -6);
            TestExpressionEvaluation("2*+6", 2 * +6);
            TestExpressionEvaluation("2/-6", 2.0 / -6);
            TestExpressionEvaluation("2/+6", 2.0 / +6);

            Action a = () => TestExpressionEvaluation("2-/6", 2.0 / -6);
            a.ShouldThrowExactly<ParserException>();
        }

        [Test]
        public void OperatorPrecedenceTest()
        {
            
        }

        [Test]
        public void CurrentTest()
        {
            //part of OperatorPrecedenceTest
            TestExpressionEvaluation("4.5+6.7*9.8", 4.5 + 6.7 * 9.8);
            TestExpressionEvaluation("4.5-6.7*9.8", 4.5 - 6.7 * 9.8);

            TestExpressionEvaluation("4.5/7.1-6.7*9.8", 4.5 - 6.7 * 9.8);
        }

        [Test]
        public void SimpleDivisionByZeroTest()
        {
            /*var t = 4.6 / 0 * - 1000000;
            t = -4.6 / 0 * 10000;*/
            
            TestExpressionEvaluation("4.67/0", Double.PositiveInfinity);
            TestExpressionEvaluation("-4.67/0", Double.NegativeInfinity);
        }

        [Test]
        public void ComplexExpressionTest()
        {
            var expression = "1 + (2 * 9.8) / (-2 - 6) * Sqrt(4.5 / 9.8 * 1.2)";
            var expected = 1 + (2 * 9.8) / (-2 - 6) * Math.Sqrt(4.5 / 9.8 * 1.2);

            TestExpressionEvaluation(expression, expected);
        }

        private void ExpressionsToTest()
        {
            //Unary ops and Unary/binary mix
            //-3
            //+3
            //- -3
            //+-3
            //+-3;
            //-+3;
            //3+-4
            //3--4
            //3++4
            //3-+4

            //operation's priorities
            //2+3*5
            //2-3*5
            //2-10/5
            //3*(10-5)
        }

        private void EvalErrors()
        {
            /*
             div by 0:
             4.5/0
             4.5/(3.4-3.4)
             */
        }

        private void TestExpressionEvaluation(string expressionText, double expected)
        {
            Evaluator.Evaluate(expressionText).Should().Be(expected);
        }
    }
}