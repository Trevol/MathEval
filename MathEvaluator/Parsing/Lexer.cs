using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MathEvaluator.Tests;
using MathEvaluator.Utils;

namespace MathEvaluator.Parsing
{
    class Lexer
    {
        private static readonly Regex TokenMatcher = GetTokenMatcher();
        private readonly string _expressionText;

        public Lexer(string expressionText)
        {
            _expressionText = expressionText;
        }

        /*public Token NextToken()
        {
            throw new NotImplementedException();
        }*/

        public IEnumerable<Token> Tokens()
        {
            if (_expressionText.IsNullOrEmpty())
            {
                throw new ParsingException("Expression is null or empty");
            }
            return TokenMatcher.Matches(_expressionText).Cast<Match>().Select(DetectToken);
        }
        private static Token DetectToken(Match match)
        {
            foreach (var tokenType in TokenTypes())
            {
                var value = ValueByTokenType(match, tokenType);
                if (value.IsNotNullOrEmpty())
                {
                    return new Token(tokenType, value);
                }
            }
            return new Token(TokenType.Unknown, match.Value);
        }
        private static string ValueByTokenType(Match match, TokenType tokenType)
        {
            return match.Result($"${{{tokenType}}}");
        }
        private static Dictionary<TokenType, string> TokenPatterns()
        {
            return new Dictionary<TokenType, string>()
            {
                {TokenType.Value, @"\d+(\.\d+)?" },
                {TokenType.AdditiveOperator, @"[+|\-]" },
                {TokenType.MultiplicativeOperator, @"[*|/]" },
                {TokenType.OParenthesis, @"\(" },
                {TokenType.CParenthesis, @"\)" }
            };
        }

        private static IEnumerable<TokenType> TokenTypes()
        {
            return Enum.GetValues(typeof(TokenType)).Cast<TokenType>();
        }
        private static Regex GetTokenMatcher()
        {
            var pattern = string.Join("|", TokenPatterns().Select(p => $"(?<{p.Key}>{p.Value})"));
            return new Regex(pattern, RegexOptions.Compiled);
        }
    }
}
