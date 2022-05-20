using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypeProgrammingCompiler
{
    class Lexer
    {
        public LexemList lexemList = new LexemList(); // Список лексем для заполнения
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
                result += lexemList.Current.Type + " - ";
                result += lexemList.Current.Symbol + " - ";
                result += "S: " + (lexemList.Current.StringNumber) + " - ";
                result += "P: " + lexemList.Current.StartPosition + "-";
                result += lexemList.Current.EndPosition + ";\n";
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

            lexemList.Add(new Lexem(LexemType.Identifier, identifier, stringNumber + 1, startPosition, endPosition));
            i--;
        }

        private void MatchDisjunction(string textString)
        {
            if (textString.Length != i + 1)
            {
                if (textString[i + 1] == '|')
                {
                    lexemList.Add(new Lexem(LexemType.Disjunction, "||", stringNumber + 1, i + 1, i + 2));
                    i++;
                    return;
                }
            }
            // Парсим некорректный оператор
            lexemList.Add(new Lexem(LexemType.ErrorOperator, textString[i].ToString(), stringNumber + 1, i + 1, i + 1));
        }

        private void MatchConjunction(string textString)
        {
            if (textString.Length != i + 1)
            {
                if (textString[i + 1] == '&')
                {
                    lexemList.Add(new Lexem(LexemType.Disjunction, "&&", stringNumber + 1, i + 1, i + 2));
                    i++;
                    return;
                }
            }
            // Парсим некорректный оператор
            lexemList.Add(new Lexem(LexemType.ErrorOperator, textString[i].ToString(), stringNumber + 1, i + 1, i + 1));
        }

        private void MatchSemicolon()
        {
            lexemList.Add(new Lexem(LexemType.Semicolon, ";", stringNumber + 1, i + 1, i + 1));
        }

        private void MatchErrorToken(string textString)
        {
            lexemList.Add(new Lexem(LexemType.ErrorToken, textString[i].ToString(), stringNumber + 1, i + 1, i + 1));
        }
    }
}
