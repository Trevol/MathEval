using System;
using System.Globalization;
using System.Linq.Expressions;
using FluentAssertions;
using MathEvaluator.Expressions;
using MathEvaluator.Parsing;
using MathEvaluator.Utils;
using NUnit.Framework;

namespace MathEvaluator.Tests
{
    [TestFixture]
    public class ProtoTest
    {
        [Test]
        public void ExpressionsTest()
        {
            var a1 = 2.0;
            var a2 = 3.0;

            //Expression<Func<double>> e = ()=>a1+a2+4.0;
            //add(add(a1, a2, 4)

            Expression<Func<double>> e = () => a1 * (a2 + 4.0 - a1);
            var body = e.To<LambdaExpression>().Body;
        }

        [Test]
        public void AdditiveTest()
        {
            var text = "2+3+4";
            var expression = new ProtoParser().Parse(text);
            expression.Output();
        }

        [Test]
        public void AdditiveMultiplicativeTest()
        {
            new ProtoParser().Parse("2+3*4").Output();
            new ProtoParser().Parse("2/3-4").Output();

            new ProtoParser().Parse("2+3*4+5").Output();
        }

        [Test]
        public void InvalidExpressionTest()
        {
            var text = "2 4";
            new ProtoParser().Invoking(p=>p.Parse(text)).ShouldThrowExactly<ParsingException>();            
        }

        public class ProtoParser
        {
            public IArithExpression Parse(string text)
            {
                var lexer = new Lexer(text);
                using (var tokens = new TokenCursor(lexer.Tokens()))
                {
                    Func<Token, bool> toEnd = t => false;
                    return ParseExpression(tokens.Next(), toEnd, tokens, false);
                }
            }

            private static IArithExpression ParseExpression(Token start, Func<Token, bool> endOfExpression, TokenCursor tokens, bool moveToPrevAtEnd)
            {
                var current = start;
                IArithExpression resultExpr = null;
                //while next && end(next).not()
                while (current.IsNotNull())
                {
                    if (endOfExpression(current))
                    {
                        if (moveToPrevAtEnd)
                        {
                            tokens.MovePrev();
                        }
                        break;
                    }
                    resultExpr = ParseToken(current, resultExpr, tokens);
                    current = tokens.Next();
                }
                if (resultExpr.IsNull())
                {
                    //TODO: provide more usefull msg
                    throw new ParsingException("resultExpr.IsNull()");
                }
                return resultExpr;

                //operators precedence:
                //- test for multiplicative operator - build one first
                //- test for expression group - (....)
                //- test for function call - ID(args)
                //prevExpression is not applicable in case of ROperand parsing
            }

            private static IArithExpression ParseToken(Token token, IArithExpression prevExpr, TokenCursor tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Value:
                        return ParseValue(token);
                    case TokenType.AdditiveOperator:
                    case TokenType.MultiplicativeOperator:
                        return ParseOperator(token, prevExpr, tokens);
                    default:
                        throw new Exception("Unexpected token: " + token);
                }
            }

            private static IArithExpression ParseOperator(Token operatorToken, IArithExpression lOperand, TokenCursor tokens)
            {
                var rOperand = ParseRightOperand(tokens.Next(), operatorToken, tokens);
                return OperatorExpression.Create(operatorToken.Value, lOperand, rOperand);
            }

            private static IArithExpression ParseRightOperand(Token operandToken, Token operatorToken, TokenCursor tokens)
            {                
                if (operatorToken.Type == TokenType.AdditiveOperator)
                {
                    return ParseAdditiveRightOperand(operandToken, tokens);
                }
                if (operatorToken.Type == TokenType.MultiplicativeOperator)
                {
                    return ParseMultiplicativeRightOperand(operandToken, tokens);
                }
                throw new ParsingException("Unexpected operator token: " + operandToken);
            }

            private static IArithExpression ParseAdditiveRightOperand(Token operandToken, TokenCursor tokens)
            {
                Func<Token, bool> endOfOperand = t => t.Type == TokenType.AdditiveOperator;
                return ParseExpression(operandToken, endOfOperand, tokens, true);                
            }
            private static IArithExpression ParseMultiplicativeRightOperand(Token operandToken, TokenCursor tokens)
            {
                Func<Token, bool> endOfOperand = t => t.Type == TokenType.AdditiveOperator;
                return ParseExpression(operandToken, endOfOperand, tokens, true);
            }

            private static IArithExpression ParseValue(Token valueToken)
            {
                return new ValueExpression(double.Parse(valueToken.Value, NumberStyles.Any, CultureInfo.InvariantCulture));
            }
        }

    }
}