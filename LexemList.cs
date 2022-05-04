using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypeProgrammingCompiler
{
    class LexemList
    {
        public List<Lexem> lexems = new List<Lexem>();
        private int i = 0;

        public LexemList() { }
        public LexemList(List<Lexem> lexems) { this.lexems = lexems; }

        public Lexem Current // Возврат лексемы на которую указывает индекс
        {
            get
            {
                if (i >= lexems.Count)
                {
                    return new Lexem(0, null, lexems[i - 1].StringNumber, 0, 0);
                }
                return lexems[i];
            }
        }

        public bool Next() // Перемещение индекса вперёд
        {
            i++;
            if (i < lexems.Count)
            {
                return true;
            }
            else
                return false;
        }

        public Lexem Prev // Возврат предыдущей лексемы от текщуего индекса
        {
            get
            {
                if (i > 0)
                    return lexems[i - 1];
                else
                    return new Lexem(0, null, 0, 0, 0);
            }
        }

        public void Add(Lexem lexem) // Вставка в конец
        {
            lexems.Add(lexem);
        }

        public int Count // Возврат числа лексем в списке
        {
            get
            {
                return lexems.Count;
            }
        }
        
        public void RemoveNext(Lexem lexem) // Удаление лексемы следующей за текущей
        {
            lexems.RemoveAt(lexems.IndexOf(lexem) + 1);
        }

        public void Insert(Lexem lexem) // Вставка лексемы по текущему индексу
        {
            lexems.Insert(i , lexem);
        }       
    }
}
