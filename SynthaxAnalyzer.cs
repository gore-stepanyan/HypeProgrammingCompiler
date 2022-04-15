using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypeProgrammingCompiler
{
    class SynthaxAnalyzer
    {
        private List<string> strings = new List<string>();
        private int i; // Индекс символа
        private List<string> errorMessages = new List<string>();
        private int errorStartPosition;
        private int errorStringNumber;
        public string States;

        public SynthaxAnalyzer(string text)
        {
            strings = text.Split("\n").ToList();
            strings[strings.Count - 1] += "\r";
        }
        
        public void Analyze()
        {
            foreach (string textString in strings)
                AnalyzeString(textString);
        }
        
        private void AnalyzeString(string textString)
        {
            i = 0;
            while (textString[i] == ' ')
                i++;
            if (textString[i] == '\r')
            {
                States += "Пустая строка\n";
                return;
            }
            if (textString[i] == ';')
            {
                States += "Пустое выражение\n";
                return;
            }
            try
            {
                LogicalExpression(textString);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (errorMessages.Count > 0)
                {
                    States += "Найдены ошибки:\n";
                    foreach (string message in errorMessages)
                        States += message + '\n';
                }
                else 
                {
                    States += "Ошибки не найдены\n";
                }
            }
        }

        private void LogicalExpression(string textString)
        {
            States += "LogicalExpression->";

            Identifier(textString);
            LogicalOperator(textString);
            Identifier(textString);

            while (textString[i] == ' ')
                i++;

            while (textString[i] != '\r' && textString[i] != ';')
            {
                LogicalOperator(textString);
                Identifier(textString);
                
                while (textString[i] == ' ')
                    i++;
            }

            while (textString[i] == ' ')
                i++;

            if (textString[i] != ';')
            {
                errorStartPosition = i;
                errorMessages.Add("Требуется \";\"");
            }
            
        }

        private void Identifier(string textString)
        {
            States += "Identifier->";

            while (textString[i] == ' ')
                i++;

            if (Char.IsLetter(textString[i]))
            {
                i++;
                //while (Char.IsLetterOrDigit(textString[i]) && textString[i] != 'x' && textString[i + 1] != 'o' && textString[i + 2] != 'r')
                while (Char.IsLetterOrDigit(textString[i]))
                {
                    i++;
                }
            }
            else if (textString[i] != '|' && textString[i] != '&' && textString[i] != '\r' && textString[i] != ';')
            {
                i++;
                while (textString[i] != '|' && textString[i] != '&' && !Char.IsLetterOrDigit(textString[i]) && textString[i] != '\r' && textString[i] != ';')
                {
                    i++;
                }
                errorStartPosition = i;
                errorMessages.Add("Неизвестная последовательность символов");
                errorMessages.Add("Идентификатор отсутствует или введён некорректно");
            }
            else
            {
                errorStartPosition = i;
                errorMessages.Add("Идентификатор отсутствует или введён некорректно");
            }

        }

        private void LogicalOperator(string textString)
        {
            States += "LogicalOperator->";
            
            while (textString[i] == ' ')
                i++;

            if (textString[i] == '&')
            { 
                i++;
                if (textString[i] == '&')
                    i++;
                else
                {
                    errorStartPosition = i;
                    errorMessages.Add("Логический оператор отсутствует или введён некорректно");
                }
            }
            else if (textString[i] == '|')
            {
                i++;
                if (textString[i] == '|')
                    i++;
                else
                {
                    errorStartPosition = i;
                    errorMessages.Add("Логический оператор отсутствует или введён некорректно");
                }
            }
            else if (textString[i] == 'x' && textString[i + 1] == 'o' && textString[i + 2] == 'r')
                i += 3;
            else
            {
                errorStartPosition = i;
                errorMessages.Add("Логический оператор отсутствует или введён некорректно");
            }
        }

    }
}
