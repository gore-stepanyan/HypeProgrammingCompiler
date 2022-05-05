using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace HypeProgrammingCompiler
{
    public class MatchedSubstring
    {
        public string Text { get; set; }
        public int StringNumber { get; set; }
        public int StartPosition { get; set; }
        public MatchedSubstring(string text, int startPosition, int stringNumber) 
        {
            Text = text;
            StringNumber = stringNumber;
            StartPosition = startPosition;
        }
    }

    public static class MyRegExp
    {
        public static List<MatchedSubstring> MatchedSubstrings = new List<MatchedSubstring>();
        public static Regex RegexIPV6 = new Regex(@"(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))");
        public static Regex Regex2010 = new Regex(@"(201[0-9])|(202[0-2])");


        public static void Match2010(string text)
        {
            MatchedSubstrings.Clear();
            List<string> textStrings = text.Split("\n").ToList();

            for (int i = 0; i < textStrings.Count; i++)
            {
                MatchCollection matches = Regex2010.Matches(textStrings[i]);

                foreach (Match match in matches)
                {
                    MatchedSubstrings.Add(new MatchedSubstring(match.Value, match.Index, i + 1));
                }
            }
        }

        public static void MatchIPV6(string text)
        {
            MatchedSubstrings.Clear();
            List<string> textStrings = text.Split("\n").ToList();
            
            for(int i = 0; i < textStrings.Count; i++)
            {
                MatchCollection matches = RegexIPV6.Matches(textStrings[i]);

                foreach (Match match in matches)
                {
                    MatchedSubstrings.Add(new MatchedSubstring(match.Value, match.Index, i + 1));
                }
            }
        }

        public static void Match10(string text)
        {
            MatchedSubstrings.Clear();
            Machine machine = new Machine(text);
            machine.Analyze();
            MatchedSubstrings = machine.MatchedSubstrings;
        }
    }
}
