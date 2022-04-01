using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypeProgrammingCompiler
{
    class DFSMachine
    {
        private List<string> strings = new List<string>();
        private int i;
        public string Statements;

        public DFSMachine(string text) 
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
            try
            {
                Zero(textString);
            }
            catch (Exception)
            {
                Statements += "Error: ";
                Statements += "Invalid Sequence\n";
            }
        }

        private void Zero(string textString)
        {
            switch (textString[i]) 
            {
                case 'a': Statements += "0->"; One(textString); break;
                default: Statements += "0->"; Error(); break;
            }
        }

        private void One(string textString)
        {
            Statements += "1->";
            i++;
            switch (textString[i])
            {
                case 'b':Two(textString); break;
                default: Error(); break;
            }
        }

        private void Two(string textString)
        {
            Statements += "2->";
            i++;
            switch (textString[i])
            {
                case 'c': Three(textString); break;
                default: Error(); break;
            }
        }

        private void Three(string textString)
        {
            Statements += "3->";
            i++;
            switch (textString[i])
            {
                case 'a': Four(textString); break;
                case 'd': Seven(textString); break;
                default: Error(); break;
            }
        }
        
        private void Four(string textString)
        {
            Statements += "4->";
            i++;
            switch (textString[i])
            {
                case 'c': Five(textString); break;
                default:  Error(); break;
            }
        }

        private void Five(string textString)
        {
            Statements += "5->";
            i++;
            switch (textString[i])
            {
                case 'a': Six(textString); break;
                case 'd': Seven(textString); break;
                default: Error(); break;
            }
        }

        private void Six(string textString)
        {
            Statements += "6->";
            i++;
            switch (textString[i])
            {
                case 'c':; Five(textString); break;
                default: ; Error(); break;
            }
        }

        private void Seven(string textString)
        {
            Statements += "7->";
            i++;
            switch (textString[i])
            {
                case 'e': Eight(textString); break;
                case 'f': Nine(textString); break;
                default: Error(); break;
            }
        }

        private void Eight(string textString)
        {
            Statements += "8->";
            i++;
            switch (textString[i])
            {
                case 'e': Eight(textString); break;
                case 'f': Nine(textString); break;
                default: Error(); break;
            }
        }

        private void Nine(string textString)
        {
            Statements += "9: ";

            if (textString[i + 1] != '\r')
            {
                Statements += "Unexpected Symbol: " + textString.Substring(i + 1);
                return;
            }
            else
                Statements += "Valid Sequence\n";
        }

        private void Error()
        {
            throw new Exception();
        }
    }
}
