namespace VMTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            CodeWriter writer = new CodeWriter(Path.GetDirectoryName(args[0]) + "/" + Path.GetDirectoryName(args[0])?.Split(Path.DirectorySeparatorChar).Last() + ".asm");
            //writer.WriteInit();
            
            string[] files = Directory.GetFiles(Path.GetDirectoryName(args[0]), "*.vm");

            foreach (string file in files)
            {
                Parser parser = new Parser(file);
                writer.InputFileName = Path.GetFileNameWithoutExtension(file);

                while(parser.HasMoreCommands())
                {
                    writer.WriteCurrentCommandInComment(parser.Advance());

                    switch(parser.CurrentCommandType)
                    {
                        case CommandType.C_ARITHMETIC:
                            writer.WriteArithmetic(parser.Arg1());
                            break;
                        case CommandType.C_POP:
                        case CommandType.C_PUSH:
                            writer.WritePushPop(parser.CurrentCommandType, parser.Arg1(), parser.Arg2());
                            break;
                        case CommandType.C_LABEL:
                            writer.WriteLabel(parser.Arg1());
                            break;
                        case CommandType.C_IF:
                            writer.WriteIf(parser.Arg1());
                            break;
                        case CommandType.C_GOTO:
                            writer.WriteGoto(parser.Arg1());
                            break;
                    }
                }   
            }
            
            writer.Close();
        }
    }
}