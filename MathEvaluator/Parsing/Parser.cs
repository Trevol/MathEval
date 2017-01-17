using System;
using System.Collections.Generic;
using System.Globalization;
using MathEvaluator.Expressions;
using MathEvaluator.Tests;
using MathEvaluator.Utils;

namespace MathEvaluator.Parsing
{
    public class Parser
    {
        public IArithExpression Parse(string text)
        {
            var lexer = new Lexer(text);
            using (var tokens = new TokenCursor(lexer.Tokens()))
            {
                Func<Token, bool> toEnd = t => false;
                var expression = ParseExpression(tokens.Next(), toEnd, tokens, false);
                if (expression.IsNull())
                {
                    throw new ParsingException("Empty expression");
                }
                return expression;
            }
        }

        private static IArithExpression ParseExpression(Token start, Func<Token, bool> endOfExpression, TokenCursor tokens, bool moveToPrevAtEnd)
        {
            var current = start;
            IArithExpression resultExpr = null;
            //while next && end(next).not()
            while (current.IsNotNull())
            {
                resultExpr = ParseToken(current, resultExpr, tokens);

                current = tokens.Next();
                if (current.IsNotNull() && endOfExpression(current))
                {
                    if (moveToPrevAtEnd)
                    {
                        tokens.MovePrev();
                    }
                    break;
                }
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
                case TokenType.OParenthesis:
                    return ParseGroup(tokens.Next(), tokens);
                case TokenType.CParenthesis:
                    return prevExpr;
                default:
                    throw new ParsingException("Unexpected token: " + token);
            }
        }

        private static IArithExpression ParseGroup(Token groupBodyToken, TokenCursor tokens)
        {
            Func<Token, bool> endOfExpression = t => t.Type == TokenType.CParenthesis;
            if (tokens.EndOfCursor())
            {
                throw new ParsingException("Parentesis mismatch");
            }
            if (endOfExpression(groupBodyToken))
            {
                throw new ParsingException("Empty expression group");
            }
            var expression = ParseExpression(groupBodyToken, endOfExpression, tokens, false);
            if (expression.IsNull())
            {
                throw new ParsingException("Empty expression group");
            }
            if (tokens.EndOfCursor())
            {
                throw new ParsingException("Parentesis mismatch");
            }
            return expression;
        }

        private static IArithExpression ParseOperator(Token operatorToken, IArithExpression lOperand, TokenCursor tokens)
        {
            var rOperand = ParseRightOperand(tokens.Next(), operatorToken, tokens);
            return OperatorExpression.Create(operatorToken.Value, lOperand, rOperand);
        }

        private static IArithExpression ParseRightOperand(Token operandToken, Token operatorToken, TokenCursor tokens)
        {
            /*Func<Token, bool> endOfOperand = t => t.Type == TokenType.AdditiveOperator || t.Type == TokenType.CParenthesis;
            return ParseExpression(operandToken, endOfOperand, tokens, true);*/

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
            Func<Token, bool> endOfOperand = t => t.Type == TokenType.AdditiveOperator || t.Type == TokenType.CParenthesis;
            return ParseExpression(operandToken, endOfOperand, tokens, true);
        }
        private static IArithExpression ParseMultiplicativeRightOperand(Token operandToken, TokenCursor tokens)
        {
            Func<Token, bool> endOfOperand = t => t.Type == TokenType.AdditiveOperator || t.Type == TokenType.CParenthesis || t.Type == TokenType.MultiplicativeOperator;
            return ParseExpression(operandToken, endOfOperand, tokens, true);
        }

        private static IArithExpression ParseValue(Token valueToken)
        {
            return new ValueExpression(double.Parse(valueToken.Value, NumberStyles.Any, CultureInfo.InvariantCulture));
        }
    }

}