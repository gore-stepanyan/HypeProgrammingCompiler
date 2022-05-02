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
        private string text;
        //public ObservableCollection<(string info, int code)> errorList = new ObservableCollection<(string info, int code)>();
        private ObservableCollection<Error> errorList = new ObservableCollection<Error>();
        private LexemList lexemList = new LexemList();

        //Класс Error - содержит информацию об ошибке и её код
        private class Error
        {
            public string info;
            public int code;

            public Error(string info, int code)
            {
                this.info = info;
                this.code = code;
            }
        }

        public Parser(string text) 
        { 
            this.text = text;
            errorList.CollectionChanged += ErrorList_CollectionChanged;
        }

        private void ErrorList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Error error = e.NewItems[0] as Error;

            if (error.code == 1)
            {
                lexemList.Insert(new Lexem(Lexem.Type.Identifier, null, 0, 0, 0));
            }
            else if (error.code == 2)
            {
                lexemList.Insert(new Lexem(Lexem.Type.Conjunction, null, 0, 0, 0));
            }
            else if (error.code == 3)
            {
                lexemList.Insert(new Lexem(Lexem.Type.Semicolon, null, 0, 0, 0));
            }
            else if (error.code == 4)
            {
                lexemList.Insert(new Lexem(Lexem.Type.Conjunction, null, 0, 0, 0));
                lexemList.RemoveNext(lexemList.Current);
            }
        }

        public string Print()
        {
            string result = "";
            foreach (var error in errorList)
            {
                result += error.info + '\n';
            }
            return result;
        }

        private bool IsErrorLexem(Lexem lexem)
        {
            if (lexem.type == Lexem.Type.ErrorToken)
            {
                errorList.Add(new Error("Встречен недопустимый символ", 5));
                return true;
            }
            return false;
        }

        public void Parse()
        {
            Lexer lexer = new Lexer(text);
            lexer.Analyze();
            lexemList = lexer.lexemList;

            //Нейтрализация всех "ошибочных" лексем
            lexemList.lexems.RemoveAll(IsErrorLexem);

            if (lexemList.Count > 0)
            {
                do
                {
                    if (lexemList.Current.type != Lexem.Type.Identifier)
                        errorList.Add(new Error("Пропущен идентификатор", 1));

                    lexemList.Next();
                    if (lexemList.Current.type != Lexem.Type.Disjunction &&
                        lexemList.Current.type != Lexem.Type.Conjunction)
                    {
                        if (lexemList.Current.type == Lexem.Type.ErrorOperator)
                            errorList.Add(new Error("Некорректный логический оператор", 4));
                        else
                            errorList.Add(new Error("Пропущен логический оператор", 2));
                    }

                    lexemList.Next();
                    if (lexemList.Current.type != Lexem.Type.Identifier)
                        errorList.Add(new Error("Пропущен идентификатор", 1));

                    while (lexemList.Next() && lexemList.Current.type != Lexem.Type.Semicolon)
                    {
                        if (lexemList.Current.type != Lexem.Type.Disjunction &&
                            lexemList.Current.type != Lexem.Type.Conjunction)
                        {
                            if (lexemList.Current.type == Lexem.Type.ErrorOperator)
                                errorList.Add(new Error("Некорректный логический оператор", 4));
                            else
                                errorList.Add(new Error("Пропущен логический оператор", 2));
                        }

                        lexemList.Next();
                        if (lexemList.Current.type != Lexem.Type.Identifier)
                            errorList.Add(new Error("Пропущен идентификатор", 1));
                    }

                    if (lexemList.Current.type != Lexem.Type.Semicolon)
                        errorList.Add(new Error("Пропущено \";\"", 3));
                }
                while (lexemList.Next());
            }
        }
    }
}