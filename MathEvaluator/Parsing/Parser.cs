﻿using System;
using System.Collections.Generic;
using System.Globalization;
using MathEvaluator.Expressions;
using MathEvaluator.Tests;
using MathEvaluator.Utils;

namespace MathEvaluator.Parsing
{
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
}