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
            TestExpressionEvaluation("2-3", 2 - 3);
        }
        [Test]
        public void MultipleAdditiveOperatorsTest()
        {
            TestExpressionEvaluation("--6.97", - -6.97);
            TestExpressionEvaluation("++6.97", + +6.97);
            TestExpressionEvaluation("-+6.97", -+6.97);
            TestExpressionEvaluation("+-6.97", +-6.97);
            TestExpressionEvaluation("-5.5+ -6.97", -5.5 + -6.97);
            TestExpressionEvaluation("-5.5 - -6.97", -5.5 - -6.97);
            TestExpressionEvaluation("-5.5 - -6.97 - +4.5", -5.5 - -6.97 - +4.5);
            TestExpressionEvaluation("-2-3+5.5+6.97", -2 - 3 + 5.5 + 6.97);
        }
        [Test]
        public void UnaryAdditionTest()
        {
            TestExpressionEvaluation("+2", +2);
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
        public void IncorrectMultiplicativeOperatorTest()
        {
            Action(() => TestExpressionEvaluation("/6")).ShouldThrowExactly<ParsingException>();
            Action(() => TestExpressionEvaluation("*6")).ShouldThrowExactly<ParsingException>();
            Action(() => TestExpressionEvaluation("6/")).ShouldThrowExactly<ParsingException>();
            Action(() => TestExpressionEvaluation("6*")).ShouldThrowExactly<ParsingException>();
            Action(() => TestExpressionEvaluation("2-/6")).ShouldThrowExactly<ParsingException>();
            Action(() => TestExpressionEvaluation("2-6/")).ShouldThrowExactly<ParsingException>();
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
            TestExpressionEvaluation("-6*2", -6 * 2);
            TestExpressionEvaluation("2*-6", 2 * -6);
            TestExpressionEvaluation("2*+6", 2 * +6);
            TestExpressionEvaluation("2/-6", 2.0 / -6);
            TestExpressionEvaluation("2/+6", 2.0 / +6);
        }

        [Test]
        public void MixGroupsTest()
        {
            TestExpressionEvaluation(
                "(2 * 9) / (8) * (4 + 1)",
                (2 * 9.0) / (8) * (4.0 + 1.0));

            TestExpressionEvaluation(
                "1 + (2 * 9.8) / (-2 - 6) * (4.5 / 9.8 + 1.2) + (-34.57 * (3.5 - 3.7))",
                1 + (2 * 9.8) / (-2 - 6) * (4.5 / 9.8 + 1.2) + (-34.57 * (3.5 - 3.7)));

            TestExpressionEvaluation(
                "(1 + (2 * 9.8) / (-2 - 6) * (4.5 / 9.8 + 1.2) + (-34.57 * (3.5 - 3.7)))/(4.7 + 2.4*777/5.6 + 7)",
                (1 + (2 * 9.8) / (-2 - 6) * (4.5 / 9.8 + 1.2) + (-34.57 * (3.5 - 3.7))) / (4.7 + 2.4 * 777 / 5.6 + 7)
                );
        }

        private static Action Action(Action a) => a;

        [Test]
        public void OperatorPrecedenceTest()
        {
            TestExpressionEvaluation("2+3*4", 2 + 3 * 4);
            TestExpressionEvaluation("4.5+6.7*9.8", 4.5 + 6.7 * 9.8);
            TestExpressionEvaluation("4.5-6.7*9.8", 4.5 - 6.7 * 9.8);
            TestExpressionEvaluation("4.5/7.1-6.7*9.8", 4.5 / 7.1 - 6.7 * 9.8);
            TestExpressionEvaluation("4.5/-7.1-6.7*9.8", 4.5 / -7.1 - 6.7 * 9.8);
        }

        [Test]
        public void SimpleGroupTest()
        {
            TestExpressionEvaluation("(3)", (3));
            TestExpressionEvaluation("(2+3)", (2 + 3));
            TestExpressionEvaluation("(+3)", (+3));
            TestExpressionEvaluation("(-3)", (-3));
            TestExpressionEvaluation("((((-3))))", ((((-3)))));
        }
        [Test]
        public void EmptyGroupTest()
        {
            Action(() => TestExpressionEvaluation("()")).ShouldThrowExactly<ParsingException>()
                .And
                .Message.Should().Be("Empty expression group");
        }
        [Test]
        public void EmptyExpressionTest()
        {
            Action(() => TestExpressionEvaluation("")).ShouldThrowExactly<ParsingException>()
                .And
                .Message.Should().Be("Empty expression");
        }


        [Test]
        public void ParentesisMismatchTest()
        {
            Action(() => TestExpressionEvaluation("(")).ShouldThrowExactly<ParsingException>()
                .And
                .Message.Should().Be("Parentesis mismatch");

            Action(() => TestExpressionEvaluation("(2+3")).ShouldThrowExactly<ParsingException>()
                .And
                .Message.Should().Be("Parentesis mismatch");

            Action(() => TestExpressionEvaluation("(2+3))")).ShouldThrowExactly<ParsingException>()
                .And
                .Message.Should().Be("Parentesis mismatch");
        }

        [Test]
        public void CurrentTest()
        {
            //current work            
        }

        [Test]
        public void IncorrectExpressionsTest()
        {
            Action(() => TestExpressionEvaluation("3 4")).ShouldThrowExactly<ParsingException>();
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
        public void DesiredFeaturesTest()
        {
            var expression = "1 + (2 * 9.8) / (-2 - 6) * Sqrt(4.5 / 9.8 * 1.2) + Math.Pow(-34.57, 3.5)";
            var expected = 1 + (2 * 9.8) / (-2 - 6) * Math.Sqrt(4.5 / 9.8 * 1.2 + Math.Pow(-34.57, 3.5));

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
             Sqrt(-2) NaN
             */
        }

        private void TestExpressionEvaluation(string expressionText, double expected)
        {
            Evaluator.Evaluate(expressionText).Should().Be(expected);
        }
        private void TestExpressionEvaluation(string expressionText)
        {
            Evaluator.Evaluate(expressionText);
        }
    }
}