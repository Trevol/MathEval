using System;

namespace MathEvaluator.Parsing
{
    internal class ParsingException : Exception
    {
        public ParsingException(string msg): base(msg)
        {
            
        }
    }
}