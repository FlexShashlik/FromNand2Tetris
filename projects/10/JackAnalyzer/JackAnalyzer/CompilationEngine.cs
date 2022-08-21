using System.Diagnostics;

namespace JackAnalyzer
{
    /// <summary>
    /// Effects the actual compilation output
    /// </summary>
    class CompilationEngine
    {
        JackTokenizer tokenizer;
        StreamWriter file;
        int indent;
        const int indentSize = 2;
        Stack<Token> token;

        List<string> typeStructureTokens = new List<string> { "int", "char", "boolean" };

        public CompilationEngine(JackTokenizer tokenizer, string outputFile)
        {
            file = new StreamWriter(outputFile);
            this.tokenizer = tokenizer;
            indent = 0;
            token = new Stack<Token>();
        }

        Token PeekToken()
        {
            if (token.Count == 0)
            {
                tokenizer.Advance();
                token.Push(tokenizer.CurrentToken);
            }

            return token.Peek();
        }

        void WriteNonTerminal(string nonTerminal, bool isStartingTag)
        {
            indent -= isStartingTag ? 0 : indentSize;
            string slash = isStartingTag ? string.Empty : "/";
            file.WriteLine(new String(' ', indent) + $"<{slash}{nonTerminal}>");
            indent += isStartingTag ? indentSize : 0;
        }
        void WriteTerminal()
        {
            Token t = token.Pop();

            if(t.Type == "stringConstant")
            {
                t.Value = t.Value.Replace("\"", string.Empty);
            }
            else
            {
                t.Value = (t.Value) switch
                {
                    "<" => "&lt;",
                    ">" => "&gt;",
                    "&" => "&amp;",
                    _ => t.Value
                };
            }

            file.WriteLine(new String(' ', indent) + $"<{t.Type}> {t.Value} </{t.Type}>");
        }

        void WriteType()
        {
            Debug.Assert(typeStructureTokens.Contains(PeekToken().Value) || PeekToken().Type == "identifier");
            WriteTerminal();
        }

        void WriteIdentifier()
        {
            Debug.Assert(PeekToken().Type == "identifier");
            WriteTerminal();
        }

        public void CompileClass()
        {
            Debug.Assert(PeekToken().Value == "class");
            WriteNonTerminal("class", true);

            WriteTerminal();

            // Write ClassName
            Debug.Assert(PeekToken().Type == "identifier");
            WriteTerminal();

            Debug.Assert(PeekToken().Value == "{");
            WriteTerminal();

            List<string> varDecTokens = new List<string> { "static", "field" };

            while (varDecTokens.Contains(PeekToken().Value))
            {
                CompileClassVarDec();
            }

            List<string> subroutineTokens = new List<string> { "constructor", "function", "method" };

            while (subroutineTokens.Contains(PeekToken().Value))
            {
                CompileSubroutine();
            }

            Debug.Assert(PeekToken().Value == "}");
            WriteTerminal();
            WriteNonTerminal("class", false);
            file.Close();
            tokenizer.Close();
        }

        public void CompileClassVarDec()
        {
            WriteNonTerminal("classVarDec", true);

            // Write structure token
            WriteTerminal();

            WriteType();

            // Write varName
            WriteIdentifier();

            while(PeekToken().Value == ",")
            {
                WriteTerminal();
                // Write varName
                WriteIdentifier();
            }

            Debug.Assert(PeekToken().Value == ";");
            WriteTerminal();

            WriteNonTerminal("classVarDec", false);
        }

        void CompileSubroutine()
        {
            WriteNonTerminal("subroutineDec", true);

            // Write structure token
            WriteTerminal();

            Debug.Assert(PeekToken().Value == "void" || typeStructureTokens.Contains(PeekToken().Value) || PeekToken().Type == "identifier");
            WriteTerminal();

            // Write subroutineName
            WriteIdentifier();

            Debug.Assert(PeekToken().Value == "(");
            WriteTerminal();

            CompileParameterList();

            Debug.Assert(PeekToken().Value == ")");
            WriteTerminal();

            WriteNonTerminal("subroutineBody", true);
            Debug.Assert(PeekToken().Value == "{");
            WriteTerminal();

            while (PeekToken().Value == "var")
            {
                CompileVarDec();
            }

            CompileStatements();

            Debug.Assert(PeekToken().Value == "}");
            WriteTerminal();
            WriteNonTerminal("subroutineBody", false);

            WriteNonTerminal("subroutineDec", false);
        }

        void CompileParameterList()
        {
            WriteNonTerminal("parameterList", true);

            if (typeStructureTokens.Contains(PeekToken().Value) || PeekToken().Type == "identifier")
            {
                // Write type
                WriteTerminal();

                // Write varName
                WriteIdentifier();

                while (PeekToken().Value == ",")
                {
                    WriteTerminal();

                    // Write type
                    Debug.Assert(typeStructureTokens.Contains(PeekToken().Value) || PeekToken().Type == "identifier");
                    WriteTerminal();

                    // Write varName
                    WriteIdentifier();
                }
            }

            WriteNonTerminal("parameterList", false);
        }

        void CompileVarDec()
        {
            Debug.Assert(PeekToken().Value == "var"); 
            WriteNonTerminal("varDec", true);
            WriteTerminal();

            WriteType();

            // Write varName
            WriteIdentifier();

            while (PeekToken().Value == ",")
            {
                WriteTerminal();

                // Write varName
                WriteIdentifier();
            }

            Debug.Assert(PeekToken().Value == ";");
            WriteTerminal();

            WriteNonTerminal("varDec", false);
        }

        void CompileStatements()
        {
            WriteNonTerminal("statements", true);
            List<string> structureTokens = new List<string> { "let", "if", "while", "do", "return" };

            while (structureTokens.Contains(PeekToken().Value))
            {
                switch (PeekToken().Value)
                {
                    case "let":
                        CompileLet();
                        break;
                    case "if":
                        CompileIf();
                        break;
                    case "while":
                        CompileWhile();
                        break;
                    case "do":
                        CompileDo();
                        break;
                    case "return":
                        CompileReturn();
                        break;
                };
            }

            WriteNonTerminal("statements", false);
        }

        void CompileLet()
        {
            Debug.Assert(PeekToken().Value == "let");
            WriteNonTerminal("letStatement", true);
            WriteTerminal();

            // Write varName
            WriteIdentifier();

            if(PeekToken().Value == "[")
            {
                WriteTerminal();
                CompileExpression();
                Debug.Assert(PeekToken().Value == "]");
                WriteTerminal();
            }

            Debug.Assert(PeekToken().Value == "=");
            WriteTerminal();
            CompileExpression();
            Debug.Assert(PeekToken().Value == ";");
            WriteTerminal();

            WriteNonTerminal("letStatement", false);
        }

        void CompileIf()
        {
            Debug.Assert(PeekToken().Value == "if");
            WriteNonTerminal("ifStatement", true);
            WriteTerminal();

            Debug.Assert(PeekToken().Value == "(");
            WriteTerminal();

            CompileExpression();

            Debug.Assert(PeekToken().Value == ")");
            WriteTerminal();

            Debug.Assert(PeekToken().Value == "{");
            WriteTerminal();

            CompileStatements();

            Debug.Assert(PeekToken().Value == "}");
            WriteTerminal();

            if(PeekToken().Value == "else")
            {
                WriteTerminal();

                Debug.Assert(PeekToken().Value == "{");
                WriteTerminal();

                CompileStatements();

                Debug.Assert(PeekToken().Value == "}");
                WriteTerminal();
            }

            WriteNonTerminal("ifStatement", false);
        }

        void CompileWhile()
        {
            Debug.Assert(PeekToken().Value == "while");
            WriteNonTerminal("whileStatement", true);
            WriteTerminal();

            Debug.Assert(PeekToken().Value == "(");
            WriteTerminal();

            CompileExpression();

            Debug.Assert(PeekToken().Value == ")");
            WriteTerminal();

            Debug.Assert(PeekToken().Value == "{");
            WriteTerminal();

            CompileStatements();

            Debug.Assert(PeekToken().Value == "}");
            WriteTerminal();

            WriteNonTerminal("whileStatement", false);
        }

        void CompileDo()
        {
            Debug.Assert(PeekToken().Value == "do");
            WriteNonTerminal("doStatement", true);
            WriteTerminal();

            // SubroutineCall
            WriteIdentifier();

            if (PeekToken().Value == "(")
            {
                WriteTerminal();
                CompileExpressionList();
                Debug.Assert(PeekToken().Value == ")");
                WriteTerminal();
            }
            else if (PeekToken().Value == ".")
            {
                WriteTerminal();
                WriteIdentifier();
                Debug.Assert(PeekToken().Value == "(");
                WriteTerminal();
                CompileExpressionList();
                Debug.Assert(PeekToken().Value == ")");
                WriteTerminal();
            }

            Debug.Assert(PeekToken().Value == ";");
            WriteTerminal();

            WriteNonTerminal("doStatement", false);
        }

        void CompileReturn()
        {
            Debug.Assert(PeekToken().Value == "return");
            WriteNonTerminal("returnStatement", true);
            WriteTerminal();

            if(PeekToken().Value != ";")
            {
                CompileExpression();
            }

            Debug.Assert(PeekToken().Value == ";");
            WriteTerminal();

            WriteNonTerminal("returnStatement", false);
        }

        void CompileExpression()
        {
            List<string> ops = new List<string> { "+", "-", "*", "/", "&", "|", "<", ">", "="};
            WriteNonTerminal("expression", true);

            CompileTerm();

            while(ops.Contains(PeekToken().Value))
            {
                WriteTerminal();
                CompileTerm();
            }

            WriteNonTerminal("expression", false);
        }

        void CompileTerm()
        {
            WriteNonTerminal("term", true);

            List<string> keywordConstants = new List<string> { "true", "false", "null", "this" };

            if(PeekToken().Type == "integerConstant")
            {
                WriteTerminal();
            }
            else if (PeekToken().Type == "stringConstant")
            {
                WriteTerminal();
            }
            else if(keywordConstants.Contains(PeekToken().Value))
            {
                WriteTerminal();
            }
            else if (PeekToken().Type == "identifier")
            { 
                WriteIdentifier();

                if (PeekToken().Value == "[")
                {
                    WriteTerminal();
                    CompileExpression();
                    Debug.Assert(PeekToken().Value == "]");
                    WriteTerminal();
                }
                else if (PeekToken().Value == "(")
                {
                    WriteTerminal();
                    CompileExpressionList();
                    Debug.Assert(PeekToken().Value == ")");
                    WriteTerminal();
                }
                else if (PeekToken().Value == ".")
                {
                    WriteTerminal();
                    WriteIdentifier();
                    Debug.Assert(PeekToken().Value == "(");
                    WriteTerminal();
                    CompileExpressionList();
                    Debug.Assert(PeekToken().Value == ")");
                    WriteTerminal();
                }
            }
            else if (PeekToken().Value == "(")
            {
                WriteTerminal();
                CompileExpression();
                Debug.Assert(PeekToken().Value == ")");
                WriteTerminal();
            }
            else if (PeekToken().Value == "-" || PeekToken().Value == "~")
            {
                WriteTerminal();
                CompileTerm();
            }

            WriteNonTerminal("term", false);
        }

        void CompileExpressionList()
        {
            WriteNonTerminal("expressionList", true);

            if (PeekToken().Value != ")")
            {
                CompileExpression();

                while(PeekToken().Value == ",")
                {
                    WriteTerminal();

                    CompileExpression();
                }
            }

            WriteNonTerminal("expressionList", false);
        }
    }
}