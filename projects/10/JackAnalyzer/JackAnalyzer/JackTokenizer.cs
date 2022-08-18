using System.Text.RegularExpressions;

namespace JackAnalyzer
{
    enum TokenType
    {
        NULL,
        KEYWORD,
        SYMBOL,
        IDENTIFIER,
        INT_CONST,
        STRING_CONST
    }

    /// <summary>
    /// Breaks the input stream into Jack tokens
    /// </summary>
    class JackTokenizer
    {
        StreamWriter writer;
        StreamReader reader;
        public string CurrentToken { get; private set; }
        public TokenType CurrentTokenType { get; private set; }
        List<Match> matches;

        string keywordPattern = "class|constructor|function|method|field|static|var|int|char|boolean|void|" +
                                    "true|false|null|this|let|do|if|else|while|return";

        string symbolPattern = "{|}|\\(|\\)|\\[|\\]|\\.|,|;|\\+|-|\\*|/|&|\\||<|>|=|~";

        string identifierPattern = "\\w+";

        string intPattern = "\\d+";

        string stringPattern = "\".*\"";

        public JackTokenizer(string inputFile)
        {
            writer = new StreamWriter(inputFile.Replace(".jack", "_outT.xml"));
            reader = new StreamReader(inputFile);
            CurrentToken = string.Empty;
            CurrentTokenType = TokenType.NULL;
            matches = new List<Match>();

            writer.WriteLine("<tokens>");
        }

        void WriteToken(string tokenType, string token)
        {
            writer.WriteLine($"<{tokenType}> {token} </{tokenType}>");
        }

        public bool HasMoreTokens()
        {
            bool hasMoreCommands = !reader.EndOfStream;
            if (!hasMoreCommands)
            {
                writer.WriteLine("</tokens>");
                writer.Close();
                reader.Close();
            }

            return hasMoreCommands;
        }

        List<Match> GetNewMatches()
        {
            string? line = reader.ReadLine()?.Trim();

            while (!reader.EndOfStream && (line == null || string.IsNullOrWhiteSpace(line) || line[0] == '*' || line[0] == '/'))
            {
                line = reader.ReadLine()?.Trim();
            }

            line = line.Split("//")[0];

            Regex re = new Regex($"{keywordPattern}|{symbolPattern}|{identifierPattern}|{intPattern}|{stringPattern}");
            return re.Matches(line).ToList();
        }

        public void Advance()
        {
            if(matches.Count == 0)
                matches = GetNewMatches();

            if (matches.Count == 0)
                return;

            GetToken(matches[0].Value.Trim());
            matches.RemoveAt(0);
        }

        string GetToken(string word)
        {
            CurrentToken = word;

            if (new Regex(keywordPattern).IsMatch(word))
            {
                CurrentTokenType = TokenType.KEYWORD;
                WriteToken("keyword", word);
            }
            else if (new Regex(symbolPattern).IsMatch(word))
            {
                CurrentTokenType = TokenType.SYMBOL;
                word = (word) switch
                {
                    "<" => "&lt;",
                    ">" => "&gt;",
                    "&" => "&amp;",
                    _ => word
                };

                WriteToken("symbol", word);
            }
            else if (new Regex(intPattern).IsMatch(word))
            {
                CurrentTokenType = TokenType.INT_CONST;
                WriteToken("integerConstant", word);
            }
            else if (new Regex(stringPattern).IsMatch(word))
            {
                CurrentTokenType = TokenType.STRING_CONST;
                WriteToken("stringConstant", word.Replace("\"", string.Empty));
            }
            else if (new Regex(identifierPattern).IsMatch(word))
            {
                CurrentTokenType = TokenType.IDENTIFIER;
                WriteToken("identifier", word);
            }

            return word;
        }
    }
}