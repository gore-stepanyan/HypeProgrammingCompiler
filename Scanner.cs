using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypeProgrammingCompiler
{
    class Scanner
    {
        public List<(Lexem lexem, string codeSymbol, int startPosition, int endPosition)> decomposedText = new List<(Lexem lexem, string codeSymbol, int startPosition, int endPosition)>();
        private string text;
        
        public Scanner(string text) { this.text = text.Trim('\n', '\r', '\0'); }

        public enum Lexem
        {
            Identifier = 0,
            AssignOperator = 1,
            Space = 2,
            Integer = 3,
            Float = 4,
            PlusOperator = 5,
            MinusOperator = 6,
            MultiplicationOperator = 7,
            DivisionOperator = 8,
            FloorDivisionOperator = 9,
            ModulusOperator = 10,
            ExponentiationOperator = 11,
            Invalid = -1
        }

        public string Print()
        {
            string result = "";

            foreach (var decomposition in decomposedText)
            {
                result += (int)decomposition.lexem + " - ";
                result += decomposition.lexem + " - ";
                result += decomposition.codeSymbol + " - ";
                result += decomposition.startPosition + " - ";
                result += decomposition.endPosition + ";\n";
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
                    if (text.Length <= i)
                        break;
                }
                else if (text[i] == '=')
                {
                    MatchAssignOperator();
                    if (text.Length <= i)
                        break;
                }
                else if (text[i] == ' ')
                {
                    MatchSpaceSymbol();
                    if (text.Length <= i)
                        break;
                }
                else if (char.IsDigit(text[i]))
                {
                    MatchDigitOrFloat();
                    if (text.Length <= i)
                        break;
                }
                else if (text[i] == '+')
                {
                    MatchPlusOperator();
                    if (text.Length <= i)
                        break;
                }
                else if (text[i] == '-')
                {
                    MatchMinusOperator();
                    if (text.Length <= i)
                        break;
                }
                else if (text[i] == '*')
                {
                    MatchMultOrExpOperator();
                    if (text.Length <= i)
                        break;
                }
                else if (text[i] == '/')
                {
                    MatchDivisionOrFloorDivisionOperator();
                    if (text.Length <= i)
                        break;
                }
                else
                {
                    if (text[i] != '\n' && text[i] != '\r')
                        MatchInvalid(i, text[i].ToString());
                }

            }
        }

        private void MatchIdentifier()
        {
            string codeSymbol = "";
            int startPosition = i + 1;

            while (char.IsLetterOrDigit(text[i]))
            {
                codeSymbol += text[i];
                i++;

                if (text.Length <= i)
                    break;
            }
            int endPosition = i;

            decomposedText.Add((Lexem.Identifier, codeSymbol, startPosition, endPosition));
            i--;
        }

        private void MatchAssignOperator()
        {
            decomposedText.Add((Lexem.AssignOperator, "=", i + 1, i + 1));
        }

        private void MatchSpaceSymbol()
        {
            decomposedText.Add((Lexem.Space, "(space)", i + 1, i + 1));
        }

        private void MatchDigitOrFloat()
        {
            string codeSymbol = "";
            int startPosition = i + 1;

            while (char.IsDigit(text[i]))
            {
                codeSymbol += text[i];
                i++;

                if (text.Length <= i)
                    break;

                if (text[i] == '.')
                {
                    MatchFloat(startPosition, codeSymbol);
                    return;
                }

            }
            int endPosition = i;

            decomposedText.Add((Lexem.Integer, codeSymbol, startPosition, endPosition));
            i--;
        }

        private void MatchFloat(int startPosition, string codeSymbol)
        {
            if (text.Length == i + 1)
            {
                MatchInvalid(startPosition, codeSymbol += text[i]);
                return;
            }

            if (!char.IsDigit(text[i + 1]))
            {
                MatchInvalid(startPosition, codeSymbol += text[i]);
                //i--;
                return;
            }

            do
            {
                codeSymbol += text[i];
                i++;

                if (text.Length <= i)
                    break;

            } while (char.IsDigit(text[i]));
            int endPosition = i;

            decomposedText.Add((Lexem.Float, codeSymbol, startPosition, endPosition));
            i--;
        }

        private void MatchPlusOperator()
        {
            decomposedText.Add((Lexem.PlusOperator, "+", i + 1, i + 1));
        }

        private void MatchMinusOperator()
        {
            decomposedText.Add((Lexem.MinusOperator, "-", i + 1, i + 1));
        }


        private void MatchDivisionOrFloorDivisionOperator()
        {
            if (text.Length != i + 1)
            {
                if (text[i + 1] == '/')
                {
                    MatchFloorDivisionOperator();
                    return;
                }
            }
            decomposedText.Add((Lexem.DivisionOperator, "/", i + 1, i + 1));
        }

        private void MatchFloorDivisionOperator()
        {
            decomposedText.Add((Lexem.FloorDivisionOperator, "//", i + 1, i + 2));
            i++;
        }

        private void MatchMultOrExpOperator()
        {
            if (text.Length != i + 1)
            {
                if (text[i + 1] == '*')
                {
                    MatchExponentiationOperator();
                    return;
                }
            }
            decomposedText.Add((Lexem.MultiplicationOperator, "*", i + 1, i + 1));
        }

        private void MatchExponentiationOperator()
        {
            decomposedText.Add((Lexem.ExponentiationOperator, "**", i + 1, i + 2));
            i++;
        }

        private void MatchModulusOperator()
        {
            decomposedText.Add((Lexem.ModulusOperator, "%", i + 1, i + 1));
        }

        private void MatchInvalid(int startPosition, string codeSymbol)
        {
            decomposedText.Add((Lexem.Invalid, codeSymbol, startPosition, i + 2));
        }

    }
}
