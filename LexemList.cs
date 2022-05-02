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

        public Lexem Current
        {
            get
            {
                if (i >= lexems.Count)
                {
                    return new Lexem(0, null, 0, 0, 0);
                }
                return lexems[i];
            }
        }

        public bool Next()
        {
            i++;
            if (i < lexems.Count)
            {
                return true;
            }
            else
                return false;
        }

        public void Add(Lexem lexem)
        {
            lexems.Add(lexem);
        }

        public int Count 
        {
            get
            {
                return lexems.Count;
            }
        }

        public Lexem this[int i]
        {
            get
            {
                return lexems[i];
            }
        }

        public void Remove(Lexem lexem)
        {
            lexems.Remove(lexem);
            if (i  >= lexems.Count)
                i--;
        }

        public void RemoveNext(Lexem lexem)
        {
            lexems.RemoveAt(lexems.IndexOf(lexem) + 1);
        }

        public void Insert(Lexem lexem)
        {
            lexems.Insert(i , lexem);
        }       
    }
}
