using System;
using System.Collections.Generic;
using System.Linq;

namespace MathEvaluator.Parsing
{
    internal class TokenCursor : IDisposable
    {
        private readonly Token[] _tokens;
        private int _position;

        public TokenCursor(IEnumerable<Token> tokens)
        {
            _position = -1;
            _tokens = tokens.ToArray();
        }

        public Token Current()
        {
            if (EndOfCursor() || StartOfCursor())
            {
                return null;
            }
            return _tokens[_position];
        }

        private bool EndOfCursor()
        {
            return _position == _tokens.Length;
        }

        public Token Next()
        {
            MoveNext();
            return Current();
        }

        private bool MoveNext()
        {
            if (EndOfCursor())
            {
                return false;
            }
            _position++;
            return EndOfCursor();
        }

        public Token Prev()
        {
            MovePrev();
            return Current();
        }

        public bool MovePrev()
        {
            if (StartOfCursor())
            {
                return false;
            }
            _position--;
            return StartOfCursor();
        }
        bool StartOfCursor()
        {
            return _position == -1;
        }

        public void Dispose()
        {
        }
    }
}