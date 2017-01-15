using System;

namespace MathEvaluator.Tests
{
    internal class ParsingException : Exception
    {
        public ParsingException(string msg): base(msg)
        {
            
        }
    }
}