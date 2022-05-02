using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypeProgrammingCompiler
{
    public class Lexem
    {
        public enum Type
        {
            ErrorOperator = -1,
            ErrorToken = 0,
            Identifier = 1,
            Disjunction = 2,
            Conjunction = 3,
            Semicolon = 4
        }

        public Type type;
        public string symbol;
        public int stringNumber;
        public int startPosition;
        public int endPosition;

        public Lexem(Type type, string symbol, int stringNumber, int startPosition, int endPosition)
        {
            this.type = type;
            this.symbol = symbol;
            this.stringNumber = stringNumber;
            this.startPosition = startPosition;
            this.endPosition = endPosition;
        }
    }
}
