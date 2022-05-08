using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypeProgrammingCompiler
{
    public enum LexemType // Перечень типов лексем
    {
        ErrorOperator = -1,
        ErrorToken = 0,
        Identifier = 1,
        Disjunction = 2,
        Conjunction = 3,
        Semicolon = 4
    }

    // Предоставляет хранение свойств токена и доступ к ним
    public class Lexem
    {
        public LexemType Type { get; set; }
        public string Symbol { get; set; }
        public int StringNumber { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }

        public Lexem(LexemType type, string symbol, int stringNumber, int startPosition, int endPosition)
        {
            Type = type;
            Symbol = symbol;
            StringNumber = stringNumber;
            StartPosition = startPosition;
            EndPosition = endPosition;
        }
    }
}
