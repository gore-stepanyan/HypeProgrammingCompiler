using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypeProgrammingCompiler
{
    class Scanner
    {
        public List<(Lexem lexem, string codeSymbol, int stringNumber, int startPosition, int endPosition)> decomposedCodeStrings = new List<(Lexem lexem, string codeSymbol, int stringNumber, int startPosition, int endPosition)>();
        private List<string> codeStrings = new List<string>();
        int i = 0; // Индекс символа
        int stringNumber;
        
        public Scanner(string code) { this.codeStrings = code.Split("\r").ToList(); }

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

            for (int i = 0; i < decomposedCodeStrings.Count; i++)
            {
                result += (int)decomposedCodeStrings[i].lexem + " - ";
                result += decomposedCodeStrings[i].lexem + " - ";
                result += decomposedCodeStrings[i].codeSymbol + " - ";
                result += "S: " + (decomposedCodeStrings[i].stringNumber + 1) + " - ";
                result += "P: " + decomposedCodeStrings[i].startPosition + "-";
                result += decomposedCodeStrings[i].endPosition + ";\n";
            }
            
            return result;
        }

        private void Prepare()
        {
            for (int i  = 0; i < codeStrings.Count; i++)
                codeStrings[i] = codeStrings[i].Replace("\n", "");
        }

        public void Analyze()
        {
            Prepare();

            for (stringNumber = 0; stringNumber < codeStrings.Count; stringNumber++)
            {
                i = 0;
                AnalyzeString(codeStrings[stringNumber]);
            }
        }

        private void AnalyzeString(string codeString)
        {
            for(; i < codeString.Length; i++)
            {
                if (char.IsLetter(codeString[i]))
                {
                    MatchIdentifier(codeString);
                    if (codeString.Length <= i)
                        break;
                }
                else if (codeString[i] == '=')
                {
                    MatchAssignOperator();
                    if (codeString.Length <= i)
                        break;
                }
                else if (codeString[i] == ' ')
                {
                    MatchSpaceSymbol();
                    if (codeString.Length <= i)
                        break;
                }
                else if (char.IsDigit(codeString[i]))
                {
                    MatchDigitOrFloat(codeString);
                    if (codeString.Length <= i)
                        break;
                }
                else if (codeString[i] == '+')
                {
                    MatchPlusOperator();
                    if (codeString.Length <= i)
                        break;
                }
                else if (codeString[i] == '-')
                {
                    MatchMinusOperator();
                    if (codeString.Length <= i)
                        break;
                }
                else if (codeString[i] == '*')
                {
                    MatchMultOrExpOperator(codeString);
                    if (codeString.Length <= i)
                        break;
                }
                else if (codeString[i] == '/')
                {
                    MatchDivisionOrFloorDivisionOperator(codeString);
                    if (codeString.Length <= i)
                        break;
                }
                else if (codeString[i] == '%')
                {
                    MatchModulusOperator();
                    if (codeString.Length <= i)
                        break;
                }
                else
                {
                    if (codeString[i] != '\n' && codeString[i] != '\r')
                        MatchInvalid(i, codeString[i].ToString());
                }

            }
        }

        private void MatchIdentifier(string codeString)
        {
            string codeSymbol = "";
            int startPosition = i + 1;

            while (char.IsLetterOrDigit(codeString[i]))
            {
                codeSymbol += codeString[i];
                i++;

                if (codeString.Length <= i)
                    break;
            }
            int endPosition = i;

            decomposedCodeStrings.Add((Lexem.Identifier, codeSymbol, stringNumber, startPosition, endPosition));
            i--;
        }

        private void MatchAssignOperator()
        {
            decomposedCodeStrings.Add((Lexem.AssignOperator, "=", stringNumber, i + 1, i + 1));
        }

        private void MatchSpaceSymbol()
        {
            decomposedCodeStrings.Add((Lexem.Space, "(space)", stringNumber, i + 1, i + 1));
        }

        private void MatchDigitOrFloat(string codeString)
        {
            string codeSymbol = "";
            int startPosition = i + 1;

            while (char.IsDigit(codeString[i]))
            {
                codeSymbol += codeString[i];
                i++;

                if (codeString.Length <= i)
                    break;

                if (codeString[i] == '.')
                {
                    MatchFloat(codeString, startPosition, codeSymbol);
                    return;
                }

            }
            int endPosition = i;

            decomposedCodeStrings.Add((Lexem.Integer, codeSymbol, stringNumber, startPosition, endPosition));
            i--;
        }

        private void MatchFloat(string codeString, int startPosition, string codeSymbol)
        {
            if (codeString.Length == i + 1)
            {
                MatchInvalid(startPosition, codeSymbol += codeString[i]);
                return;
            }

            if (!char.IsDigit(codeString[i + 1]))
            {
                MatchInvalid(startPosition, codeSymbol += codeString[i]);
                //i--;
                return;
            }

            do
            {
                codeSymbol += codeString[i];
                i++;

                if (codeString.Length <= i)
                    break;

            } while (char.IsDigit(codeString[i]));
            int endPosition = i;

            decomposedCodeStrings.Add((Lexem.Float, codeSymbol, stringNumber, startPosition, endPosition));
            i--;
        }

        private void MatchPlusOperator()
        {
            decomposedCodeStrings.Add((Lexem.PlusOperator, "+", stringNumber, i + 1, i + 1));
        }

        private void MatchMinusOperator()
        {
            decomposedCodeStrings.Add((Lexem.MinusOperator, "-", stringNumber, i + 1, i + 1));
        }


        private void MatchDivisionOrFloorDivisionOperator(string codeString)
        {
            if (codeString.Length != i + 1)
            {
                if (codeString[i + 1] == '/')
                {
                    MatchFloorDivisionOperator();
                    return;
                }
            }
            decomposedCodeStrings.Add((Lexem.DivisionOperator, "/", stringNumber, i + 1, i + 1));
        }

        private void MatchFloorDivisionOperator()
        {
            decomposedCodeStrings.Add((Lexem.FloorDivisionOperator, "//", stringNumber, i + 1, i + 2));
            i++;
        }

        private void MatchMultOrExpOperator(string codeString)
        {
            if (codeString.Length != i + 1)
            {
                if (codeString[i + 1] == '*')
                {
                    MatchExponentiationOperator();
                    return;
                }
            }
            decomposedCodeStrings.Add((Lexem.MultiplicationOperator, "*", stringNumber, i + 1, i + 1));
        }

        private void MatchExponentiationOperator()
        {
            decomposedCodeStrings.Add((Lexem.ExponentiationOperator, "**", stringNumber, i + 1, i + 2));
            i++;
        }

        private void MatchModulusOperator()
        {
            decomposedCodeStrings.Add((Lexem.ModulusOperator, "%", stringNumber, i + 1, i + 1));
        }

        private void MatchInvalid(int startPosition, string codeSymbol)
        {
            decomposedCodeStrings.Add((Lexem.Invalid, codeSymbol, stringNumber, startPosition, i + 2));
        }

    }
}
