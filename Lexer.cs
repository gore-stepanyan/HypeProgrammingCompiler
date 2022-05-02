using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypeProgrammingCompiler
{
    class Lexer
    {
        public LexemList lexemList = new LexemList();
        private List<string> textStrings = new List<string>();
        private int i = 0; // Индекс символа
        private int stringNumber = 0; // Номер строки

        public Lexer(string text) 
        { 
            textStrings = text.Split("\n").ToList(); 
            textStrings[textStrings.Count - 1] += "\r"; 
        }

        public string Print()
        {
            string result = "";

            do
            {
                result += lexemList.Current.type + " - ";
                result += lexemList.Current.symbol + " - ";
                result += "S: " + (lexemList.Current.stringNumber) + " - ";
                result += "P: " + lexemList.Current.startPosition + "-";
                result += lexemList.Current.endPosition + ";\n";
            }
            while (lexemList.Next());

            return result;
        }

        public void Analyze()
        {
            foreach (string textString in textStrings)
            {
                AnalyzeString(textString);
                stringNumber++;
            }
        }

        private void AnalyzeString(string textString)
        {
            i = 0;
            for (; i < textString.Length; i++)
            {
                // Пропуск пробелов перед лексемой
                while (textString[i] == ' ') i++;

                // Если лексема начинается с буквы, то парсим идентификатор
                if (char.IsLetter(textString[i]))
                {
                    MatchIdentifier(textString);
                    if (textString.Length <= i)
                        break;
                }
                // Иначе парсим дизъюнкцию
                else if (textString[i] == '|')
                {
                    MatchDisjunction(textString);
                    if (textString.Length <= i)
                        break;
                }
                // Иначе парсим конъюнкцию
                else if (textString[i] == '&')
                {
                    MatchConjunction(textString);
                    if (textString.Length <= i)
                        break;
                }
                // Иначе парсим точку с запятой
                else if (textString[i] == ';')
                {
                    MatchSemicolon();
                    if (textString.Length <= i)
                        break;
                }
                // Иначе парсим неизвестный символ
                else
                {
                    if (textString[i] != '\r')
                        MatchErrorToken(textString);
                }
            }

        }

        private void MatchIdentifier(string textString)
        {
            string identifier = "";
            int startPosition = i + 1;

            while (char.IsLetterOrDigit(textString[i]))
            {
                identifier += textString[i];
                i++;

                if (textString.Length <= i)
                    break;
            }
            int endPosition = i;

            lexemList.Add(new Lexem(Lexem.Type.Identifier, identifier, stringNumber + 1, startPosition, endPosition));
            i--;
        }

        private void MatchDisjunction(string textString)
        {
            if (textString.Length != i + 1)
            {
                if (textString[i + 1] == '|')
                {
                    lexemList.Add(new (Lexem.Type.Disjunction, "||", stringNumber + 1, i + 1, i + 2));
                    i++;
                    return;
                }
            }
            lexemList.Add(new (Lexem.Type.ErrorOperator, textString[i].ToString(), stringNumber + 1, i + 1, i + 1));
        }

        private void MatchConjunction(string textString)
        {
            if (textString.Length != i + 1)
            {
                if (textString[i + 1] == '&')
                {
                    lexemList.Add(new (Lexem.Type.Disjunction, "&&", stringNumber + 1, i + 1, i + 2));
                    i++;
                    return;
                }
            }
            lexemList.Add(new (Lexem.Type.ErrorOperator, textString[i].ToString(), stringNumber + 1, i + 1, i + 1));
        }

        private void MatchSemicolon()
        {
            lexemList.Add(new (Lexem.Type.Semicolon, ";", stringNumber + 1, i + 1, i + 1));
        }

        private void MatchErrorToken(string textString)
        {
            lexemList.Add(new (Lexem.Type.ErrorToken, textString[i].ToString(), stringNumber + 1, i + 1, i + 1));
        }
    }
}
