using System;

namespace MathEvaluator.Parsing
{
    public class ParserException : Exception
    {
        public ParserException(string msg): base(msg)
        {
            
        }
    }
}