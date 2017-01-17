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
                /*if (endOfExpression(current))
                {
                    if (moveToPrevAtEnd)
                    {
                        tokens.MovePrev();
                    }
                    break;
                }*/
                resultExpr = ParseToken(current, resultExpr, tokens);

                /*if (tokens.Current().IsNotNull() && endOfExpression(tokens.Current()))
                {
                    if (moveToPrevAtEnd)
                    {
                        tokens.MovePrev();
                    }
                    break;
                }*/

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

    /*
        public class Parser
        {
            public IArithExpression Parse(string expressionText)
            {
                var lexer = new Lexer(expressionText);

                using (var tokens = lexer.Tokens().GetEnumerator())
                {
                    IArithExpression prevExpr=null;
                    while (tokens.MoveNext())
                    {
                        prevExpr= Parse(tokens.Current, tokens, prevExpr);
                    }
                    if (prevExpr.IsNull())
                    {
                        //TODO: provide more usefull msg
                        throw new ParsingException("prevExpr.IsNull()");
                    }
                    return prevExpr;
                }
            }

            private static IArithExpression ParseExpression(Token start, Func<bool, Token> end)
            {
                throw new NotImplementedException();
            }

            private static IArithExpression Parse(Token token, IEnumerator<Token> tokens, IArithExpression prevExpression = null)
            {
                if (token.IsNull())
                {
                    return null;
                }
                switch (token.Type)
                {
                    case TokenType.Value:
                        return ParseValue(token);                    
                    case TokenType.AdditiveOperator:
                        return ParseOperator(token, tokens, prevExpression);                    
                    default:
                        throw new ParsingException("Unexpected token: " + token);
                }
            }

            private static IArithExpression ParseRightOperand(Token token, IEnumerator<Token> tokens)
            {
                //operators precedence:
                //- test for multiplicative operator - build one first
                //- test for expression group - (....)
                //- test for function call - ID(args)
                //prevExpression is not applicable in case of ROperand parsing
                return Parse(token, tokens);
            }

            private static IArithExpression ParseOperator(Token operatorToken, IEnumerator<Token> tokens, IArithExpression lOperand)
            {
                //throw new NotImplementedException("ParseAdditiveOperator and ParseMultiplicativeOperator or Expression Precedence/Priority");
                IArithExpression rOperand = ParseRightOperand(tokens.Next(), tokens);
                return OperatorExpression.Create(operatorToken.Value, lOperand, rOperand);
            }

            private static IArithExpression ParseValue(Token valueToken)
            {            
                return new ValueExpression(double.Parse(valueToken.Value, NumberStyles.Any, CultureInfo.InvariantCulture));            
            }
        }
    */
}