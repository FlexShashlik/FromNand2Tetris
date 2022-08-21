namespace JackAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(Path.GetDirectoryName(args[0]), "*.jack");

            foreach (string file in files)
            {
                CompilationEngine engine = new CompilationEngine(new JackTokenizer(file), file.Replace(".jack", "_out.xml"));
                engine.CompileClass();
            }
        }
    }
}