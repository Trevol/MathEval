using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MathEvaluator.Parsing;
using MathEvaluator.Utils;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MathEvaluator.Tests
{
    [TestFixture]
    public class RegExpTests
    {
        [Test]
        public void DoTest()
        {
            var tokenDesc = new Dictionary<TokenType, string>()
            {
                {TokenType.Value, @"\d+(\.\d+)?" },
                {TokenType.AdditiveOperator, "[+|-|*|/]" }
            };

            var pattern = string.Join("|", tokenDesc.Select(p => $"(?<{p.Key}>{p.Value})"));

            foreach (Match m in Regex.Matches("2.345+3.9/6.78", pattern))
            {
                var token = DetectToken(m);
                Console.WriteLine($"{m.Value}, {token.Type}, {token.Value}");
            }
        }

        private static Token DetectToken(Match match)
        {
            foreach (TokenType tokenType in Enum.GetValues(typeof(TokenType)))
            {
                var value = ValueByTokenType(match, tokenType);
                if (value.IsNotNullOrEmpty())
                {
                    return new Token(tokenType, value);
                }
                /*var @group = match.Groups[name];
                if (@group != null)
                {
                    return new Token((TokenType)Enum.Parse(typeof(TokenType), name), @group.Value);
                }*/
            }

            return new Token(TokenType.Unknown, match.Value);
        }

        private static string ValueByTokenType(Match match, TokenType tokenType)
        {
            var q = $"${{{tokenType}}}";
            var r = match.Result(q);
            return r;
        }
    }
}