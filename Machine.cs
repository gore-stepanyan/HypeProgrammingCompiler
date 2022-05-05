using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypeProgrammingCompiler
{
    class Machine
    {
        private List<string> strings = new List<string>();
        public List<MatchedSubstring> MatchedSubstrings = new List<MatchedSubstring>();
        private string substring;
        private int startPosition;
        private int i;
        private int stringNumber;

        public Machine(string text)
        {
            strings = text.Split("\n").ToList();
            strings[strings.Count - 1] += "\r";
        }

        public void Analyze()
        {
            for(stringNumber = 0; stringNumber < strings.Count; stringNumber++)
                AnalyzeString(strings[stringNumber]);
        }

        private void AnalyzeString(string textString)
        {
            for(startPosition = 0; startPosition < textString.Length - 3; startPosition++)
            {
                i = startPosition;
                try
                {
                    Zero(textString);
                }
                catch (Exception) { }
            }
        }

        private void Zero(string textString)
        {
            substring = "";
            switch (textString[i])
            {
                case '0':; One(textString); break;
                case '1': Two(textString); break;
                default: Error(); break;
            }
        }

        private void One(string textString)
        {
            substring += textString[i];
            i++;
            switch (textString[i])
            {
                case '0': One(textString); break;
                case '1': Two(textString); break;
                default: Error(); break;
            }
        }

        private void Two(string textString)
        {
            substring += textString[i];
            i++;
            switch (textString[i])
            {
                case '0': Three(textString); break;
                case '1': Four(textString); break;
                default: Error(); break;
            }
        }

        private void Three(string textString)
        {
            substring += textString[i];
            i++;
            switch (textString[i])
            {
                case '0': Three(textString); break;
                case '1': Four(textString); break;
                default: Error(); break;
            }
        }

        private void Four(string textString)
        {
            substring += textString[i];
            i++;
            switch (textString[i])
            {
                case '0': Five(textString); break;
                case '1': Six(textString); break;
                default: Error(); break;
            }
        }

        private void Five(string textString)
        {
            substring += textString[i];
            i++;
            switch (textString[i])
            {
                case '0': Five(textString); break;
                case '1': Six(textString); break;
                default: Error(); break;
            }
        }

        private void Six(string textString)
        {
            substring += textString[i];
            i++;
            switch (textString[i])
            {
                case '0': Seven(textString); break;
                default: Eight(textString); break;
            }
        }

        private void Seven(string textString)
        {
            substring += textString[i];
            i++;
            switch (textString[i])
            {
                case '0': Seven(textString); break;
                default: Eight(textString); break;
            }
        }

        private void Eight(string textString)
        {             
            MatchedSubstrings.Add(new MatchedSubstring(substring, startPosition, stringNumber + 1));
            startPosition += substring.Length;
        }

        private void Error()
        {
            throw new Exception();
        }
    }
}