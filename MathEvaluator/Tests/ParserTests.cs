using FluentAssertions;
using MathEvaluator.Expressions;
using MathEvaluator.Parsing;
using MathEvaluator.Utils;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MathEvaluator.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void UnaryAdditionExpressionTest()
        {
            var parser = new Parser();
            var expression = parser.Parse("2+3");
            expression.Should().BeOfType<AdditiveOperatorExpression>();

            expression.To<AdditiveOperatorExpression>().IsSubtraction.Should().BeFalse();

            expression.To<AdditiveOperatorExpression>().LOperand.Should().BeOfType<ValueExpression>();
            expression.To<AdditiveOperatorExpression>().LOperand.To<ValueExpression>().Value.Should().Be(2);

            expression.To<AdditiveOperatorExpression>().ROperand.Should().BeOfType<ValueExpression>();
            expression.To<AdditiveOperatorExpression>().ROperand.To<ValueExpression>().Value.Should().Be(3);
        }
        [Test]
        public void UnarySubtractionExpressionTest()
        {
            var parser = new Parser();
            var expression = parser.Parse("2-3");
            expression.Should().BeOfType<AdditiveOperatorExpression>();

            expression.To<AdditiveOperatorExpression>().IsSubtraction.Should().BeTrue();

            expression.To<AdditiveOperatorExpression>().LOperand.Should().BeOfType<ValueExpression>();
            expression.To<AdditiveOperatorExpression>().LOperand.To<ValueExpression>().Value.Should().Be(2);

            expression.To<AdditiveOperatorExpression>().ROperand.Should().BeOfType<ValueExpression>();
            expression.To<AdditiveOperatorExpression>().ROperand.To<ValueExpression>().Value.Should().Be(3);
        }

        [Test]
        public void DoTest()
        {
            new Parser().Parse("2*-6").Output();
            var expression = new Parser().Parse("2.56*6-10.56");
            expression.Output();
        }
    }
}