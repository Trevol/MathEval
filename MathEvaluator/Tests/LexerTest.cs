using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MathEvaluator.Parsing;
using NUnit.Framework;

namespace MathEvaluator.Tests
{
    [TestFixture]
    public class LexerTest
    {
        [Test]
        public void EnumeratorTest()
        {
            var enumerator = new Lexer("2+3").Tokens().GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Output();
            }
        }
        [Test]
        public void BinaryAdditionTest()
        {
            var lexer = new Lexer("2+3");
            var tokens = lexer.Tokens().ToArray();
            tokens.Length.Should().Be(3);

            tokens[0].Type.Should().Be(TokenType.Value);
            tokens[0].Value.Should().Be("2");

            tokens[1].Type.Should().Be(TokenType.AdditiveOperator);
            tokens[1].Value.Should().Be("+");

            tokens[2].Type.Should().Be(TokenType.Value);
            tokens[2].Value.Should().Be("3");
        }
        [Test]
        public void BinarySubtractionTest()
        {
            var lexer = new Lexer("2-3");
            var tokens = lexer.Tokens().ToArray();
            tokens.Length.Should().Be(3);

            tokens[0].Type.Should().Be(TokenType.Value);
            tokens[0].Value.Should().Be("2");

            tokens[1].Type.Should().Be(TokenType.AdditiveOperator);
            tokens[1].Value.Should().Be("-");

            tokens[2].Type.Should().Be(TokenType.Value);
            tokens[2].Value.Should().Be("3");
        }

        [Test]
        public void WhiteSpaceTest()
        {
            var lexer=new Lexer(" 2 + \t 3 ");
            var tokens = lexer.Tokens().ToArray();
            
            tokens.Length.Should().Be(3);

            tokens[0].Type.Should().Be(TokenType.Value);
            tokens[0].Value.Should().Be("2");

            tokens[1].Type.Should().Be(TokenType.AdditiveOperator);
            tokens[1].Value.Should().Be("+");

            tokens[2].Type.Should().Be(TokenType.Value);
            tokens[2].Value.Should().Be("3");
        }

        [Test]
        public void UnexpectedTokenTest()
        {            
            var lexer = new Lexer("2+2^3");
            lexer.Invoking(l => l.Tokens()).ShouldThrowExactly<ParsingException>();            
        }

        [Test]
        public void WhiteSpaceOnlyNumbersTest()
        {
            var lexer = new Lexer("2 \t\r 3");
            var tokens = lexer.Tokens().ToArray();

            tokens.Length.Should().Be(2);

            tokens[0].Type.Should().Be(TokenType.Value);
            tokens[0].Value.Should().Be("2");

            tokens[1].Type.Should().Be(TokenType.Value);
            tokens[1].Value.Should().Be("3");
        }
        
    }
}
