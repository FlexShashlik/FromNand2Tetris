namespace VMTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            CodeWriter writer = new CodeWriter(Path.GetDirectoryName(args[0]) + "/" + Path.GetDirectoryName(args[0])?.Split(Path.DirectorySeparatorChar).Last() + ".asm");

            if(args[0].Contains('.')) // If it is a single .vm file 
            {
                Parser parser = new Parser(args[0]);

                while(parser.HasMoreCommands())
                {
                    parser.Advance();
                    
                    switch(parser.CurrentCommandType())
                    {
                        case CommandType.C_ARITHMETIC:
                            writer.WriteArithmetic(parser.Arg1());
                            break;
                        case CommandType.C_PUSH:
                            writer.WritePushPop(CommandType.C_PUSH, parser.Arg1(), parser.Arg2());
                            break;
                    }
                }          
            }
            else // If it is a directory
            {

            }
            
            writer.Close();
        }
    }
}