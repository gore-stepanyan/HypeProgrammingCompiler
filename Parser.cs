using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace HypeProgrammingCompiler
{
    class Parser
    {
        // Исходный текст
        private string text;
        // Список ошибок для вывода и нейтралищации
        public ObservableCollection<Error> errorList = new ObservableCollection<Error>();
        // Список лексем - результат декомпозиции текста
        private LexemList lexemList = new LexemList();
        private string fixedString = "";
        // Исправленная строка
        public string FixedString 
        {
            get
            {
                foreach (var lexem in lexemList.lexems)
                {
                    fixedString += lexem.Symbol + " ";
                }
                return fixedString;
            }
        }


        // Перечесление кодов ошибок
        public enum ErrorCode 
        {
            NoIdentifier = 1,
            NoOperator = 2,
            NoSemicolon = 3,
            InvalidOperator = 4,
            InvalidCharacter = 5
        }

        //Класс Error - содержит информацию об ошибке и её код
        public class Error
        {            
            public string Info { get; set; }
            public ErrorCode Code { get; set; }
            public string Symbol { get; set; }
            public int StringNumber { get; set; }
            public int StartPosition { get; set; }
            public int EndPosition { get; set; }

            public Error(string info, ErrorCode code, string symbol, int stringNumber, int startPosition, int endPoition)
            {
                Info = info;
                Code = code;
                Symbol = symbol;
                StringNumber = stringNumber;
                StartPosition = startPosition;
                EndPosition = endPoition;
            }
            public Error(string info, ErrorCode code, int stringNumber, int startPosition, int endPoition)
            {
                Info = info;
                Code = code;
                StringNumber = stringNumber;
                StartPosition = startPosition;
                EndPosition = endPoition;
            }
        }

        public Parser(string text) 
        { 
            this.text = text;
            errorList.CollectionChanged += ErrorList_CollectionChanged;
        }

        // Нейтрализация ошибок при их обнаружении
        private void ErrorList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Error error = e.NewItems[0] as Error;

            switch (error.Code)
            {
                case ErrorCode.NoIdentifier: 
                    lexemList.Insert(new Lexem(LexemType.Identifier, "<Identifier>", lexemList.Current.StringNumber, lexemList.Current.StartPosition, 0)); 
                    break;
                case ErrorCode.NoOperator: 
                    lexemList.Insert(new Lexem(LexemType.Conjunction, "||", lexemList.Current.StringNumber, lexemList.Current.StartPosition, 0)); 
                    break;
                case ErrorCode.NoSemicolon: 
                    lexemList.Insert(new Lexem(LexemType.Semicolon, ";", lexemList.Current.StringNumber, lexemList.Current.StartPosition, 0)); 
                    break;
                case ErrorCode.InvalidOperator:
                    if (lexemList.Current.Symbol == "|")
                        lexemList.Insert(new Lexem(LexemType.Conjunction, "||", lexemList.Current.StringNumber, lexemList.Current.StartPosition, 0));
                    else
                        lexemList.Insert(new Lexem(LexemType.Conjunction, "&&", lexemList.Current.StringNumber, lexemList.Current.StartPosition, 0));

                    lexemList.RemoveNext(lexemList.Current); 
                    break;
            }

        }

        public string Print()
        {
            string result = "";
            foreach (var error in errorList)
            {
                result += "Стр. " + error.StringNumber + " ";
                result += error.Info + '\n';
            }
            return result;
        }

        private bool IsErrorLexem(Lexem lexem) // Предикат определения недопустимых лексем и их удаления
        {
            if (lexem.Type == LexemType.ErrorToken)
            {
                errorList.Add(new Error("Встречен недопустимый символ", ErrorCode.InvalidCharacter, lexem.Symbol, lexem.StringNumber, lexem.StartPosition, lexem.EndPosition));
                return true;
            }
            return false;
        }

        public void Parse()
        {
            Lexer lexer = new Lexer(text);
            lexer.Analyze(); // Декомпозиция текста на лексемы
            lexemList = lexer.lexemList;

            // Нейтрализация недопустимых символов
            lexemList.lexems.RemoveAll(IsErrorLexem);

            if (lexemList.Count > 0)
            {
                do
                {
                    // Если пропущен идентификатор
                    if (lexemList.Current.Type != LexemType.Identifier)
                        errorList.Add(new Error("Пропущен идентификатор", ErrorCode.NoIdentifier, lexemList.Current.StringNumber, lexemList.Current.StartPosition, lexemList.Current.EndPosition));

                    lexemList.Next();
                    // Если оператор пропущен или некорректен
                    if (lexemList.Current.Type != LexemType.Disjunction &&
                        lexemList.Current.Type != LexemType.Conjunction)
                    {
                        if (lexemList.Current.Type == LexemType.ErrorOperator)
                            errorList.Add(new Error("Некорректный логический оператор", ErrorCode.InvalidOperator, lexemList.Current.Symbol, lexemList.Current.StringNumber, lexemList.Current.StartPosition, lexemList.Current.EndPosition));
                        else
                            errorList.Add(new Error("Пропущен логический оператор", ErrorCode.NoOperator, lexemList.Prev.StringNumber, lexemList.Current.StartPosition, lexemList.Current.EndPosition));
                    }

                    lexemList.Next();
                    //Если пропущен идентификатор
                    if (lexemList.Current.Type != LexemType.Identifier)
                        errorList.Add(new Error("Пропущен идентификатор", ErrorCode.NoIdentifier, lexemList.Current.StringNumber, lexemList.Current.StartPosition, lexemList.Current.EndPosition));

                    // Итерация * (Замыкание Клини)
                    while (lexemList.Next() && lexemList.Current.Type != LexemType.Semicolon)
                    {
                        if (lexemList.Current.Type != LexemType.Disjunction &&
                            lexemList.Current.Type != LexemType.Conjunction)
                        {
                            if (lexemList.Current.Type == LexemType.ErrorOperator)
                                errorList.Add(new Error("Некорректный логический оператор", ErrorCode.InvalidOperator, lexemList.Current.Symbol, lexemList.Current.StringNumber, lexemList.Current.StartPosition, lexemList.Current.EndPosition));
                            else
                                errorList.Add(new Error("Пропущен логический оператор", ErrorCode.NoOperator, lexemList.Prev.StringNumber, lexemList.Current.StartPosition, lexemList.Current.EndPosition));
                        }

                        lexemList.Next();
                        if (lexemList.Current.Type != LexemType.Identifier)
                            errorList.Add(new Error("Пропущен идентификатор", ErrorCode.NoIdentifier, lexemList.Current.StringNumber, lexemList.Current.StartPosition, lexemList.Current.EndPosition));
                    }

                    // Если пропущено ";"
                    if (lexemList.Current.Type != LexemType.Semicolon)
                        errorList.Add(new Error("Пропущено \";\"", ErrorCode.NoSemicolon, lexemList.Current.StringNumber, lexemList.Current.StartPosition, lexemList.Current.EndPosition));
                }
                while (lexemList.Next()); // Цикл до последней лексемы включительно
            }
        }
    }
}