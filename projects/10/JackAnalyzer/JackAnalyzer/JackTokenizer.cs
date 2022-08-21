using System.Text.RegularExpressions;

namespace JackAnalyzer
{
    class Token
    {
        public string Value = string.Empty;
        public string Type = string.Empty;
    }

    /// <summary>
    /// Breaks the input stream into Jack tokens
    /// </summary>
    class JackTokenizer
    {
        StreamWriter writer;
        StreamReader reader;
        public Token CurrentToken { get; private set; }
        List<Match> matches;

        string keywordPattern = "^class$|^constructor$|^function$|^method$|^field$|^static$|^var$|^int$|^char$|^boolean$|^void$|" +
                                    "^true$|^false$|^null$|^this$|^let$|^do$|^if$|^else$|^while$|^return$";

        string symbolPattern = "{|}|\\(|\\)|\\[|\\]|\\.|,|;|\\+|-|\\*|/|&|\\||<|>|=|~";

        string identifierPattern = "\\w+";

        string intPattern = "\\d+";

        string stringPattern = "\".*\"";

        public JackTokenizer(string inputFile)
        {
            writer = new StreamWriter(inputFile.Replace(".jack", "_outT.xml"));
            reader = new StreamReader(inputFile);
            CurrentToken = new Token();
            matches = new List<Match>();

            writer.WriteLine("<tokens>");
        }

        void WriteToken(string tokenType, string token)
        {
            writer.WriteLine($"<{tokenType}> {token} </{tokenType}>");
        }

        public void Close()
        {
            writer.WriteLine("</tokens>");
            writer.Close();
            reader.Close();
        }

        List<Match> GetNewMatches()
        {
            string? line = reader.ReadLine()?.Trim();

            while (line == null || string.IsNullOrWhiteSpace(line) || line[0] == '*' || line[0] == '/')
            {
                line = reader.ReadLine()?.Trim();
            }

            line = line.Split("//")[0];

            Regex re = new Regex($"{keywordPattern}|{symbolPattern}|{identifierPattern}|{intPattern}|{stringPattern}");
            return re.Matches(line).ToList();
        }

        public void Advance()
        {
            if (matches.Count == 0)
                matches = GetNewMatches();

            if (matches.Count == 0)
                return;

            GetToken(matches[0].Value.Trim());
            matches.RemoveAt(0);
        }

        void GetToken(string word)
        {
            CurrentToken.Value = word;

            if (new Regex(keywordPattern).IsMatch(word))
            {
                CurrentToken.Type = "keyword";
                WriteToken(CurrentToken.Type, word);
            }
            else if (new Regex(symbolPattern).IsMatch(word))
            {
                CurrentToken.Type = "symbol";
                word = (word) switch
                {
                    "<" => "&lt;",
                    ">" => "&gt;",
                    "&" => "&amp;",
                    _ => word
                };

                WriteToken(CurrentToken.Type, word);
            }
            else if (new Regex(intPattern).IsMatch(word))
            {
                CurrentToken.Type = "integerConstant";
                WriteToken(CurrentToken.Type, word);
            }
            else if (new Regex(stringPattern).IsMatch(word))
            {
                CurrentToken.Type = "stringConstant";
                WriteToken(CurrentToken.Type, word.Replace("\"", string.Empty));
            }
            else if (new Regex(identifierPattern).IsMatch(word))
            {
                CurrentToken.Type = "identifier";
                WriteToken(CurrentToken.Type, word);
            }
        }
    }
}