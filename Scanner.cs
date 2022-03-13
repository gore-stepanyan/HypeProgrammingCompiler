using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypeProgrammingCompiler
{
    class Scanner
    {
        public List<(int codeNumber, string codeIdentifier, string codeSymbol, int startPosition, int endPosition)> decomposedText = new List<(int codeNumber, string codeIdentifier, string codeSymbol, int startPosition, int endPosition)>();
        private string text;
        
        public Scanner(string text) { this.text = text; }

        private enum Lexems
        {
            Identifier = 0,
            AssignOperator = 1,
            Separator = 2,
            Integer = 3,
            Float = 4,
            PlusOperator = 5,
            MinusOperator = 6,
            MultOperator = 7,
            DvideOperator = 8,
            DoubleSlashOperator = 9,
            PercentOperator = 10,
            DoubleStarOperator = 11,
            Error = -1
        }

        public string Print()
        {
            string result = "";

            foreach (var decomposition in decomposedText)
            {
                result += decomposition.codeNumber.ToString() + " - ";
                result += decomposition.codeIdentifier.ToString() + " - ";
                result += decomposition.codeSymbol + " - ";
                result += decomposition.startPosition.ToString() + " - ";
                result += decomposition.endPosition.ToString() + ";\n";
            }
            
            return result;
        }

        int i = 0;
        public void Analyze()
        {
            for(; i < text.Length; i++)
            {
                if (char.IsLetter(text[i]))
                {
                    MatchIdentifier();
                }
            }
        }

        private void MatchIdentifier()
        {
            string identifier = "";
            int startPosition = i + 1;
            //identifier += text[i];

            while (char.IsLetterOrDigit(text[i]))
            {
                identifier += text[i];
                i++;

                if (text.Length <= i)
                    break;
            }
            int endPosition = i;

            decomposedText.Add(((int)Lexems.Identifier, Lexems.Identifier.ToString(), identifier, startPosition, endPosition));
        }

    }
}
