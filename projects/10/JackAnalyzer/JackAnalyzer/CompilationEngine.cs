namespace JackAnalyzer
{
    /// <summary>
    /// Effects the actual compilation output
    /// </summary>
    class CompilationEngine
    {
        JackTokenizer tokenizer;
        StreamWriter file;
        Stack<string> tokens;
        int indent;

        public CompilationEngine(JackTokenizer tokenizer, string outputFile)
        {
            //file = new StreamWriter(outputFile);
            this.tokenizer = tokenizer;
            tokens = new Stack<string>();
            indent = 0;
        }

        void WriteToken(string token)
        {
            file.WriteLine(new String(' ', indent) + $"<{token}>");
        }

        public void CompileClass()
        {
            while (tokenizer.HasMoreTokens())
            {
                tokenizer.Advance();
            }

            //WriteToken("class");
            indent += 2;
        }

        public void CompileClassVarDec()
        {

        }

        public void CompileSubroutine()
        {

        }

        public void CompileParameterList()
        {

        }

        public void CompileVarDec()
        {

        }

        public void CompileStatements()
        {

        }

        public void CompileDo()
        {

        }

        public void CompileLet()
        {

        }

        public void CompileWhile()
        {

        }

        public void CompileReturn()
        {

        }

        public void CompileIf()
        {

        }

        public void CompileExpression()
        {

        }

        public void CompileTerm()
        {

        }

        public void CompileExpressionList()
        {

        }
    }
}